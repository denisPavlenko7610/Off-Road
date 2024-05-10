using System;
using UnityEngine;

namespace Off_Road
{
    public class CarTank : MonoBehaviour
    {
        [field:SerializeField] float MaxFuel { get; } = 100f;
        [SerializeField] float _fuelConsumptionRate = 1f;
        
        float CurrentFuel { get; set; }

        void Start()
        {
            CurrentFuel = MaxFuel;
        }

        public Action <float> OnFuelChanged;

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
            CurrentFuel -= value;
            CurrentFuel = Mathf.Clamp(CurrentFuel, 0f, MaxFuel);
            OnFuelChanged?.Invoke(CurrentFuel/MaxFuel);
        }

        public void Refuel(float value)
        {
            CurrentFuel += value;
            CurrentFuel = Mathf.Clamp(CurrentFuel, 0f, MaxFuel);
            OnFuelChanged?.Invoke(CurrentFuel / MaxFuel);
        }
    }
}
