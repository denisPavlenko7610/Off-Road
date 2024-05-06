using UnityEngine;
using UnityEngine.UI;

namespace Off_Road
{
    public class FuelLevelUI : MonoBehaviour
    {
        [SerializeField] private Image _fuelLevel;
        [SerializeField] private CarTank _carTank;

        private void OnEnable()
        {
            _carTank.OnFuelChanged += UpdateFuelUI;
        }

        private void OnDisable()
        {
            _carTank.OnFuelChanged -= UpdateFuelUI;
        }

        void UpdateFuelUI(float fuelValue)
        {
            _fuelLevel.fillAmount = fuelValue;
        }

    }


}
