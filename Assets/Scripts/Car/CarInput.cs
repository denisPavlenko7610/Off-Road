using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        public event Action OnTriggeredRefuel;

        CarInputSystem _carInputSystem;

        Vector2 _moveVector;

        void Awake()
        {
            _carInputSystem = new CarInputSystem();


            //!!!!!!!! _carInputSystem.Car.Move.performed += context => MovementInput();
        }

        void OnEnable()
        {
            _carInputSystem.Enable();

            _carInputSystem.Car.SetStateEngine.performed += context => EngineState();
            _carInputSystem.Car.SetStateLights.performed += context => StateLights();
            _carInputSystem.Car.DownGear.performed += context => DownGear();
            _carInputSystem.Car.UpGear.performed += context => UpGear();
            _carInputSystem.Car.RefuelCar.performed += context => Refuel();
            //_carInputSystem.Car.Move.performed += MovementInput;
        }

        void OnDisable()
        {
            _carInputSystem.Disable();

            _carInputSystem.Car.SetStateEngine.performed += context => EngineState();
            _carInputSystem.Car.SetStateLights.performed += context => StateLights();
            _carInputSystem.Car.DownGear.performed += context => DownGear();
            _carInputSystem.Car.UpGear.performed += context => UpGear();
            _carInputSystem.Car.RefuelCar.performed += context => Refuel();
        }

        void Refuel()
        {
            OnTriggeredRefuel?.Invoke();
        }

        void Update()
        {
            GetInput();
        }

        void GetInput()
        {
            //EngineState();
            MovementInput();
            //StateLights();
            //DownGear();
            //UpGear();
            // ClutchInput();
        }

        void EngineState()
        {
            //if (Input.GetKeyDown(KeyCode.E))
            OnSetEngineState?.Invoke();
        }

        public void MovementInput()
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
            //if (Input.GetKeyDown(KeyCode.L))
            OnSetStateLights?.Invoke();
        }

        void UpGear()
        {
            //if (Input.GetKeyDown(KeyCode.LeftShift))
            OnGetGearInputShiftUp?.Invoke();
            ClutchInput();
        }

        void DownGear()
        {
            // if (Input.GetKeyDown(KeyCode.LeftControl))
            OnGetGearInputShiftDown?.Invoke();
        }

        void ClutchInput()
        {
            bool isClutchPressed = Input.GetKey(KeyCode.RightShift);
            OnClutch?.Invoke(isClutchPressed);
        }
    }
}
