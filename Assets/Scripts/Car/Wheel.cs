using RDTools.AutoAttach;
using UnityEngine;
using static Off_Road.Car.Car;

namespace Off_Road.Car
{
    public class Wheel : MonoBehaviour
    {
        [field: SerializeField] public WheelType WheelType { get; private set; }
        [field: SerializeField] public Transform WheelTransform { get; private set; }
        [field: SerializeField, Attach] public WheelCollider WheelCollider { get; private set; }
    }
}