using UnityEngine;

namespace Off_Road
{
    public class CarLights : MonoBehaviour
    {
        [SerializeField] Light[] _frontLights;
        [SerializeField] Light[] _backLights;

        [SerializeField] Renderer FrontGlass;
        [SerializeField] Renderer BackGlass;

        [SerializeField] float _intensityBreakingLights = 10f;
        [SerializeField] float _intensityBackLights = 5f;

        bool _lightsEnabled;
        Color _baseColor;
        
        const string EMISSION = "_EMISSION";

        void Start()
        {
            _baseColor = BackGlass.material.GetColor("_EmissionColor");
        }

        void Update()
        {
            HandleLightToggle();
            HandleBrakingLights();
        }

        void HandleLightToggle()
        {
            if (!Input.GetKeyDown(KeyCode.L))
                return;
            
            _lightsEnabled = !_lightsEnabled;
            ToggleLights(_frontLights);
            ToggleLights(_backLights);
            ToggleEmission(FrontGlass, _lightsEnabled);
            ToggleEmission(BackGlass, _lightsEnabled);
        }

        void HandleBrakingLights()
        {
            bool isBraking = Input.GetAxis("Vertical") < 0;
            foreach (Light light in _backLights)
            {
                light.enabled = isBraking || _lightsEnabled;
                light.intensity = isBraking 
                    ? _intensityBreakingLights 
                    : _intensityBackLights;
            }

            BackGlass.material.SetColor("_EmissionColor", isBraking 
                ? Color.red 
                : _baseColor);
            
            ToggleEmission(BackGlass, isBraking || _lightsEnabled);
        }

        void ToggleLights(Light[] lights)
        {
            foreach (Light light in lights)
            {
                light.enabled = _lightsEnabled;
            }
        }

        void ToggleEmission(Renderer renderer, bool state)
        {
            if (state)
            {
                renderer.material.EnableKeyword(EMISSION);
            }
            else
            {
                renderer.material.DisableKeyword(EMISSION);
            }
        }
    }
}
