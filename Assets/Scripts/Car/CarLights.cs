using RDTools.AutoAttach;
using UnityEngine;

namespace Off_Road.Car
{
    public class CarLights : MonoBehaviour
    {
        [SerializeField] SpeedometerUI _speedometerUI;
        [SerializeField, Attach] CarInput _carInput;
        [SerializeField] Light[] _frontLights;

        [SerializeField] Renderer _frontGlass;
        [SerializeField] Renderer _backGlass;

        [SerializeField] float _intensityBreakingLights = 50f;
        [SerializeField] float _intensityBackLights = 20f;

        bool _lightsEnabled;

        void OnEnable()
        {
            _carInput.OnSetStateLights += SetStateLights;
            _carInput.OnPressBrake += BrakeLightsOn;
            _carInput.OnUnpressBrake += BrakeLightsOff;
        }

        void OnDisable()
        {
            _carInput.OnSetStateLights -= SetStateLights;
            _carInput.OnPressBrake -= BrakeLightsOn;
            _carInput.OnUnpressBrake -= BrakeLightsOff;
        }

        void SetStateLights()
        {
            _lightsEnabled = !_lightsEnabled;
            ToggleLights(_frontLights, _lightsEnabled);
            SetEmission(_frontGlass, _lightsEnabled, Color.cyan, _intensityBackLights);
        }

        void BrakeLightsOn()
        {
            SetEmission(_backGlass, true, Color.red, _intensityBreakingLights);
        }

        void BrakeLightsOff()
        {
            SetEmission(_backGlass, _lightsEnabled, Color.red, _intensityBackLights);
        }

        void ToggleLights(Light[] lights, bool state)
        {
            foreach (Light light in lights)
                light.enabled = state;
            if (state)
                _speedometerUI.SetHeadlightOn();
            else
                _speedometerUI.SetHeadlightOff();
        }

        void SetEmission(Renderer glass, bool enableEmission, Color color, float intensity)
        {
            if (enableEmission)
            {
                glass.material.EnableKeyword(ShaderConstants.EmissionKeyword);
                glass.material.SetColor(ShaderConstants.EmissiveColor, color * intensity);
            }
            else
            {
                glass.material.DisableKeyword(ShaderConstants.EmissionKeyword);
                glass.material.SetColor(ShaderConstants.EmissiveColor, color * 0);
            }
        }
    }
}
