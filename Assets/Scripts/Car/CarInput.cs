using System;
using UnityEngine;

namespace Off_Road
{
    public class CarInput : MonoBehaviour
    {
        public event Action<float, float> OnGetInput;
        public event Action OnGetGearInputShiftUp;
        public event Action OnGetGearInputShiftDown;
        public event Action<bool> OnClutch;
        public event Action OnSetEngineState;

        void Update()
        {
            GetInput();
        }
        void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.E))
                OnSetEngineState?.Invoke();

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
