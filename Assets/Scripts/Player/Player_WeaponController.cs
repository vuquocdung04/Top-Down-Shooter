using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f;

    [SerializeField] private LayerMask whatIsAlly;
    private Player player;

    [SerializeField] private List<Weapon_Data> defaultWeaponData;
    
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;


    [Header("Bullet Details")]
    [SerializeField] private float bulletImpactForce = 100;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    
    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")] [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;

    [SerializeField] private GameObject weaponPickupPrefab;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if (isShooting)
            Shoot();
    }

    public void UpdateWeaponUI()
    {
        UI.instance.inGameUI.UpdateWeaponUI(weaponSlots,currentWeapon);
    }
    
    #region Slot management - Pick/Equip/Drop weapon

    public void SetDefaultWeapon(List<Weapon_Data> newWeaponData)
    {
        defaultWeaponData = new(newWeaponData);
        weaponSlots.Clear();

        foreach (Weapon_Data weapon in defaultWeaponData)
        {
            PickupWeapon(new Weapon(weapon));
        }
        
        EquipWeapon(0);
        
    }

    private void EquipWeapon(int i)
    {
        if(i >= weaponSlots.Count) return;
        
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
        
        //CameraManager.instance.ChangeCameraDistance(CurrentWeapon().cameraDistance);
        UpdateWeaponUI();
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }

        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            CreateWeaponOnTheGround();
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();
        
        UpdateWeaponUI();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) return;

        CreateWeaponOnTheGround();
        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab, transform);
        droppedWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon,transform);
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;

    #endregion

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);
        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();
            
            yield return new WaitForSeconds(currentWeapon.burstFireDelay);
            
            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        if (!WeaponReady()) return;
        if (!CurrentWeapon().CanShoot()) return;
        
        player.weaponVisuals.PlayFireAnimation();

        if (CurrentWeapon().shootType == ShootType.Single)
            isShooting = false;

        if (CurrentWeapon().BurstActivated())
        {
            StartCoroutine(BurstFire());
            return;
        }
        FireSingleBullet();
        TriggerEnemyDodge();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;
        UpdateWeaponUI();
        
        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab, GunPoint());
        
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(whatIsAlly,currentWeapon.bulletDamage,currentWeapon.gunDistance,bulletImpactForce);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
        
        // we do actually refill of bullets in Player_AnimationEvents
        // we UpdateWeaponUI in Player_AnimationEvents
        // we UpdateWeaponUI in Player_AnimationEvents
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.AimController.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (!player.AimController.CanAimPrecisely() && player.AimController.Target() == null)
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (var weapon in weaponSlots)
        {
            if(weapon.weaponType == weaponType)
                return weapon;
        }
        return null;
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Weapon BackupWeapon()
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon != currentWeapon)
                return weapon;
        }

        Debug.Log("Null");
        return null;
    }

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    private void TriggerEnemyDodge()
    {
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDirection = BulletDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Enemy_Melee enemyMelee = hit.collider.transform.GetComponentInParent<Enemy_Melee>();
            
            if(enemyMelee)
                enemyMelee.ActivateDodgeRoll();
        }
    }
    
    #region Input Events

    private void AssignInputEvents()
    {
        player.controls.Character.Fire.performed += context => isShooting = true;
        player.controls.Character.Fire.canceled += context => isShooting = false;

        player.controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        player.controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        player.controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        player.controls.Character.EquipSlot4.performed += context => EquipWeapon(3);


        player.controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        player.controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
        player.controls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurst();
    }

    #endregion
}