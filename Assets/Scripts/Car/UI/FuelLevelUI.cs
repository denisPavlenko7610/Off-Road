using RDTools.AutoAttach;
using UnityEngine;
using UnityEngine.UI;

namespace Off_Road.Car.UI
{
    public class FuelLevelUI : MonoBehaviour
    {
        [SerializeField] Image _fuelLevel;
        [SerializeField, Attach] CarTank _carTank;

        void OnEnable()
        {
            _carTank.OnFuelChanged += UpdateFuelUI;
        }

        void OnDisable()
        {
            _carTank.OnFuelChanged -= UpdateFuelUI;
        }

        void UpdateFuelUI(float fuelValue)
        {
            _fuelLevel.fillAmount = fuelValue;
        }
    }
}
