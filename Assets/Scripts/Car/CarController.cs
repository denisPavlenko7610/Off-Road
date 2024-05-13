﻿using Dythervin.AutoAttach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Off_Road.Car.Car;
using Random = UnityEngine.Random;

namespace Off_Road.Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        [SerializeField] DriveUnit _driveUnit;
        [SerializeField, Attach(Attach.Child)] List<Wheel> _wheels;

        List<Wheel> _frontWheels = new();
        List<Wheel> _rearWheels = new();

        [SerializeField] float _speed;
        [SerializeField] float maxSteerAngle = 30f;

        [field: SerializeField]
        public float MotorForce { get; set; } = 100f;

        [field: SerializeField]
        public float MotorForceAtStart { get; set; }

        [SerializeField] float brakeForce = 50_000f;

        float _horizontalInput;
        float _verticalInput;
        float _steeringAngle;
        readonly float _rotateAngle = 180f;

        [SerializeField] float[] _maxSpeedPerGear;
        [SerializeField] float _RPM;
        [SerializeField] float _idleRPM = 800f;

        [SerializeField] int _currentGear;
        GearState _gearState;
        float _clutch;

        [SerializeField] float[] _gearRatios;
        float wheelRPM;
        [SerializeField] float _differentialRatio = 4f;
        [SerializeField] AnimationCurve _hpToRPMCurve;
        [SerializeField] float _redLine = 6500f;

        int _randomAdditinalRPM = 50;
        float _idleRPMLimit = 200f;

        WaitForSeconds _waitForChangeGearInS = new WaitForSeconds(0.5f);
        WaitForSeconds _waitForDecreaseGearInS = new WaitForSeconds(0.1f);
        WaitForSeconds _waitForIncreaseGearInS = new WaitForSeconds(0.7f);

        void Awake()
        {
            SetupWheels();
        }

        private void Start()
        {
            MotorForceAtStart = MotorForce;
        }


        void Update()
        {
            GetInput();
            GetGearInput();
        }

        void FixedUpdate()
        {
            Steer();
            ApplyBrake();
            Accelerate();
            UpdateWheelPoses();
        }

        void GetGearInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (_currentGear < 5)
                    StartCoroutine(ChangeGear(1));

            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (_currentGear > 0)
                    StartCoroutine(ChangeGear(-1));
            }
        }

        void ApplyBrake()
        {
            foreach (Wheel wheel in _wheels)
            {
                if (_verticalInput < Mathf.Epsilon)
                {
                    wheel.WheelCollider.brakeTorque = 0;
                }
                else
                {
                    wheel.WheelCollider.brakeTorque = brakeForce;
                }
            }
        }

        void SetupWheels()
        {
            foreach (Wheel wheel in _wheels)
            {
                if (IsFrontWheel(wheel))
                {
                    _frontWheels.Add(wheel);
                }
                else
                {
                    _rearWheels.Add(wheel);
                }
            }
        }

        void GetInput()
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = -Input.GetAxis("Vertical");

            CalculateClutch();
        }
        void CalculateClutch()
        {
            if (_gearState != GearState.Changing)
            {
                switch (_gearState)
                {
                    case GearState.Neutral:
                        {
                            _clutch = 0;
                            if (Mathf.Abs(_verticalInput) > Mathf.Epsilon)
                            {
                                _gearState = GearState.Running;
                            }
                            break;
                        }
                    default:
                        _clutch = Input.GetKey(KeyCode.RightShift)
                            ? 0
                            : Mathf.Lerp(_clutch, 1, Time.deltaTime);
                        break;
                }
            }
            else
            {
                _clutch = 0;
            }
        }

        void Steer()
        {
            _steeringAngle = maxSteerAngle * _horizontalInput;
            foreach (var wheel in _frontWheels)
            {
                wheel.WheelCollider.steerAngle = _steeringAngle;
            }
        }

        bool IsFrontWheel(Wheel wheel)
            => wheel.WheelType == WheelType.FL
                || wheel.WheelType == WheelType.FR;

        void Accelerate()
        {
            if (_driveUnit == DriveUnit.Rear)
            {
                AccelerateWheels(_rearWheels);
            }
            else if (_driveUnit == DriveUnit.FrontWheel)
            {
                AccelerateWheels(_frontWheels);
            }
            else
            {
                AccelerateWheels(_wheels);
            }
        }

        float CalculateTorque()
        {
            float currentTorque = 0;

            if (_RPM < _idleRPM + _idleRPMLimit
                && _verticalInput == -0
                && _currentGear == 0)
            {
                _gearState = GearState.Neutral;
            }

            if (_clutch < Mathf.Epsilon)
            {
                _RPM = Mathf.Lerp(_RPM, Mathf.Max(_idleRPM, _redLine * _verticalInput)
                    + Random.Range(-_randomAdditinalRPM, _randomAdditinalRPM), Time.deltaTime);
            }
            else
            {
                wheelRPM = Mathf.Abs((_wheels[0].WheelCollider.rpm + _wheels[1].WheelCollider.rpm) / 2f)
                    * _gearRatios[_currentGear] * _differentialRatio;

                _RPM = Mathf.Lerp(_RPM, Mathf.Max(_idleRPM - _idleRPM / 2f, wheelRPM), Time.deltaTime * 3f);

                currentTorque = (_hpToRPMCurve.Evaluate(_RPM / _redLine) * MotorForce / _RPM)
                    * _gearRatios[_currentGear] * _differentialRatio * 5252f * _clutch;
            }

            return currentTorque;
        }

        void AccelerateWheels(List<Wheel> wheels)
        {
            if (_verticalInput > Mathf.Epsilon)
            {
                return;
            }

            float currentTorque = CalculateTorque();
            if (_gearState == GearState.Neutral)
            {
                return;
            }

            foreach (Wheel wheel in wheels)
            {
                _speed = ConvertMToKM(wheel.WheelCollider.attachedRigidbody.velocity.magnitude);

                if (_speed < _maxSpeedPerGear[_currentGear])
                {
                    wheel.WheelCollider.motorTorque = currentTorque * _verticalInput;
                }
                else
                {
                    wheel.WheelCollider.motorTorque = 0;
                }
            }
        }

        float ConvertMToKM(float value) => value * 3.6f;

        void UpdateWheelPoses()
        {
            foreach (Wheel wheel in _wheels)
            {
                Transform wheelTransform = wheel.WheelTransform.transform;
                Vector3 position = wheelTransform.position;
                Quaternion rotation = wheelTransform.rotation;

                wheel.WheelCollider.GetWorldPose(out position, out rotation);

                wheelTransform.position = position;
                wheelTransform.rotation = rotation;

                if (wheel.WheelType == WheelType.FL
                    || wheel.WheelType == WheelType.RL)
                {
                    wheelTransform.Rotate(Vector3.up, _rotateAngle);
                }
            }
        }

        IEnumerator ChangeGear(int gearChange)
        {
            _gearState = GearState.CheckingChange;
            if (_currentGear + gearChange >= 0)
            {
                switch (gearChange)
                {
                    case > 0:
                        {
                            yield return _waitForIncreaseGearInS;
                            if (_currentGear >= _gearRatios.Length - 1)
                            {
                                _gearState = GearState.Running;
                                yield break;
                            }
                            break;
                        }
                    case < 0:
                        {
                            yield return _waitForDecreaseGearInS;

                            if (_currentGear <= 0)
                            {
                                _gearState = GearState.Running;
                                yield break;
                            }
                            break;
                        }
                }
                _gearState = GearState.Changing;
                yield return _waitForChangeGearInS;

                _currentGear += gearChange;
            }

            if (_gearState != GearState.Neutral)
                _gearState = GearState.Running;
        }
    }
}