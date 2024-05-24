﻿using Off_Road.Car;
using RDTools.AutoAttach;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Off_Road
{
    public class SpeedometerUI : MonoBehaviour
    {
        private static readonly Color _darkGrayColor = new Color(0.2f, 0.2f, 0.2f);
        private static readonly Color _orange = new Color(1f, 0.3f, 0f);

        [SerializeField, Attach(Attach.Scene)] CarController _carController;
        [SerializeField] CarInfoSO _carInfoSO;

        [SerializeField] float _minRpmArrowAngle;
        [SerializeField] float _maxRpmArrowAngle;
        [SerializeField] float _correctionLevelForRPMIndicator = 500f;

        [SerializeField] TextMeshProUGUI _textSpeed;
        [SerializeField] TextMeshProUGUI _textGear;

        [SerializeField] RectTransform _arrow;

        [SerializeField] Image _engineStateIndicator;
        [SerializeField] Image _headlightIndicator;
        [SerializeField] Image _engineRPMIndicator;

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

            if (_carController.RPMEngine > _carInfoSO.RedLine - _correctionLevelForRPMIndicator)
                _engineRPMIndicator.color = _orange;
            else
                _engineRPMIndicator.color = _darkGrayColor;

        }

        void UpdateSpeedometer()
        {
            if (_textSpeed == null)
                return;

            _textSpeed.text = $"{(int)_carController.SpeedAuto}\n km/h";
        }

        void UpdateGearUI() => _textGear.text = _carController.CurrentGear.ToString();

        public void SetEngineStartIndicator() => _engineStateIndicator.color = _darkGrayColor;

        public void SetEngineStopIndicator() => _engineStateIndicator.color = Color.red;

        public void SetHeadlightOn() => _headlightIndicator.color = Color.green;

        public void SetHeadlightOff() => _headlightIndicator.color = _darkGrayColor;




    }
}