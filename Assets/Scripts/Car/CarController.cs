using Dythervin.AutoAttach;
using Enums;
using UnityEngine;

namespace Off_Road
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] DriveUnit driveUnit;
        [SerializeField, Attach(Attach.Child)] Wheel[] Wheels;

        [SerializeField] float maxSteerAngle = 30f;
        [SerializeField] float motorForce = 50f;

        float horizontalInput;
        float verticalInput;
        float steeringAngle;


        void Update()
        {
            GetInput();
        }

        void FixedUpdate()
        {
            Steer();
            Accelerate();
            UpdateWheelPoses();
        }

        void GetInput()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        void Steer()
        {
            steeringAngle = maxSteerAngle * horizontalInput;
            foreach (var wheel in Wheels)
            {
                if (!IsFrontWheel(wheel))
                    continue;

                wheel.WheelCollider.steerAngle = steeringAngle;
            }
        }

        bool IsFrontWheel(Wheel wheel)
            => wheel.WheelType == WheelType.FL
                || wheel.WheelType == WheelType.FR;

        void Accelerate()
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

        void AccelerateWheels()
        {
            foreach (var wheel in Wheels)
            {
                wheel.WheelCollider.motorTorque = -verticalInput * motorForce;
            }
        }

        void AccelerateFrontWheels()
        {
            foreach (var wheel in Wheels)
            {
                if (!IsFrontWheel(wheel))
                    continue;

                wheel.WheelCollider.motorTorque = -verticalInput * motorForce;
            }
        }

        void AccelerateRearWheels()
        {
            foreach (var wheel in Wheels)
            {
                if (IsFrontWheel(wheel))
                    continue;

                wheel.WheelCollider.motorTorque = -verticalInput * motorForce;
            }
        }

        void UpdateWheelPoses()
        {
            UpdateWheelPose(Wheels);
            UpdateWheelPose(Wheels);
        }

        void UpdateWheelPose(Wheel[] wheels)
        {
            foreach (var wheel in wheels)
            {
                Transform wheelTransform = wheel.WheelTransform.transform;
                Vector3 position = wheelTransform.position;
                Quaternion rotation = wheelTransform.rotation;

                wheel.WheelCollider.GetWorldPose(out position, out rotation);

                wheelTransform.position = position;
                wheelTransform.rotation = rotation;

                if (wheel.WheelType == WheelType.FL || wheel.WheelType == WheelType.RL)
                {
                    wheelTransform.Rotate(Vector3.up, 180);
                }
            }
        }
    }
}