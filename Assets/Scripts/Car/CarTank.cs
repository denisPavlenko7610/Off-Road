using System;
using UnityEngine;

namespace Off_Road.Car
{
    public class CarTank : MonoBehaviour
    {
        [field:SerializeField]
        public float MaxFuel { get; set; } = 100f;
        
        [field:SerializeField]
        public float CurrentFuel { get; set; }
        
        [SerializeField] float _fuelConsumptionRate = 1f;

        public Action<float> OnFuelChanged;
        public Action OnTriggeredRefuel;

        void Start()
        {
            CurrentFuel = MaxFuel;
            CurrentFuel = PlayerPrefs.GetFloat("_fuelLevel");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                OnTriggeredRefuel?.Invoke();
        }

        void FixedUpdate()
        {
            UpdateFuelLevel();
        }
        void UpdateFuelLevel()
        {
            ConsumeFuel(_fuelConsumptionRate * Time.deltaTime);
            if (CurrentFuel <= 0f)
                Debug.Log("Out of fuel");
        }

        void ConsumeFuel(float value)
        {
            CurrentFuel -= value;
            CurrentFuel = Mathf.Clamp(CurrentFuel, 0f, MaxFuel);
            OnFuelChanged?.Invoke(CurrentFuel / MaxFuel);
        }

        public void Refuel(float value)
        {
            CurrentFuel += value;
            CurrentFuel = Mathf.Clamp(CurrentFuel, 0f, MaxFuel);
            OnFuelChanged?.Invoke(CurrentFuel / MaxFuel);
        }
    }
}
