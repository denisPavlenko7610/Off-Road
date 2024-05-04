using UnityEngine;

namespace Off_Road
{
    public class CarLights : MonoBehaviour
    {
        [SerializeField] private Light _lightFrontRight;
        [SerializeField] private Light _lightFrontLeft;
        [SerializeField] private Light _lightBackRight;
        [SerializeField] private Light _lightBackLeft;

        [SerializeField] private Renderer FrontGlass;
        [SerializeField] private Renderer BackGlass;

        private bool _lightsEnabled = false;
        private Color _baseColor;

        private void Start()
        {
            _baseColor = BackGlass.material.GetColor("_EmissionColor");
        }

        private void Update()
        {
            TurnLights();
            BrakingLights();
        }

        private void BrakingLights()
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                _lightBackRight.enabled = true;
                _lightBackLeft.enabled = true;
                _lightBackRight.intensity = 10f;
                _lightBackLeft.intensity = 10f;
                BackGlass.material.SetColor("_EmissionColor", Color.red);
                BackGlass.material.EnableKeyword("_EMISSION");
            }
            else
            {
                _lightBackRight.intensity = 5f;
                _lightBackLeft.intensity = 5f;
                BackGlass.material.SetColor("_EmissionColor", _baseColor);

                if (!_lightsEnabled)
                {
                    _lightBackRight.enabled = false;
                    _lightBackLeft.enabled = false;
                   
                    BackGlass.material.DisableKeyword("_EMISSION");
                }

            }
        }

        private void TurnLights()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _lightsEnabled = !_lightsEnabled;

                _lightFrontRight.enabled = _lightsEnabled;
                _lightFrontLeft.enabled = _lightsEnabled;
                _lightBackRight.enabled = _lightsEnabled;
                _lightBackLeft.enabled = _lightsEnabled;

                if (_lightsEnabled)
                {
                    BackGlass.material.EnableKeyword("_EMISSION");
                    FrontGlass.material.EnableKeyword("_EMISSION");

                }
                else
                {
                    BackGlass.material.DisableKeyword("_EMISSION");
                    FrontGlass.material.DisableKeyword("_EMISSION");
                }
            }
        }

    }
}
