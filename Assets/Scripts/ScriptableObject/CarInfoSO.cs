using UnityEngine;

namespace Off_Road
{
    [CreateAssetMenu(fileName = "CarInfo", menuName = "Car/CreateCar")]
    public class CarInfoSO : ScriptableObject
    {
        public float MaxSteerAngle = 30;
        public float MotorForce = 100;
        public float BrakeForce = 50_000;
        public float IdleRPM = 800;
        public float RedLine = 6500;
        public float IdleRPMLimit = 200;
        public int RandomAdditionalRPM = 50;
        public float RotateAngle = 180;
        public float DifferentialRatio = 4f;
        public float DecreaseRPMIntensity = 2000f;

        public float[] MaxSpeedPerGear;
        public AnimationCurve HpToRPMCurve;
        public float[] GearRatios;
    }
}