﻿using Dythervin.AutoAttach;
using System;
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
        public event Action OnRPMUpdate;
        public event Action OnSpeedUpdate;
        public event Action OnGearUpdate;

        [field: SerializeField] public CarInfoSO CarInfoSO { get; private set; }
        [SerializeField, Attach] CarInput carInput;
        [SerializeField] DriveUnit _driveUnit;
        [SerializeField, Attach(Attach.Child)] List<Wheel> _wheels;

        List<Wheel> _frontWheels = new();
        List<Wheel> _rearWheels = new();

        //temp
        public float SpeedAuto { get; set; }
        public float RPMEngine { get; set; }
        public int CurrentGear { get; set; }
        //end temp

        public float CurrentMotorForce { get; set; }

        float _horizontalInput;
        float _verticalInput;
        float _steeringAngle;
        bool _isClutchPressed;
        GearState _gearState;
        float _clutch;
        float wheelRPM;

        WaitForSeconds _waitForChangeGearInS = new WaitForSeconds(0.5f);
        WaitForSeconds _waitForDecreaseGearInS = new WaitForSeconds(0.1f);
        WaitForSeconds _waitForIncreaseGearInS = new WaitForSeconds(0.7f);

        void OnEnable()
        {
            carInput.OnGetInput += GetInput;
            carInput.OnGetGearInputShiftUp += GetGearInputUp;
            carInput.OnGetGearInputShiftDown += GetGearInputDown;
            carInput.OnClutch += HandleClutch;
        }

        void OnDisable()
        {
            carInput.OnGetInput -= GetInput;
            carInput.OnGetGearInputShiftUp -= GetGearInputUp;
            carInput.OnGetGearInputShiftDown -= GetGearInputDown;
            carInput.OnClutch -= HandleClutch;
        }

        void Awake()
        {
            SetupWheels();
        }

        void FixedUpdate()
        {
            Steer();
            ApplyBrake();
            Accelerate();
            UpdateWheelPoses();
        }

        void GetGearInputUp()
        {
            if (CurrentGear < 5)
                StartCoroutine(ChangeGear(1));
        }

        void GetGearInputDown()
        {
            if (CurrentGear > 0)
                StartCoroutine(ChangeGear(-1));
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
                    wheel.WheelCollider.brakeTorque = CarInfoSO.BrakeForce;
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

        void GetInput(float horizontalInput, float verticalInput)
        {
            _horizontalInput = horizontalInput;
            _verticalInput = verticalInput;

            CalculateClutch();
        }

        void HandleClutch(bool isClutchPressed)
        {
            _isClutchPressed = isClutchPressed;
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
                        _clutch = _isClutchPressed
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
            _steeringAngle = CarInfoSO.MaxSteerAngle * _horizontalInput;
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

            if (RPMEngine < CarInfoSO.IdleRPM + CarInfoSO.IdleRPMLimit
                && _verticalInput == -0
                && CurrentGear == 0)
            {
                _gearState = GearState.Neutral;
            }

            if (_clutch < Mathf.Epsilon)
            {
                RPMEngine = Mathf.Lerp(RPMEngine, Mathf.Max(CarInfoSO.IdleRPM, CarInfoSO.RedLine * _verticalInput)
                    + Random.Range(-CarInfoSO.RandomAdditionalRPM, CarInfoSO.RandomAdditionalRPM), Time.deltaTime);
            }
            else
            {
                wheelRPM = Mathf.Abs((_wheels[0].WheelCollider.rpm + _wheels[1].WheelCollider.rpm) / 2f)
                    * CarInfoSO.GearRatios[CurrentGear] * CarInfoSO.DifferentialRatio;

                RPMEngine = Mathf.Lerp(RPMEngine, Mathf.Max(CarInfoSO.IdleRPM - CarInfoSO.IdleRPM / 2f, wheelRPM), Time.deltaTime * 3f);

                currentTorque = (CarInfoSO.HpToRPMCurve.Evaluate(RPMEngine / CarInfoSO.RedLine) * CurrentMotorForce / RPMEngine)
                    * CarInfoSO.GearRatios[CurrentGear] * CarInfoSO.DifferentialRatio * 5252f * _clutch;
            }

            OnRPMUpdate?.Invoke();

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
                SpeedAuto = ConvertMToKM(wheel.WheelCollider.attachedRigidbody.velocity.magnitude);

                if (SpeedAuto < CarInfoSO.MaxSpeedPerGear[CurrentGear])
                {
                    wheel.WheelCollider.motorTorque = currentTorque * _verticalInput;
                }
                else
                {
                    wheel.WheelCollider.motorTorque = 0;
                }
            }
            OnSpeedUpdate?.Invoke();
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
                    wheelTransform.Rotate(Vector3.up, CarInfoSO.RotateAngle);
                }
            }
        }

        IEnumerator ChangeGear(int gearChange)
        {
            _gearState = GearState.CheckingChange;
            if (CurrentGear + gearChange >= 0)
            {
                switch (gearChange)
                {
                    case > 0:
                        {
                            yield return _waitForIncreaseGearInS;
                            if (CurrentGear >= CarInfoSO.GearRatios.Length - 1)
                            {
                                _gearState = GearState.Running;
                                yield break;
                            }
                            break;
                        }
                    case < 0:
                        {
                            yield return _waitForDecreaseGearInS;

                            if (CurrentGear <= 0)
                            {
                                _gearState = GearState.Running;
                                yield break;
                            }
                            break;
                        }
                }
                _gearState = GearState.Changing;
                yield return _waitForChangeGearInS;

                CurrentGear += gearChange;
            }

            if (_gearState != GearState.Neutral)
                _gearState = GearState.Running;

            OnGearUpdate?.Invoke();
        }
    }
}