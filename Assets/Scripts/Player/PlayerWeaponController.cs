using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f;


    private Player player;

    [SerializeField] private Weapon currentWeapon;

    [Header("Bullet Details")] [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")] [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.bulletsInMagazine = currentWeapon.totalReserveAmmo;
    }

    #region Slot management - Pick/Equip/Drop weapon

    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];
    }

    public void PickupItem(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
            return;

        weaponSlots.Add(newWeapon);
    }

    private void DropWeapon()
    {
        if (weaponSlots.Count <= 1) return;

        weaponSlots.Remove(currentWeapon);

        currentWeapon = weaponSlots[0];
    }

    #endregion

    private void Shoot()
    {
        if (!currentWeapon.CanShoot()) return;

        GameObject newBullet =
            Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;
        Destroy(newBullet, 10f);
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (!player.aim.CanAimPrecisely() && player.aim.Target() == null)
            direction.y = 0;
        // weaponHolder.LookAt(aim);
        // gunPoint.LookAt(aim); TODO: find a better place for it

        return direction;
    }

    public Weapon CurrentWeapon() => currentWeapon;
    public Transform GunPoint() => gunPoint;

    #region Input Events

    private void AssignInputEvents()
    {
        player.controls.Character.Fire.performed += context => Shoot();
        player.controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        player.controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        player.controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        player.controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload())
                player.weaponVisuals.PlayReloadAnimation();
        };
    }

    #endregion
}