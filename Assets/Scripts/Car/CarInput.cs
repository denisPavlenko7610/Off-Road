using System;
using UnityEngine;

namespace Off_Road
{
    public class CarInput : MonoBehaviour
    {
        public event Action<float, float> OnGetInput;

        public event Action OnGetGearInputShiftUp;
        public event Action OnGetGearInputShiftDown;
        public event Action OnSetStateLights;
        public event Action OnPressBrake;
        public event Action OnUnpressBrake;

        public event Action<bool> OnClutch;
        public event Action OnSetEngineState;

        void Update()
        {
            GetInput();
        }

        void GetInput()
        {
            EngineState();
            MovementInput();
            StateLights();
            GearInput();
            ClutchInput();
        }

        void EngineState()
        {
            if (Input.GetKeyDown(KeyCode.E))
                OnSetEngineState?.Invoke();
        }

        void MovementInput()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = -Input.GetAxisRaw("Vertical");
            
            OnGetInput?.Invoke(horizontalInput, verticalInput);

            bool isBraking = verticalInput > 0;
            if (isBraking)
                OnPressBrake?.Invoke();
            else
                OnUnpressBrake?.Invoke();
        }

        void StateLights()
        {
            if (Input.GetKeyDown(KeyCode.L))
                OnSetStateLights?.Invoke();
        }

        void GearInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                OnGetGearInputShiftUp?.Invoke();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                OnGetGearInputShiftDown?.Invoke();
        }

        void ClutchInput()
        {
            bool isClutchPressed = Input.GetKey(KeyCode.RightShift);
            OnClutch?.Invoke(isClutchPressed);
        }
    }
}
