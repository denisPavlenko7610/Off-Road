using Off_Road.Car;
using UnityEngine;

namespace Off_Road
{
    public class EngineControl : MonoBehaviour
    {
        [SerializeField] CarController _carController;
        [SerializeField] CarTank _carTank;
        [SerializeField] ParticleSystem[] _particleSystems;

        bool _isRunning;

        void Start()
        {
            StopEngine();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_isRunning)
                {
                    StopEngine();
                }
                else
                {
                    StartEngine();
                }
            }
        }

        void StartEngine()
        {
            SetEngineState(_carController.CarInfoSO.MotorForce, _carTank.FuelConsumptionRate, true, "Engine is started");
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

