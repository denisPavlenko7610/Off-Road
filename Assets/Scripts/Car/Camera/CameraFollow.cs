﻿using Dythervin.AutoAttach;
using UnityEngine;

namespace Off_Road.Car.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField, Attach(Attach.Scene)] Car _car;
        [SerializeField, Attach] UnityEngine.Camera _camera;
        
        Rigidbody _carRigidbody;

        [SerializeField] float _distance = 8.3f;
        [SerializeField] float _height = 3.1f;
        [SerializeField] float _rotationDamping = 3.0f;
        [SerializeField] float _heightDamping = 2.0f;
        [SerializeField] float _zoomRatio = 0.5f;
        [SerializeField] float _defaultFOV = 60f;

        Vector3 rotationVector;
        float _forwardDegree = 180;

        void Awake()
        {
            _carRigidbody = _car.GetComponent<Rigidbody>();
        }
        
        void FixedUpdate()
        {
            UpdateCameraFOV();
        }
        
        void LateUpdate()
        {
            UpdateCameraRotation();
        }
        void UpdateCameraRotation()
        {
            float wantedAngle = rotationVector.y;
            float wantedHeight = _car.transform.position.y + _height;
            float myAngle = transform.eulerAngles.y;
            float myHeight = transform.position.y;

            myAngle = Mathf.LerpAngle(myAngle, wantedAngle, _rotationDamping*Time.deltaTime);
            myHeight = Mathf.Lerp(myHeight, wantedHeight, _heightDamping*Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);
            transform.position = _car.transform.position;
            transform.position -= currentRotation * Vector3.forward*_distance;
            Vector3 temp = transform.position;
            temp.y = myHeight;
            transform.position = temp;
            transform.LookAt(_car.transform);
        }
        
        void UpdateCameraFOV()
        {
            Vector3 temp = rotationVector;
            temp.y = _car.transform.eulerAngles.y + _forwardDegree;
            rotationVector = temp;
            
            float accelerationValue = _carRigidbody.velocity.magnitude;
            _camera.fieldOfView = _defaultFOV + accelerationValue * _zoomRatio * Time.deltaTime;
        }

        void LookBack(bool isLookBack) =>
            _forwardDegree = isLookBack ? 0 : 180;
    }
}