using Dythervin.AutoAttach;
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
        [SerializeField] CarInfoScriptableObject _carCutlass;
        [SerializeField] InputManager _inputManager;
        [SerializeField] DriveUnit _driveUnit;
        [SerializeField, Attach(Attach.Child)] List<Wheel> _wheels;

        List<Wheel> _frontWheels = new();
        List<Wheel> _rearWheels = new();

        [SerializeField] float _speed;
        [SerializeField] float _maxSteerAngle;
        [SerializeField] float[] _maxSpeedPerGear;
        [SerializeField] float _RPM;
        [SerializeField] float _idleRPM;
        [SerializeField] int _currentGear;
        [SerializeField] float[] _gearRatios;
        [SerializeField] float _differentialRatio = 4f;
        [SerializeField] float _redLine;
        [SerializeField] float _brakeForce;
        [SerializeField] AnimationCurve _hpToRPMCurve;

        [field: SerializeField]
        public float MotorForce { get; set; }

        [field: SerializeField]
        public float MotorForceAtStart { get; set; }

        float _horizontalInput;
        float _verticalInput;
        float _steeringAngle;
        readonly float _rotateAngle = 180f;
        bool _isClutchPressed;
        GearState _gearState;
        float _clutch;
        float wheelRPM;
        int _randomAdditinalRPM = 50;
        float _idleRPMLimit = 200f;

        WaitForSeconds _waitForChangeGearInS = new WaitForSeconds(0.5f);
        WaitForSeconds _waitForDecreaseGearInS = new WaitForSeconds(0.1f);
        WaitForSeconds _waitForIncreaseGearInS = new WaitForSeconds(0.7f);

        void OnEnable()
        {
            _inputManager.OnGetInput += GetInput;
            _inputManager.OnGetGearInputShiftUp += GetGearInputUp;
            _inputManager.OnGetGearInputShiftDown += GetGearInputDown;
            _inputManager.OnClutch += HandleClutch;
        }

        void OnDisable()
        {
            _inputManager.OnGetInput -= GetInput;
            _inputManager.OnGetGearInputShiftUp -= GetGearInputUp;
            _inputManager.OnGetGearInputShiftDown -= GetGearInputDown;
            _inputManager.OnClutch -= HandleClutch;
        }

        void Awake()
        {
            SetupWheels();
        }

        void Start()
        {
            _maxSteerAngle = _carCutlass.maxSteerAngle;
            MotorForce = _carCutlass.MotorForce;
            _brakeForce = _carCutlass.brakeForce;
            _idleRPM = _carCutlass._idleRPM;
            _redLine = _carCutlass._redLine;

            MotorForceAtStart = MotorForce;
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
            if (_currentGear < 5)
                StartCoroutine(ChangeGear(1));
        }

        void GetGearInputDown()
        {
            if (_currentGear > 0)
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
                    wheel.WheelCollider.brakeTorque = _brakeForce;
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
            _steeringAngle = _maxSteerAngle * _horizontalInput;
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