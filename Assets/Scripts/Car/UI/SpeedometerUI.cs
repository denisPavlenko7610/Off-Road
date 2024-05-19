using Off_Road.Car;
using RDTools.AutoAttach;
using TMPro;
using UnityEngine;

namespace Off_Road
{
    public class SpeedometerUI : MonoBehaviour
    {
        [SerializeField, Attach(Attach.Scene)] CarController _carController;
        [SerializeField] CarInfoSO _carInfoSO;

        [SerializeField] float _minRpmArrowAngle;
        [SerializeField] float _maxRpmArrowAngle;

        [SerializeField] TextMeshProUGUI _textSpeed;
        [SerializeField] TextMeshProUGUI _textGear;

        [SerializeField] RectTransform _arrow;

        public CarInfoSO CarInfoSO { get => _carInfoSO; }

        void OnEnable()
        {
            SubscribeToCarControllerEvents();
        }

        void OnDisable()
        {
            UnsubscribeFromCarControllerEvents();
        }

        void SubscribeToCarControllerEvents()
        {
            _carController.OnRPMUpdate += UpdateRPM;
            _carController.OnSpeedUpdate += UpdateSpeedometer;
            _carController.OnGearUpdate += UpdateGearUI;
        }

        void UnsubscribeFromCarControllerEvents()
        {
            _carController.OnRPMUpdate -= UpdateRPM;
            _carController.OnSpeedUpdate -= UpdateSpeedometer;
            _carController.OnGearUpdate -= UpdateGearUI;
        }

        void UpdateRPM()
        {
            if (_arrow == null)
                return;

            float rpmRatio = _carController.RPMEngine / CarInfoSO.RedLine;
            float arrowAngle = Mathf.Lerp(_minRpmArrowAngle, _maxRpmArrowAngle, rpmRatio);
            _arrow.localEulerAngles = new Vector3(0, 0, arrowAngle);
        }

        void UpdateSpeedometer()
        {
            if (_textSpeed == null)
                return;

            _textSpeed.text = $"{(int)_carController.SpeedAuto}\n km/h";
        }

        void UpdateGearUI() => _textGear.text = _carController.CurrentGear.ToString();
    }
}