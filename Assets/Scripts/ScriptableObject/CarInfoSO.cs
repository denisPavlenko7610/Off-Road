using UnityEngine;

namespace Off_Road
{
    [CreateAssetMenu(fileName = "CarInfo", menuName = "Car/CreateCar")]
    public class CarInfoSO : ScriptableObject
    {
        public float MaxSteerAngle;

        public float MotorForce;

        public float BrakeForce;
        
        public float IdleRPM;

        public float RedLine;
    }
}
