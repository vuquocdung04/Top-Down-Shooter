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


[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;
    public int bulletsInMagazine; // hien tai
    public int magazineCapacity; // suc chua
    public int totalReserveAmmo; // du tru

    [Range(1, 3)] public float reloadSpeed = 1;

    [Range(1, 3)] public float equipmentSpeed = 1;
    public bool CanShoot() => HaveEnoughBullets();

    private bool HaveEnoughBullets()
    {
        if (bulletsInMagazine <= 0) return false;
        bulletsInMagazine--;
        return true;
    }

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity) return false;
        return totalReserveAmmo > 0;
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
}