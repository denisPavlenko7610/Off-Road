using Dythervin.AutoAttach;
using Enums;
using UnityEngine;

namespace Off_Road
{
    public class Wheel : MonoBehaviour
    {
        [field: SerializeField] public WheelType WheelType { get; set; }
        [field: SerializeField] public Transform WheelTransform { get; set; }
        [field: SerializeField, Attach] public WheelCollider WheelCollider { get; set; }
    }
}