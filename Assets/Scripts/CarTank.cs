using UnityEngine;

namespace Off_Road
{
    public class CarTank : MonoBehaviour
    {
        [SerializeField] private float _maxFuel = 100f;
        [SerializeField] private float _fuelConsumptionRate = 1f;
        [SerializeField] private float _currentFuel;

        void Start()
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

        void Update()
        {
            ConsumeFuel(_fuelConsumptionRate * Time.deltaTime);

            if (_currentFuel <= 0f)
                Debug.Log("Out of fuel");
        }

        void ConsumeFuel(float value)
        {
            _currentFuel -= value;
        }

        void Refuel(float value)
        {
            _currentFuel += value;
        }
    }
}
