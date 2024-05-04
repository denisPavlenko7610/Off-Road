using UnityEngine;

namespace Off_Road
{
    public class CarLights : MonoBehaviour
    {
        [SerializeField] private Light[] _frontLights;
        [SerializeField] private Light[] _backLights;

        [SerializeField] private Renderer FrontGlass;
        [SerializeField] private Renderer BackGlass;

        [SerializeField] private float _intensityBreakingLights = 10f;
        [SerializeField] private float _intensityBackLights = 5f;

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
                for (int i = 0; i < _backLights.Length; i++)
                {
                    _backLights[i].enabled = true;
                    _backLights[i].intensity = _intensityBreakingLights;
                }

                BackGlass.material.SetColor("_EmissionColor", Color.red);
                BackGlass.material.EnableKeyword("_EMISSION");
            }
            else
            {
                for (int i = 0; i < _backLights.Length; i++)
                {
                    _backLights[i].intensity = _intensityBackLights;
                }

                BackGlass.material.SetColor("_EmissionColor", _baseColor);

                if (!_lightsEnabled)
                {
                    for (int i = 0; i < _backLights.Length; i++)
                    {
                        _backLights[i].enabled = false;
                    }

                    BackGlass.material.DisableKeyword("_EMISSION");
                }

            }
        }

        private void TurnLights()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _lightsEnabled = !_lightsEnabled;

                for (int i = 0; i < _frontLights.Length; i++)
                {
                    _frontLights[i].enabled = _lightsEnabled;
                }

                for (int i = 0; i < _backLights.Length; i++)
                {
                    _backLights[i].enabled = _lightsEnabled;
                }

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
