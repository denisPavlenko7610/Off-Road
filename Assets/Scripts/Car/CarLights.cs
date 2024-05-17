using UnityEngine;

namespace Off_Road.Car
{
    public class CarLights : MonoBehaviour
    {
        [SerializeField] Light[] _frontLights;

        [SerializeField] Renderer FrontGlass;
        [SerializeField] Renderer BackGlass;

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
            if (!Input.GetKeyDown(KeyCode.L))
                return;
            
            _lightsEnabled = !_lightsEnabled;
            ToggleLights(_frontLights);
        }

        void HandleLights()
        {
            bool isBraking = Input.GetAxis("Vertical") < 0;
            
            if (_lightsEnabled)
            {
                FrontGlass.material.EnableKeyword("_EMISSION");
                FrontGlass.material.SetColor("_EmissiveColor", Color.cyan * _intensityBackLights);
            }
            else
            {
                FrontGlass.material.DisableKeyword("_EMISSION");
                FrontGlass.material.SetColor("_EmissiveColor", Color.cyan * 0);
            }

            if (isBraking)
            {
                BackGlass.material.EnableKeyword("_EMISSION");
                BackGlass.material.SetColor("_EmissiveColor", Color.red * _intensityBreakingLights);
                return;
            }
            
            if (_lightsEnabled)
            {
                BackGlass.material.EnableKeyword("_EMISSION");
                BackGlass.material.SetColor("_EmissiveColor", Color.red * _intensityBackLights);
            }
            else
            {
                BackGlass.material.DisableKeyword("_EMISSION");
                BackGlass.material.SetColor("_EmissiveColor", Color.red * 0);
            }
        }

        void ToggleLights(Light[] lights)
        {
            foreach (Light light in lights)
            {
                light.enabled = _lightsEnabled;
            }
        }
    }
}
