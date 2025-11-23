using UnityEngine;

public enum WeaponType
{
    None = 0,
    Pistol = 1,
    Revolver = 2,
    AutoRifle = 3,
    ShotGun = 4,
    Sniper = 5,
}

public enum ShootType
{
    Single = 0,
    Auto = 1,
}


[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;
    [Header("Shooting specifics")] public ShootType shootType;
    [Space] public float fireRate = 1; // bullets per second
    private float lastShootTime;

    [Header("Magazine Details")]
    public int bulletsInMagazine; // hien tai
    public int magazineCapacity; // suc chua
    public int totalReserveAmmo; // du tru

    [Range(1, 3)] public float reloadSpeed = 1;

    [Range(1, 3)] public float equipmentSpeed = 1;


    public bool CanShoot()
    {
        if (!HaveEnoughBullets() || !ReadyToFire()) return false;
        bulletsInMagazine--;
        return true;
    }

    private bool ReadyToFire()
    {
        if (!(Time.time > lastShootTime + 1 / fireRate)) return false;
        lastShootTime = Time.time;
        return true;
    }
    

    #region Reload methods

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity) return false;
        return totalReserveAmmo > 0;
    }

    private bool HaveEnoughBullets()
    {
        return bulletsInMagazine > 0;
    }

    public void RefillBullets()
    {
        //totalReserveAmmo += bulletsInMagazine; 

        int bulletsToReload = magazineCapacity;
        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }

    #endregion
}