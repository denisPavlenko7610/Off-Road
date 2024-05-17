using Off_Road.Car;
using UnityEngine;

namespace Off_Road
{
    public class StartStopEngine : MonoBehaviour
    {
        [SerializeField] CarController _carController;
        [SerializeField] CarTank _carTank;
        [SerializeField] ParticleSystem[] _particleSystems;

        bool _isRunning = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && _isRunning)
            {
                StopEngine();
            }
            else if (Input.GetKeyDown(KeyCode.E) && !_isRunning)
            {
                StartEngine();
            }
        }

        void StartEngine()
        {
            _carController.MotorForce = _carController.MotorForceAtStart;
            _carTank.FuelConsumptionRate = _carTank.FuelConsumptionRateAtStart;

            foreach (ParticleSystem Smoke in _particleSystems)
            {
                Smoke.Play();
            }
            Debug.Log("Engine is started");
            _isRunning = true;
        }

        void StopEngine()
        {
            _carTank.FuelConsumptionRate = 0f;
            _carController.MotorForce = 0f;

            foreach (ParticleSystem Smoke in _particleSystems)
            {
                Smoke.Stop();
            }
            Debug.Log("Engine is stopped");
            _isRunning = false;
        }
    }
}

