using System;
using UnityEngine;

namespace Off_Road.Car
{
    public class CarTank : MonoBehaviour
    {
        [field: SerializeField]
        public float MaxFuel { get; set; } = 40f;

        [field: SerializeField]
        public float CurrentFuel { get; set; }
        
        [field: SerializeField]
        public float FuelConsumptionRate { get; set; } = 0.1f;

        public Action<float> OnFuelChanged;
        public Action OnTriggeredRefuel;

        void Start()
        {
            if (PlayerPrefs.HasKey(CarConstants.FUEL_LEVEL_KEY))
            {
                CurrentFuel = PlayerPrefs.GetFloat(CarConstants.FUEL_LEVEL_KEY);
            }
            else
            {
                CurrentFuel = MaxFuel;
            }
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
            ConsumeFuel(FuelConsumptionRate * Time.deltaTime);
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

        void OnApplicationQuit()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                PlayerPrefs.SetFloat(CarConstants.FUEL_LEVEL_KEY, CurrentFuel);
            }
        }
    }
}

