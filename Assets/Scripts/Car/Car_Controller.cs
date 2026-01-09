using UnityEngine;

public enum DriveType
{
    FrontWheelDrive = 0, // Cầu trước (tiết kiệm nhiên liệu, dễ điều khiển)
    RearWheelDrive = 1, // Cầu sau (drift dễ hơn, xe thể thao)
    AllWheelDrive = 2,  // Cả 4 bánh (bám đường tốt, off-road)
}
[RequireComponent(typeof(Rigidbody))]
public class Car_Controller : MonoBehaviour
{
    public Car_Sounds carSounds {get; private set;}
    public Rigidbody rb {get; private set;}
    public bool carActive { get; private set; }
    private PlayerControls controls;
    private float moveInput;
    private float steerInput;
    public float speed;
    
    [SerializeField] private LayerMask whatIsGround;
    
    [Range(30,60)]
    [SerializeField] private float turnSensitivity = 30;

    [Header("Car Settings")] [SerializeField]
    private DriveType driveType;
    [SerializeField] private Transform centerOfMass;
    [Range(350, 1000)] [SerializeField] private float carMass = 400;
    [Range(20,80)] [SerializeField] private float wheelsMass = 30;

    // Độ bám đường
    [Range(0.5f, 2)] [SerializeField] private float frontWheelTraction = 1; 
    [Range(0.5f, 2)] [SerializeField] private float backWheelTraction = 1;
    
    [Header("Engine Settings")] [SerializeField]
    private float currentSpeed;

    [Range(7, 12)] [SerializeField] private float maxSpeed = 7;
    //Tốc độ tăng tốc (gia tốc):
    [Range(0.5f, 10f)] [SerializeField] private float accelerationSpeed = 2;
    // Lực động cơ tác động lên bánh:
    [Range(1500, 5000)] [SerializeField] private float motorForce = 1500f;

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
    private bool isDrifting;
    private bool canEmitTrails = true;
    
    private Car_Wheel[] wheels;
    private UI ui;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();
        carSounds  = GetComponent<Car_Sounds>();
        ui = UI.instance;
        controls = ControlsManager.instance.controls;
        
        ActivateCar(false);
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
        if(!carActive) return;
        
        ui.inGameUI.UpdateSpeedText(Mathf.RoundToInt(speed * 10) + " km/h");
        
        speed = rb.velocity.magnitude;

        driftTimer -= Time.deltaTime;
        if (driftTimer < 0)
        {
            isDrifting = false;
        }
    }

    private void FixedUpdate()
    {
        if(!carActive) return;

        ApplyTrailsOnGround();
        
        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();
        ApplySpeedLimit();

        if (isDrifting)
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
            
            float newBrakeTorque = brakePower * brakeSensetivity;
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
        currentSpeed = moveInput * accelerationSpeed;
        float motorTorqueValue = motorForce * currentSpeed * Time.fixedDeltaTime;
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
    
    private void ApplyTrailsOnGround()
    {
        if(!canEmitTrails) return;
        
        foreach (var wheel in wheels)
        {
            WheelHit hit;
            if (wheel.cd.GetGroundHit(out hit))
            {
                if (whatIsGround == (whatIsGround | (1 << hit.collider.gameObject.layer)))
                    wheel.trail.emitting = true;
                else
                    wheel.trail.emitting = false;
            }
            else
                wheel.trail.emitting = false;
        }
    }

    public void ActivateCar(bool activate)
    {
        carActive = activate;
        if(carSounds != null)
            carSounds.ActivateCarSFX(activate);
        // carActive = activate;
        // rb.constraints = activate ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
    }

    public void BrakeTheCar()
    {
        canEmitTrails = false;
        foreach (var wheel in wheels)
        {
            wheel.trail.emitting = false;
        }

        rb.drag = 1;
        motorForce = 0;
        isDrifting = true;
        frontDriftFactor = 0.9f;
        backDriftFactor = 0.9f;
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
            isDrifting = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += _ => isBraking = false;
    }

    [ContextMenu("Focus camera and enable")]
    public void TestThisCar()
    {
        ActivateCar(true);
        CameraManager.instance.ChangeCameraTarget(transform,20);
    }
}