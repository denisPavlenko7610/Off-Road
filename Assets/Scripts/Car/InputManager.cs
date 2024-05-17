using System;
using UnityEngine;

namespace Off_Road
{
    public class InputManager : MonoBehaviour
    {
        public event Action<float, float> OnGetInput;
        public event Action OnGetGearInputShiftUp;
        public event Action OnGetGearInputShiftDown;
        public event Action<bool> OnClutch;

        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = -Input.GetAxis("Vertical");

            OnGetInput?.Invoke(horizontalInput, verticalInput);

            if (Input.GetKeyDown(KeyCode.LeftShift))
                OnGetGearInputShiftUp?.Invoke();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                OnGetGearInputShiftDown?.Invoke();

            bool isClutchPressed = Input.GetKey(KeyCode.RightShift);
            OnClutch?.Invoke(isClutchPressed);
        }
    }
}
