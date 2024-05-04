using UnityEngine;
using UnityEngine.UI;

namespace Off_Road
{
    public class FuelLevelUI : MonoBehaviour
    {
        [SerializeField] private Image _fuelLevel;
        [SerializeField] private CarTank _carTank;

        void Update()
        {
            _fuelLevel.fillAmount = _carTank.CurrentFuel/_carTank.MaxFuel;
        }
    }


}
