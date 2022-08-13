using Dythervin.AutoAttach;
using Enums;
using UnityEngine;

namespace Off_Road
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] private DriveUnit driveUnit;
        [SerializeField, Attach(Attach.Child)] private Wheel[] Wheels;

        [SerializeField] private float maxSteerAngle = 30f;
        [SerializeField] private float motorForce = 50f;

        private float horizontalInput;
        private float verticalInput;
        private float steeringAngle;


        private void Update()
        {
            GetInput();
        }

        private void FixedUpdate()
        {
            Steer();
            Accelerate();
            UpdateWheelPoses();
        }

        public void GetInput()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        private void Steer()
        {
            steeringAngle = maxSteerAngle * horizontalInput;
            foreach (var wheel in Wheels)
            {
                if (!IsFrontWheel(wheel))
                    continue;

                wheel.WheelCollider.steerAngle = steeringAngle;
            }
        }

        private bool IsFrontWheel(Wheel wheel)
            => wheel.WheelType == WheelType.FL
               || wheel.WheelType == WheelType.FR;

        private void Accelerate()
        {
            if (driveUnit == DriveUnit.Rear)
            {
                AccelerateRearWheels();
            }
            else if (driveUnit == DriveUnit.FrontWheel)
            {
                AccelerateFrontWheels();
            }
            else
            {
                AccelerateWheels();
            }
        }

        private void AccelerateWheels()
        {
            foreach (var wheel in Wheels)
            {
                wheel.WheelCollider.motorTorque = -verticalInput * motorForce;
            }
        }

        private void AccelerateFrontWheels()
        {
            foreach (var wheel in Wheels)
            {
                if (!IsFrontWheel(wheel))
                    continue;

                wheel.WheelCollider.motorTorque = -verticalInput * motorForce;
            }
        }

        private void AccelerateRearWheels()
        {
            foreach (var wheel in Wheels)
            {
                if (IsFrontWheel(wheel))
                    continue;

                wheel.WheelCollider.motorTorque = -verticalInput * motorForce;
            }
        }

        private void UpdateWheelPoses()
        {
            UpdateWheelPose(Wheels);
            UpdateWheelPose(Wheels);
        }

        private void UpdateWheelPose(Wheel[] wheels)
        {
            foreach (var wheel in wheels)
            {
                var wheelTransform = wheel.WheelTransform.transform;
                var position = wheelTransform.position;
                var rotation = wheelTransform.rotation;

                wheel.WheelCollider.GetWorldPose(out position, out rotation);

                wheelTransform.position = position;
                wheelTransform.rotation = rotation;

                if (wheel.WheelType == WheelType.FL || wheel.WheelType == WheelType.RL)
                {
                    wheelTransform.Rotate(Vector3.up,180);
                }
            }
        }
    }
}