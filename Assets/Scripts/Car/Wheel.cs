using Dythervin.AutoAttach;
using Off_Road.Car.Enums;
using UnityEngine;

namespace Off_Road.Car
{
    public class Wheel : MonoBehaviour
    {
        [field: SerializeField] public WheelType WheelType { get; set; }
        [field: SerializeField] public Transform WheelTransform { get; set; }
        [field: SerializeField, Attach] public WheelCollider WheelCollider { get; set; }
    }
}