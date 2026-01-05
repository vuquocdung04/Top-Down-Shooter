using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    private float moveInput;
    private float steerInput;
    [SerializeField] private  float speed;
    [SerializeField] private  float turnSensitivity;

    [Header("Car Settings")] [SerializeField]
    private Transform centerOfMass;

    [Header("Engine Settings")] [SerializeField] private  float currentSpeed;
    [Range(7, 12)] [SerializeField] private float maxSpeed;
    [Range(0.5f, 5f)] [SerializeField] private  float accelerationSpeed; // gia toc
    [Range(1500, 3000)] [SerializeField] private  float motorForce = 1500f;

    [Header("Brake Settings")] [Range(4, 10)]
    [SerializeField] private  float brakeSensitivity = 5;

    [Range(4000, 6000)] [SerializeField] private  float brakePower = 5000;
    private bool isBraking;

    [Header("Drift Settings")]
    [Range(0, 1)] 
    [SerializeField] private float frontDriftFactor = 0.5f;
    [Range(0, 1)] 
    [SerializeField] private float backDriftFactor = 0.5f;
    [SerializeField] private float driftDuration = 1f;
    private float driftTimer;

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

        driftTimer -= Time.deltaTime;
        if (driftTimer < 0)
        {
            isBraking = false;
        }
    }

    private void FixedUpdate()
    {
        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();
        ApplySpeedLimit();

        if (isBraking)
        {
            ApplyDrift();
        }
        else
        {
            StopDrift();
        }
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

    private void ApplyDrift()
    {
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axelType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : backDriftFactor;
            
            
            WheelFrictionCurve slidewaysFriction = wheel.cd.sidewaysFriction;

            slidewaysFriction.stiffness *= (1 - driftFactor);
            wheel.cd.sidewaysFriction = slidewaysFriction;
        }
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStiffness();
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
            wheel.cd.GetWorldPose(out var position, out var rotation);
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
        controls.Car.Movement.canceled += _ =>
        {
            moveInput = 0;
            steerInput = 0;
        };

        controls.Car.Brake.performed += _ =>
        {
            isBraking = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += _ => isBraking = false;
    }
}