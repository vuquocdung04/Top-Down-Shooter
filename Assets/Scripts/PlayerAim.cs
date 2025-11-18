using System;
using UnityEngine;


public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    private Vector2 aimInput;
    
    [Header("Aim Info")]
    
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;

    private void Start()
    {
        player = GetComponent<Player>();
        
        AssignInputEvents();
    }

    private void Update()
    {
        aim.position = new Vector3(GetMousePosition().x, transform.position.y + 1f,  GetMousePosition().z);
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