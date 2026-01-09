using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class Player_AimController : MonoBehaviour
{
    private CameraManager cameraManager;
    private Player player;
    private PlayerControls controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser;

    [Header("Aim control")] [SerializeField]
    private float preciseAimCameraDistance = 6;
    [SerializeField] private float regularAimCameraDistance = 7;
    [SerializeField] private float camChangeRate = 5;
    
    [Header("Aim Setup")]
    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisely;
    [SerializeField] private float offSetChangeRate = 6;
    private float offSetY;
    
    [Header("Aim Layers")]
    [SerializeField] private LayerMask preciseAim;
    [SerializeField] private LayerMask regularAim;
    
    [Header("Camera control")] [Range(1, 3f)] [SerializeField]
    private float maxCameraDistance = 4f;

    [Range(0.5f, 1)] [SerializeField] private float minCameraDistance = 1.5f;
    [Range(3f, 5f)] [SerializeField] private float cameraSensitivity = 5f;

    [SerializeField] private Transform cameraTarget;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;
    private Camera mainCamera;

    private void Start()
    {
        cameraManager = CameraManager.instance;
        player = GetComponent<Player>();
        mainCamera = Camera.main;
        AssignInputEvents();

        Cursor.visible = false;
    }

    private void Update()
    {
        if(player.health.hasDead) return;
        
        if(player.controlsEnable == false) return;
        
        UpdateAimVisuals();
        
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void EnablePreciseAim(bool enable)
    {
        isAimingPrecisely = !isAimingPrecisely;
        Cursor.visible = false;
        if (enable)
        {
            cameraManager.ChangeCameraDistance(preciseAimCameraDistance,camChangeRate);
            Time.timeScale = 0.9f;
        }
        else
        {
            cameraManager.ChangeCameraDistance(regularAimCameraDistance,camChangeRate);
            Time.timeScale = 1f;
        }
    }
    
    public Transform GetAimCameraTarget()
    {
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    public void EnableAimLaser(bool enable) => aimLaser.enabled = enable;
    
    private void UpdateAimVisuals()
    {
        aim.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        
        aimLaser.enabled = player.weapon.WeaponReady();
        if(!aimLaser.enabled) return;

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);
        
        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();
        
        float laserTipLenght = 0.5f;
        float gunDistance = player.weapon.CurrentWeapon().gunDistance;
        
        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLenght = 0;
        }
        
        aimLaser.SetPosition(0,gunPoint.position);
        aimLaser.SetPosition(1,endPoint);
        aimLaser.SetPosition(2,endPoint + laserDirection * laserTipLenght);
    }

    private void UpdateAimPosition()
    {
        aim.position = GetMouseHitInfo().point;

        Vector3 newAimPosition = isAimingPrecisely ? aim.position : transform.position;
        
        aim.position = new Vector3(aim.position.x, newAimPosition.y + AdjustedOffsetY(), aim.position.z);
           
    }

    private float AdjustedOffsetY()
    {
        if(isAimingPrecisely)
            offSetY = Mathf.Lerp(offSetY,0,Time.deltaTime * offSetChangeRate * 0.5f);
        else
            offSetY = Mathf.Lerp(offSetY, 1, Time.deltaTime * offSetChangeRate);

        return offSetY;
    }
    
    public Transform Aim() => aim;
    public bool CanAimPrecisely() => isAimingPrecisely;

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = mainCamera.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, preciseAim))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }

    #region Camera Region
    private void UpdateCameraPosition()
    {
        bool canMoveCamera = Vector3.Distance(cameraTarget.position, DesiredCameraPosition()) > 1;
        if(canMoveCamera == false) return;
        
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(),
            cameraSensitivity * Time.deltaTime);
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

    #endregion

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += _ => mouseInput = Vector2.zero;

        controls.Character.PreciseAim.performed += _ => EnablePreciseAim(true);
        controls.Character.PreciseAim.canceled += _ => EnablePreciseAim(false);
    }


}