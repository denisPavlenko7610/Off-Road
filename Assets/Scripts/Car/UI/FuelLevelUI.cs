using UnityEngine;
using UnityEngine.UI;

namespace Off_Road
{
    public class FuelLevelUI : MonoBehaviour
    {
        [SerializeField] Image _fuelLevel;
        [SerializeField] CarTank _carTank;

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
