using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Off_Road
{
    public class DayCycleController : MonoBehaviour
    {
        [Range(0, 24)]
        [SerializeField] float _timeOfDay;
        [SerializeField] float _orbitSpeed = 1.0f;
        [SerializeField] float _starsIntensity = 3f;

        [SerializeField] Light _sun;
        [SerializeField] Light _moon;

        [SerializeField] Volume _skyVolume;

        [SerializeField] AnimationCurve _starsCurve;

        PhysicallyBasedSky _sky;
        bool _isNight;

        private void Start() => _skyVolume.profile.TryGet(out _sky);

        void Update()
        {
            _timeOfDay += Time.deltaTime * _orbitSpeed;
            if (_timeOfDay > 24)
                _timeOfDay = 0;

            UpdateTime();
        }

        void UpdateTime()
        {
            float alpha = _timeOfDay / 24.0f;
            float sunRotation = Mathf.Lerp(-90f, 270, alpha);
            float moonRotation = sunRotation - 180f;

            _sun.transform.rotation = Quaternion.Euler(sunRotation, -150.0f, 0);
            _moon.transform.rotation = Quaternion.Euler(moonRotation, -150.0f, 0);

            _sky.spaceEmissionMultiplier.value = _starsCurve.Evaluate(alpha) * _starsIntensity;

            CheckNightDatTransition();
        }

        void CheckNightDatTransition()
        {
            if (_isNight)
            {
                if (_moon.transform.rotation.eulerAngles.x > 180)
                    StartDay();
            }
            else
            {
                if (_sun.transform.rotation.eulerAngles.x > 180)
                    StartNight();
            }
        }

        void StartDay()
        {
            _isNight = false;
            _sun.shadows = LightShadows.Soft;
            _moon.shadows = LightShadows.None;
        }

        void StartNight()
        {
            _isNight = true;
            _sun.shadows = LightShadows.None;
            _moon.shadows = LightShadows.Soft;
        }
    }
}
