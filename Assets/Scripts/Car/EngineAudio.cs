using Off_Road.Car;
using System.Collections;
using UnityEngine;

namespace Off_Road
{
    public class EngineAudio : MonoBehaviour
    {
        [SerializeField] AudioSource _startEngineSound;
        [SerializeField] AudioSource _engineSound;
        [SerializeField] AudioSource _stopEngineSound;
        [SerializeField] AudioClip _startEngineClip;

        [SerializeField] CarController _carController;
        [SerializeField] CarInfoSO _carInfoSO;
        [SerializeField] EngineControl _engineControl;

        [SerializeField] float _lowPitchMin = 0.3f;
        [SerializeField] float _lowPitchMax = 1.5f;

        bool _isCoroutineRunning = false;
      
        void OnEnable()
        {
            _carController.OnRPMUpdate += UpdatePitch;
            _engineControl.OnSetEngineState += HandleEngineSoundStateChange;
        }

        void OnDisable()
        {
            _carController.OnRPMUpdate -= UpdatePitch;
            _engineControl.OnSetEngineState -= HandleEngineSoundStateChange;
        }

        void UpdatePitch() => _engineSound.pitch = Mathf.Lerp(_lowPitchMin, _lowPitchMax, (_carController.RPMEngine - _carInfoSO.IdleRPM) / _carInfoSO.RedLine);

        void HandleEngineSoundStateChange(bool isRunning)
        {
            if (isRunning && !_isCoroutineRunning)
                StartCoroutine(StartEngineQueue());
            else
                StopEngineSound();
        }

        IEnumerator StartEngineQueue()
        {
            _isCoroutineRunning = true;
            _startEngineSound.Play();

            yield return new WaitForSeconds(_startEngineClip.length);

            _engineSound.Play();
            _isCoroutineRunning = false;
        }

        void StopEngineSound()
        {
            if (_engineSound.isPlaying)
                _engineSound.Stop();

            _stopEngineSound.Play();
        }
    }
}

