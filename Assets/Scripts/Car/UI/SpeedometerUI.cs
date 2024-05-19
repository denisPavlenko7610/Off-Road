using Off_Road.Car;
using TMPro;
using UnityEngine;

namespace Off_Road
{
    public class SpeedometerUI : MonoBehaviour
    {
        [SerializeField] CarController _carController;
        [field: SerializeField] public CarInfoSO CarInfoSO { get; private set; }

        [SerializeField] float _minRpmArrowAngle;
        [SerializeField] float _maxRpmArrowAngle;

        [SerializeField] TextMeshProUGUI _textSpeed;
        [SerializeField] TextMeshProUGUI _textGear;

        [SerializeField] RectTransform _arrow;

        private void OnEnable()
        {
            _carController.OnRPMUpdate += UpdateRPM;
            _carController.OnSpeedUpdate+= UpdateSpeedometer;
            _carController.OnGearUpdate += UpdateGearUI;
        }

        private void OnDisable()
        {
            _carController.OnRPMUpdate -= UpdateRPM;
            _carController.OnSpeedUpdate -= UpdateSpeedometer;
            _carController.OnGearUpdate -= UpdateGearUI;
        }

        void UpdateRPM()
        {
            if (_arrow != null)
                _arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(_minRpmArrowAngle, _maxRpmArrowAngle, _carController.RPMEngine / CarInfoSO.RedLine));
        }
        
        void UpdateSpeedometer()
        {
            if (_textSpeed != null)
                _textSpeed.text = ((int)_carController.SpeedAuto) + " km/h";
        }
        
        void UpdateGearUI()
        {
            _textGear.text = _carController.CurrentGear.ToString();
        }
    }
}
