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
    [Tooltip("Số lượng đạn bay ra trong 1 lần bóp cò (Shotgun thì để > 1, súng thường là 1)")]
    public int bulletsPerShot;
    [Tooltip("Tốc độ bắn mặc định (dùng để reset khi tắt chế độ Burst)")]
    public float defaultFireRate;
    public float fireRate = 1; // bullets per second
    private float lastShootTime;

    [Header("Burst fire")]
    [Tooltip("Súng này có hỗ trợ chế độ Burst không?")]
    public bool burstAvailable;
    [Tooltip("Có đang bật chế độ Burst hay không?")]
    public bool burstActive;
    [Tooltip("Số viên đạn bắn ra trong 1 loạt Burst (thường là 3 viên)")]
    public int burstBulletsPerShot;
    [Tooltip("Tốc độ bắn khi ở chế độ Burst (thường nhanh hơn bắn thường)")]
    public float burstFireRate;
    public float burstFireDelay = 0.1f;

    [Header("Magazine Details")] public int bulletsInMagazine; // hien tai
    public int magazineCapacity; // suc chua
    public int totalReserveAmmo; // du tru

    [Range(1, 3)] public float reloadSpeed = 1;

    [Range(1, 3)] public float equipmentSpeed = 1;

    [Header("Spread")] public float baseSpread = 1;
    public float maximumSpread = 3;
    public float spreadIncreaseRate = 0.15f;

    private float currentSpread;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;

    #region Spread methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
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