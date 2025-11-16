using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private CharacterController characterController;
    private Animator animator;
    [Header("Movement Info")]
    [SerializeField] private float walkSpeed;
    private Vector3 movementDirection;
    [SerializeField] private float gravityScale = 9.81f;
    private float verticalVelocity;

    [Header("Aim Info")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookingDirection;
    
    private Vector2 moveInput;
    private Vector2 aimInput;

    private void Awake()
    {
        controls = new PlayerControls();
        //controls.Character.Fire.performed += context => Shoot();
        
        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;
        
        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ApplyMovement();

        AimTowardMouse();
        
        AnimatorController();
    }

    private void AnimatorController()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);
        
        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);
    }

    private void AimTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if(Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lookingDirection = hitInfo.point - transform.position;
            lookingDirection.y = 0;
            lookingDirection.Normalize();
            
            transform.forward = lookingDirection;
            
            aim.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        movementDirection =  new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * (Time.deltaTime * walkSpeed));
        }
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
            verticalVelocity = -0.5f;
    }

    private void Shoot()
    {
        Debug.Log("Shoot");
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
