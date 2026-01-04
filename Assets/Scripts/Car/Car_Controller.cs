using System;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    private float moveInput;
    private float steerInput;
    public float speed;
    public float turnSensitivity;
    
    [Header("Car Settings")]
    [SerializeField] private Transform centerOfMass;
    
    [Header("Engine Settings")]
    public float currentSpeed;
    [Range(7,12)] public float maxSpeed;
    [Range(0.5f,5f)] public float accelerationSpeed; // gia toc
    [Range(1500,3000)] public float motorForce = 1500f;

    [Header("Brake Settings")]
    [Range(4,10)]public float brakeSensitivity = 5;
    [Range(4000,6000)] public float brakePower = 5000;
    private bool isBraking;
    private Car_Wheel[] wheels;
    
    private void Start()
    {
        controls = ControlsManager.instance.controls;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
        ControlsManager.instance.SwitchToCarControls();
        wheels = GetComponentsInChildren<Car_Wheel>();
        AssignInputEvents();
    }

    private void Update()
    {
        speed = rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();
        ApplySpeedLimit();
    }

    private void ApplyBrakes()
    {
        float newBrakeTorque = brakePower * brakeSensitivity * Time.fixedDeltaTime;
        float currentBrakeTorque = isBraking ? newBrakeTorque : 0f;

        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                wheel.cd.brakeTorque = currentBrakeTorque;
            }
        }
    }

    private void ApplyDrive()
    {
        currentSpeed = moveInput * accelerationSpeed * Time.fixedDeltaTime;
        float motorTorqueValue = motorForce * currentSpeed;
        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                wheel.cd.motorTorque = motorTorqueValue;
            }
        }
    }

    private void ApplySpeedLimit()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
    
    private void ApplySteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                float targetSteerAngle = steerInput * turnSensitivity;
                wheel.cd.steerAngle = Mathf.Lerp(wheel.cd.steerAngle, targetSteerAngle, 0.5f);
            }
        }
    }

    private void ApplyAnimationToWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;
            wheel.cd.GetWorldPose(out position, out rotation);

            if (wheel.model != null)
            {
                wheel.model.transform.position = position;
                wheel.model.transform.rotation = rotation;
            }
        }
    }
    
    private void AssignInputEvents()
    {
        controls.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            moveInput = input.y;
            steerInput = input.x;
        };
        controls.Car.Movement.canceled += ctx =>
        {
            moveInput = 0;
            steerInput = 0;
        };

        controls.Car.Brake.performed += ctx => isBraking = true;
        controls.Car.Brake.canceled += ctx =>  isBraking = false;
    }
}