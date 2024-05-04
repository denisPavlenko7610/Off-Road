using UnityEngine;

namespace Off_Road
{
    public class CarTank : MonoBehaviour
    {
        [SerializeField] private float _maxFuel = 100f;
        [SerializeField] private float _fuelConsumptionRate = 1f;
        [SerializeField] private float _currentFuel;

        private void Start()
        {
            _currentFuel = _maxFuel;
        }

        public float MaxFuel
        {
            get { return _maxFuel; }
        }

        public float CurrentFuel
        {
            get { return _currentFuel; }
        }

        private void Update()
        {
            ConsumeFuel(_fuelConsumptionRate * Time.deltaTime);

            if (_currentFuel <= 0f)
                Debug.Log("Out of fuel");
        }

        private void ConsumeFuel(float value)
        {
            _currentFuel -= value;
        }

        public void Refuel(float value)
        {
            _currentFuel += value;
        }
    }
}
