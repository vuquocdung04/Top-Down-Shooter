using UnityEngine;

public enum DriveType
{
    FrontWheelDrive = 0,
    RearWheelDrive = 1, // sau
    AllWheelDrive = 2,
}

public class Car_Controller : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    private float moveInput;
    private float steerInput;
    [SerializeField] private float speed;
    [Range(30,60)]
    [SerializeField] private float turnSensitivity = 30;

    [Header("Car Settings")] [SerializeField]
    private DriveType driveType;
    [SerializeField] private Transform centerOfMass;
    [Range(350, 1000)] [SerializeField] private float carMass = 400;
    [Range(20,80)] [SerializeField] private float wheelsMass = 30;

    [Range(0.5f, 2)] [SerializeField] private float frontWheelTraction = 1;
    [Range(0.5f, 2)] [SerializeField] private float backWheelTraction = 1;
    
    [Header("Engine Settings")] [SerializeField]
    private float currentSpeed;

    [Range(7, 12)] [SerializeField] private float maxSpeed = 7;
    [Range(0.5f, 5f)] [SerializeField] private float accelerationSpeed = 2; // gia toc
    [Range(1500, 3000)] [SerializeField] private float motorForce = 1500f;

    [Header("Brake Settings")]
    [Range(0, 10)] [SerializeField] private float frontBrakeSensitivity = 5;
    [Range(0, 10)] [SerializeField] private float backBrakeSensitivity = 5;

    [Range(4000, 6000)] [SerializeField] private float brakePower = 5000;
    private bool isBraking;

    [Header("Drift Settings")] [Range(0, 1)] [SerializeField]
    private float frontDriftFactor = 0.5f;

    [Range(0, 1)] [SerializeField] private float backDriftFactor = 0.5f;
    [SerializeField] private float driftDuration = 1f;
    private float driftTimer;

    private Car_Wheel[] wheels;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();
        
        controls = ControlsManager.instance.controls;
        ControlsManager.instance.SwitchToCarControls();
        
        AssignInputEvents();
        SetupDefaultValues();
    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelsMass;
            if(wheel.axelType == AxelType.Front)
                wheel.SetDefaultStiffness(frontWheelTraction);

            if (wheel.axelType == AxelType.Back)
                wheel.SetDefaultStiffness(backWheelTraction);
        }
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
        foreach (var wheel in wheels)
        {
            bool frontBakes = wheel.axelType == AxelType.Front;
            float brakeSensetivity = frontBakes ? frontBrakeSensitivity : backBrakeSensitivity;
            
            float newBrakeTorque = brakePower * brakeSensetivity * Time.fixedDeltaTime;
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0f;
            
            wheel.cd.brakeTorque = currentBrakeTorque;
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
            switch (driveType)
            {
                case DriveType.FrontWheelDrive:
                {
                    if (wheel.axelType == AxelType.Front)
                        wheel.cd.motorTorque = motorTorqueValue;
                    break;
                }
                case DriveType.RearWheelDrive:
                {
                    if (wheel.axelType == AxelType.Back)
                        wheel.cd.motorTorque = motorTorqueValue;
                    break;
                }
                default:
                    wheel.cd.motorTorque = motorTorqueValue;
                    break;
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