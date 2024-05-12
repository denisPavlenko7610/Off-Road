using System.Collections;
using UnityEngine;

namespace Off_Road
{
    public class GasStationRefuel : MonoBehaviour
    {
        [SerializeField] float _timeToRefuel = 2f;
        [SerializeField] CarTank _carTank;

        bool _isTanking;

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarTank _Tank))
            {
                _carTank = _Tank;
                _carTank.OnTriggeredRefuel += StartCoroutineRefuel;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_carTank != null)
                _carTank.OnTriggeredRefuel -= StartCoroutineRefuel;
        }

        IEnumerator Refuel()
        {
            _isTanking = true;

            float refuelValuePerSecond = _carTank.MaxFuel / 2f;

            for (float time = 0; time <= _timeToRefuel; time += Time.deltaTime)
            {
                _carTank.Refuel(refuelValuePerSecond * Time.deltaTime);
                yield return null;
            }
            _isTanking = false;
            Debug.Log("Refuel is completed!");
        }

        void StartCoroutineRefuel()
        {
            if (!_isTanking)
                StartCoroutine(Refuel());

        }
    }
}
