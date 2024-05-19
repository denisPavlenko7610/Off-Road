using Off_Road.Car;
using RDTools.AutoAttach;
using UnityEngine;

namespace Off_Road
{
    public class EngineControl : MonoBehaviour
    {
        [SerializeField, Attach] CarController _carController;
        [SerializeField, Attach] CarTank _carTank;
        [SerializeField] ParticleSystem[] _particleSystems;
        [SerializeField, Attach] CarInput _carInput;

        float _startFuelConsumptionRate;

        bool _isRunning;

        void Start()
        {
            _startFuelConsumptionRate = _carTank.FuelConsumptionRate;
            StopEngine();
        }

        void OnEnable()
        {
            _carInput.OnSetEngineState += ToggleSetStateEngine;
        }

        void OnDisable()
        {
            _carInput.OnSetEngineState -= ToggleSetStateEngine;
        }

        void ToggleSetStateEngine()
        {
            if (_isRunning)
                StopEngine();
            else
                StartEngine();
        }

        void StartEngine()
        {
            SetEngineState(_carController.CarInfoSO.MotorForce, _startFuelConsumptionRate, true, "Engine is started");
        }

        void StopEngine()
        {
            SetEngineState(0f, 0f, false, "Engine is stopped");
        }

        void SetEngineState(float motorForce, float fuelConsumptionRate, bool isRunning, string logMessage)
        {
            _carController.CurrentMotorForce = motorForce;
            _carTank.FuelConsumptionRate = fuelConsumptionRate;

            foreach (ParticleSystem Smoke in _particleSystems)
            {
                if (isRunning)
                {
                    Smoke.Play();
                }
                else
                {
                    Smoke.Stop();
                }
            }

            print(logMessage);
            _isRunning = isRunning;
        }
    }
}

