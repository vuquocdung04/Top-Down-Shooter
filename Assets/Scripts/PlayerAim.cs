using System;
using UnityEngine;


public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;


    [Header("Aim control")]
    
    [SerializeField] private Transform aim;
    
    [Header("Camera control")]
    
    [Range(1,3f)]
    [SerializeField] private float maxCameraDistance = 4f;
    [Range(0.5f,1)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(3f,5f)]
    [SerializeField] private float cameraSensitivity = 5f;
    
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private LayerMask aimLayerMask;

    private Vector2 aimInput;
    private RaycastHit lastKnownMouseHit;
    
    private void Start()
    {
        player = GetComponent<Player>();
        
        AssignInputEvents();
    }

    private void Update()
    {
        aim.position = GetMouseHitInfo().point;
        aim.position = new Vector3(aim.position.x, transform.position.y + 1f, aim.position.z);
        
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    private Vector3 DesiredCameraPosition()
    {
        
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;
        
        
        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;
        
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);

        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);
        
        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1f;
        
        return desiredCameraPosition;
    }
    
    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }
        return lastKnownMouseHit;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;
        
        controls.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aimInput = Vector2.zero;
    }
}