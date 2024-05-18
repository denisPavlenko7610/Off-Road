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

        void Update()
        {
            GetInput();
        }

        void GetInput()
        {
            bool isBraking = Input.GetAxis("Vertical") < 0;
            if (isBraking)
                OnPressBrake?.Invoke();
            else
                OnUnpressBrake?.Invoke();

            if (Input.GetKeyDown(KeyCode.L))
                OnSetStateLights?.Invoke();

            if (Input.GetKeyDown(KeyCode.LeftShift))
                OnGetGearInputShiftUp?.Invoke();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                OnGetGearInputShiftDown?.Invoke();

            bool isClutchPressed = Input.GetKey(KeyCode.RightShift);
            OnClutch?.Invoke(isClutchPressed);

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = -Input.GetAxis("Vertical");

            if (Mathf.Approximately(horizontalInput, 0)
                && Mathf.Approximately(verticalInput, 0))
                return;

            OnGetInput?.Invoke(horizontalInput, verticalInput);
        }
    }
}
