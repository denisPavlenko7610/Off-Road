using UnityEngine;

namespace Off_Road
{
    [CreateAssetMenu(fileName = "CarInfo", menuName = "Car/CreateCar")]
    public class CarInfoScriptableObject : ScriptableObject
    {
        [SerializeField] public float maxSteerAngle;

        [field: SerializeField]
        public float MotorForce { get; set; }

        [SerializeField] public float brakeForce;
        
        [SerializeField] public float _idleRPM;

        [SerializeField] public float _redLine;
    }
}
