using Off_Road.Utils;
using RDTools.AutoAttach;
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

        [SerializeField] EngineControl _engineControl;
        [field: SerializeField] public CarInfoSO CarInfoSO { get; private set; }
        [SerializeField, Attach] CarInput carInput;
        [SerializeField] DriveUnit _driveUnit;
        [SerializeField, Attach(Attach.Child)] List<Wheel> _wheels;

        List<Wheel> _frontWheels = new();
        List<Wheel> _rearWheels = new();

        public float SpeedAuto { get; private set; }
        public float RPMEngine { get; private set; }
        public int CurrentGear { get; private set; }

        public float CurrentMotorForce { get; set; }

        float _horizontalInput;
        float _verticalInput;
        float _steeringAngle;
        bool _isClutchPressed;
        GearState _gearState;
        float _clutch;
        float _wheelRPM;

        WaitForSeconds _waitForChangeGearInS = new WaitForSeconds(0.5f);
        WaitForSeconds _waitForDecreaseGearInS = new WaitForSeconds(0.1f);
        WaitForSeconds _waitForIncreaseGearInS = new WaitForSeconds(0.7f);
        WaitForSeconds _speedUpdateInterval = new WaitForSeconds(0.2f);

        Coroutine _speedUpdateCoroutine;
        float torqueMultiplier = 5252f;
        float value = 2f;

        void OnEnable()
        {
            SubscribeToCarInputEvents();
            _speedUpdateCoroutine = StartCoroutine(SpeedUpdateRoutine());
        }

        void Awake() => SetupWheels();

        void FixedUpdate()
        {
            Steer();
            ApplyBrake();
            Accelerate();
            UpdateWheelPoses();
        }

        void OnDisable()
        {
            UnSubscribeToCarInputEvents();
            if (_speedUpdateCoroutine != null)
                StopCoroutine(_speedUpdateCoroutine);
        }

        void SubscribeToCarInputEvents()
        {
            carInput.OnGetInput += GetInput;
            carInput.OnGetGearInputShiftUp += GetGearInputUp;
            carInput.OnGetGearInputShiftDown += GetGearInputDown;
            carInput.OnClutch += HandleClutch;
        }

        void UnSubscribeToCarInputEvents()
        {
            carInput.OnGetInput -= GetInput;
            carInput.OnGetGearInputShiftUp -= GetGearInputUp;
            carInput.OnGetGearInputShiftDown -= GetGearInputDown;
            carInput.OnClutch -= HandleClutch;
        }

        void GetGearInputUp()
        {
            if (CurrentGear < CarInfoSO.GearRatios.Length - 1)
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
                SpeedAuto = CarConverterUtils.ConvertMToKM(wheel.WheelCollider.attachedRigidbody.linearVelocity.magnitude);

                if (_verticalInput < Mathf.Epsilon)
                    wheel.WheelCollider.brakeTorque = 0;
                else
                    wheel.WheelCollider.brakeTorque = CarInfoSO.BrakeForce;
            }

            if (CurrentGear > 0)
            {
                float currentTorque = CalculateTorque();
            }
        }

        void SetupWheels()
        {
            foreach (Wheel wheel in _wheels)
            {
                if (IsFrontWheel(wheel))
                    _frontWheels.Add(wheel);
                else
                    _rearWheels.Add(wheel);
            }
        }

        void GetInput(float horizontalInput, float verticalInput)
        {
            _horizontalInput = horizontalInput;
            _verticalInput = verticalInput;

            CalculateClutch();
        }

        void HandleClutch(bool isClutchPressed) => _isClutchPressed = isClutchPressed;

        void CalculateClutch()
        {
            if (_gearState != GearState.Changing)
            {
                switch (_gearState)
                {
                    case GearState.Neutral:
                        {
                            _clutch = 0;
                            if (Mathf.Abs(_verticalInput) > Mathf.Epsilon && CurrentGear != 0)
                                _gearState = GearState.Running;

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
                wheel.WheelCollider.steerAngle = _steeringAngle;
        }

        bool IsFrontWheel(Wheel wheel) => wheel.WheelType == WheelType.FL
            || wheel.WheelType == WheelType.FR;

        void Accelerate()
        {
            if (_driveUnit == DriveUnit.Rear)
                AccelerateWheels(_rearWheels);
            else if (_driveUnit == DriveUnit.FrontWheel)
                AccelerateWheels(_frontWheels);
            else
                AccelerateWheels(_wheels);
        }

        float CalculateTorque()
        {
            float currentTorque = 0;

            if (CurrentGear == 0 && _engineControl.IsRunning)
            {
                RPMEngine = Mathf.Lerp(RPMEngine, Mathf.Max(CarInfoSO.IdleRPM, CarInfoSO.RedLine * -_verticalInput * value)
                    + Random.Range(-CarInfoSO.RandomAdditionalRPM, CarInfoSO.RandomAdditionalRPM), Time.deltaTime);
                _wheelRPM = 0f;
                RPMEngine = Mathf.Clamp(RPMEngine, 0f, CarInfoSO.RedLine);
            }

            if (RPMEngine < CarInfoSO.IdleRPM + CarInfoSO.IdleRPMLimit && _verticalInput == -0 && CurrentGear == 0)
                _gearState = GearState.Neutral;

            if (_clutch < Mathf.Epsilon)
            {
                RPMEngine = Mathf.Lerp(RPMEngine, Mathf.Max(CarInfoSO.IdleRPM, CarInfoSO.RedLine * _verticalInput)
                    + Random.Range(-CarInfoSO.RandomAdditionalRPM, CarInfoSO.RandomAdditionalRPM), Time.deltaTime);
            }
            else if (CurrentGear > 0)
            {
                _wheelRPM = Mathf.Abs((_wheels[0].WheelCollider.rpm + _wheels[1].WheelCollider.rpm) / value)
                    * CarInfoSO.GearRatios[CurrentGear] * CarInfoSO.DifferentialRatio;

                RPMEngine = Mathf.Lerp(RPMEngine, Mathf.Max(CarInfoSO.IdleRPM - CarInfoSO.IdleRPM / value, _wheelRPM), Time.deltaTime * 3f);

                currentTorque = (CarInfoSO.HpToRPMCurve.Evaluate(RPMEngine / CarInfoSO.RedLine) * CurrentMotorForce / RPMEngine)
                    * CarInfoSO.GearRatios[CurrentGear] * CarInfoSO.DifferentialRatio * torqueMultiplier * _clutch;
            }

            if (!_engineControl.IsRunning)
            {
                if (SpeedAuto > 0.1f)
                    DecreaseRpmEngine();
                else
                    RPMEngine = 0;
            }
            OnRPMUpdate?.Invoke();
            return currentTorque;
        }

        void DecreaseRpmEngine()
        {
            RPMEngine -= CarInfoSO.DecreaseRPMIntensity * Time.deltaTime;
            if (RPMEngine < CarInfoSO.IdleRPM)
                RPMEngine = CarInfoSO.IdleRPM;
        }

        void AccelerateWheels(List<Wheel> wheels)
        {
            if (_verticalInput > Mathf.Epsilon)
                return;

            float currentTorque = CalculateTorque();

            if (_gearState == GearState.Neutral)
                return;

            foreach (Wheel wheel in wheels)
            {
                SpeedAuto = CarConverterUtils.ConvertMToKM(wheel.WheelCollider.attachedRigidbody.linearVelocity.magnitude);

                if (SpeedAuto < CarInfoSO.MaxSpeedPerGear[CurrentGear])
                    wheel.WheelCollider.motorTorque = currentTorque * _verticalInput;
                else
                    wheel.WheelCollider.motorTorque = 0;
            }
        }

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

                if (wheel.WheelType == WheelType.FL || wheel.WheelType == WheelType.RL)
                    wheelTransform.Rotate(Vector3.up, CarInfoSO.RotateAngle);
            }
        }

        IEnumerator SpeedUpdateRoutine()
        {
            while (Application.isPlaying)
            {
                OnSpeedUpdate?.Invoke();
                yield return _speedUpdateInterval;
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