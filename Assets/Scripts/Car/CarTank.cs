using System;
using UnityEngine;

namespace Off_Road
{
    public class CarTank : MonoBehaviour
    {
        [SerializeField] float _maxFuel = 100f;
        [SerializeField] float _currentFuel;
        [SerializeField] float _fuelConsumptionRate = 1f;

        public Action<float> OnFuelChanged;
        public Action OnTriggeredRefuel;
        public float MaxFuel { get { return _maxFuel; } }
        public float CurrentFuel { get { return _currentFuel; } }

        void Start()
        {
            _currentFuel = _maxFuel;
            _currentFuel = PlayerPrefs.GetFloat("_fuelLevel");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                OnTriggeredRefuel?.Invoke();
        }

        void FixedUpdate()
        {
            updateFuelLevel();
        }
        void updateFuelLevel()
        {
            ConsumeFuel(_fuelConsumptionRate * Time.deltaTime);
            if (CurrentFuel <= 0f)
                Debug.Log("Out of fuel");
        }

        void ConsumeFuel(float value)
        {
            _currentFuel -= value;
            _currentFuel = Mathf.Clamp(CurrentFuel, 0f, MaxFuel);
            OnFuelChanged?.Invoke(CurrentFuel / MaxFuel);
        }

        public void Refuel(float value)
        {
            _currentFuel += value;
            _currentFuel = Mathf.Clamp(CurrentFuel, 0f, MaxFuel);
            OnFuelChanged?.Invoke(CurrentFuel / MaxFuel);
        }
    }
}
