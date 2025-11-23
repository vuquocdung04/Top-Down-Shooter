using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f;


    private Player player;

    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;


    [Header("Bullet Details")] [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")] [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        Invoke(nameof(EquipStartingWeapon), 1f);
    }

    private void Update()
    {
        if (isShooting)
            Shoot();

        if (Input.GetKeyDown(KeyCode.T))
            currentWeapon.ToggleBurst();
    }


    #region Slot management - Pick/Equip/Drop weapon

    private void EquipStartingWeapon()
    {
        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
    }

    public void PickupItem(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
            return;

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) return;

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
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
        
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;
        
        GameObject newBullet = ObjectPool.instance.GetBullet();

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (!player.aim.CanAimPrecisely() && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

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

    #region Input Events

    private void AssignInputEvents()
    {
        player.controls.Character.Fire.performed += context => isShooting = true;
        player.controls.Character.Fire.canceled += context => isShooting = false;

        player.controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        player.controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        player.controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        player.controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
    }

    #endregion
}