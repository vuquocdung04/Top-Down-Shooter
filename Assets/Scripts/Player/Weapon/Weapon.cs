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
    public int bulletDamage;
    #region Regular mode variables
    public ShootType shootType;
    public int bulletsPerShot { get; private set; }
    private float defaultFireRate;
    public float fireRate; // bullets per second
    private float lastShootTime;
    #endregion

    #region Burst variables
    private bool burstAvailable;
    public bool burstActive;
    private int burstBulletsPerShot;
    private float burstFireRate;
    public float burstFireDelay { get; private set;}
    #endregion

    [Header("Magazine Details")] public int bulletsInMagazine; // hien tai
    public int magazineCapacity; // suc chua
    public int totalReserveAmmo; // du tru

    #region Weapon Renegic info
    public float reloadSpeed { get; private set; }
    public float equipmentSpeed { get; private set; }
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set;  }
    #endregion

    #region Spread variables
    private float baseSpread;
    private float maximumSpread;
    private float spreadIncreaseRate;
    private float currentSpread;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;
    #endregion
    
    public Weapon_Data Data { get; private set; }
    

    public Weapon(Weapon_Data data)
    {
        bulletDamage = data.bulletDamage;
        weaponType = data.weaponType;
        shootType = data.shootType;
        bulletsPerShot = data.bulletsPerShot;
        fireRate = data.fireRate;
        
        bulletsInMagazine = data.bulletsInMagazine;
        magazineCapacity = data.magazineCapacity;
        totalReserveAmmo = data.totalReserveAmmo;

        baseSpread = data.baseSpread;
        maximumSpread = data.maximumSpread;
        spreadIncreaseRate = data.spreadIncreaseRate;
        
        reloadSpeed = data.reloadSpeed;
        equipmentSpeed = data.equipmentSpeed;
        gunDistance = data.gunDistance;
        cameraDistance = data.cameraDistance;
        
        burstAvailable = data.burstAvailable;
        burstActive = data.burstActive;
        burstBulletsPerShot = data.burstBulletsPerShot;
        burstFireRate = data.burstFireRate;
        burstFireDelay = data.burstFireDelay;
        
        defaultFireRate = fireRate;

        this.Data = data;
    }
    

    #region Spread methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue/2, randomizedValue);
        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            currentSpread = baseSpread;
        else
            IncreaseSpread();
        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion

    #region Burst methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.ShotGun)
        {
            burstFireDelay = 0;
            return true;
        }
        return burstActive;
    }

    public void ToggleBurst()
    {
        if(!burstAvailable) return;
        
        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShot;
            fireRate  = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion


    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

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