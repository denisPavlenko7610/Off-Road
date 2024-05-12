using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Off_Road
{
    public class Saver : MonoBehaviour
    {
        [SerializeField] CarTank _carTank;

        private void Start()
        {
            _carTank = FindObjectOfType<CarTank>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                PlayerPrefs.SetFloat("_fuelLevel", _carTank.CurrentFuel);
            }
        }
    }
}
