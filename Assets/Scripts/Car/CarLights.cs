using UnityEngine;

namespace Off_Road.Car
{
    public class CarLights : MonoBehaviour
    {
        [SerializeField] Light[] _frontLights;

        [SerializeField] Renderer _frontGlass;
        [SerializeField] Renderer _backGlass;

        [SerializeField] float _intensityBreakingLights = 50f;
        [SerializeField] float _intensityBackLights = 20f;

        bool _lightsEnabled;

        void Update()
        {
            HandleLightToggle();
            HandleLights();
        }

        void HandleLightToggle()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _lightsEnabled = !_lightsEnabled;
                ToggleLights(_frontLights, _lightsEnabled);
            }
        }

        void HandleLights()
        {
            bool isBraking = Input.GetAxis("Vertical") < 0;

            if (isBraking)
                SetEmission(_backGlass, true, Color.red, _intensityBreakingLights);
            else
                SetEmission(_backGlass, _lightsEnabled, Color.red, _intensityBackLights);

        }

        void ToggleLights(Light[] lights, bool state)
        {
            foreach (Light light in lights)
                light.enabled = state;
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
