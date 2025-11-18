using System;
using UnityEngine;


public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    private Vector2 aimInput;
    
    [Header("Aim Info")]
    
    [Range(1,3f)]
    [SerializeField] private float maxCameraDistance = 4f;
    [Range(0.5f,1)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(3f,5f)]
    [SerializeField] private float aimSensitivity = 5f;
    
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;

    private void Start()
    {
        player = GetComponent<Player>();
        
        AssignInputEvents();
    }

    private void Update()
    {
        aim.position = Vector3.Lerp(aim.position, DesiredAimPosition(), aimSensitivity * Time.deltaTime);
    }

    private Vector3 DesiredAimPosition()
    {
        
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;
        
        
        Vector3 desiredAimPosition = GetMousePosition();
        Vector3 aimDirection = (desiredAimPosition - transform.position).normalized;
        
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredAimPosition);

        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);
        
        desiredAimPosition = transform.position + aimDirection * clampedDistance;
        desiredAimPosition.y = transform.position.y + 1f;
        
        return desiredAimPosition;
    }
    
    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayerMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;
        
        controls.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aimInput = Vector2.zero;
    }
}