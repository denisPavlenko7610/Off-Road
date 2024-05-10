using UnityEngine;

namespace Off_Road
{
    public class GasStationRefuel : MonoBehaviour
    {
        [SerializeField] private CarTank _carTank;
        [SerializeField] float _fuelRefuelRate = 2f;

        private void Start()
        {
            _carTank = FindObjectOfType<CarTank>();
        }

        private void OnTriggerStay(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                if (Input.GetKey(KeyCode.F))
                {
                    _carTank.Refuel(_fuelRefuelRate * Time.deltaTime);
                    Debug.Log("Refuel");
                }
            }
        }
    }
}
