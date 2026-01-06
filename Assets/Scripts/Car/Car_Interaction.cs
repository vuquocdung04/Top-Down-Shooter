using System;
using UnityEngine;

public class Car_Interaction : Interactable
{
    private Car_HealthController carHealthController;
    private Car_Controller car;
    private Transform player;

    private float defaultPlayerScale;

    [Header("Exit details")]
    [SerializeField] private float exitCheckRadius;
    [SerializeField] private Transform[] exitPoints;
    [SerializeField] private LayerMask whatToIgnoreForExit;
    
    private void Start()
    {
        car = GetComponent<Car_Controller>();
        carHealthController = GetComponent<Car_HealthController>();
        player = GameManager.instance.player.transform;
        
        PlayerControls controls = ControlsManager.instance.controls;

        controls.Car.CarExit.performed += ctx => GetOutOfTheCar();
    }

    public override void Interaction()
    {
        base.Interaction();
        GetIntoTheCar();
    }

    public void GetIntoTheCar()
    {
        ControlsManager.instance.SwitchToCarControls();
        carHealthController.UpdateCarHealthUI(); 
        car.ActivateCar(true);
        defaultPlayerScale = player.localScale.x;
        player.localScale = new Vector3(0.01f,0.01f,0.01f);
        player.parent = transform;
        player.localPosition = Vector3.up / 2;
        
        CameraManager.instance.ChangeCameraTarget(transform);
    }
    
    private void GetOutOfTheCar()
    {
        if(car.carActive == false) return;
        
        car.ActivateCar(false);
        player.parent = null;
        player.position = GetExitPoint();
        player.localScale = Vector3.one * defaultPlayerScale;
        ControlsManager.instance.SwitchToCarControls();
        Player_AimController aim = GameManager.instance.player.AimController;
        CameraManager.instance.ChangeCameraTarget(aim.GetAimCameraTarget());
    }

    private Vector3 GetExitPoint()
    {
        for (int i = 0; i < exitPoints.Length; i++)
        {
            if(IsExitClear(exitPoints[i].position))
                return exitPoints[i].position;
        }
        
        return exitPoints[0].position;
    }
    
    private bool IsExitClear(Vector3 point)
    {
        Collider[] colliders = Physics.OverlapSphere(point, exitCheckRadius, ~whatToIgnoreForExit);
        return colliders.Length == 0;
    }
    
    private void OnDrawGizmos()
    {
        if (exitPoints.Length > 0)
        {
            foreach (var point in exitPoints)
            {
                Gizmos.DrawWireSphere(point.position, exitCheckRadius);
            }
        }
    }
}