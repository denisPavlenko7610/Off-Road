using Off_Road.Car;
using UnityEngine;

namespace Off_Road
{
    public class StartStopEngine : MonoBehaviour
    {
        [SerializeField] CarController _carController;
        [SerializeField] CarTank _carTank;
        [SerializeField] ParticleSystem[] ParticleSystems;

        bool _isRunning = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && _isRunning)
            {
                _carTank.FuelConsumptionRate = 0f;
                _carController.MotorForce = 0f;

                foreach (ParticleSystem Smoke in ParticleSystems)
                {
                    Smoke.Stop();
                }
                Debug.Log("Engine is stopped");
                _isRunning = false;
            }

            else if (Input.GetKeyDown(KeyCode.E) && !_isRunning)
            {

                _carController.MotorForce = _carController.MotorForceAtStart;
                _carTank.FuelConsumptionRate = _carTank.FuelConsumptionRateAtStart;

                foreach (ParticleSystem Smoke in ParticleSystems)
                {
                    Smoke.Play();
                }
                Debug.Log("Engine is started");
                _isRunning = true;
            }
        }
    }

}

