using UnityEngine;

namespace Off_Road.Car
{
    public class Car : MonoBehaviour
    {
        public enum WheelType
        {
            FL,
            FR,
            RL,
            RR
        }
        
        public enum GearState
        {
            Neutral,
            Running,
            CheckingChange,
            Changing
        };
    
        public enum DriveUnit
        {
            FrontWheel,
            Rear,
            FourWheel
        }
    }
}