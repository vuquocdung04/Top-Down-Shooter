using System;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    public PlayerControls controls;
    private float moveInput;
    private float steerInput;

    private bool isBraking;
    private void Start()
    {
        controls = ControlsManager.instance.controls;
        ControlsManager.instance.SwitchToCarControls();

        AssignInputEvents();
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