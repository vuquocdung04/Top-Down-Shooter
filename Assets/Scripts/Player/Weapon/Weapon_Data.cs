using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data", order = 0)]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;
    [Header("Bullet")]
    public int bulletDamage;
    
    [Header("Magazine Details")]
    public int bulletsInMagazine; // hien tai
    public int magazineCapacity; // suc chua
    public int totalReserveAmmo; // du tru
    
    [Header("Regular shot")]
    public ShootType shootType;
    public int bulletsPerShot = 1;
    public float fireRate;
    
    [Header("Burst shot")]
    public bool burstAvailable;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay;

    [Header("Weapon Spread")]
    public float baseSpread;
    public float maximumSpread;
    public float spreadIncreaseRate = 0.15f;

    [Header("Weapon Generics")]
    public WeaponType weaponType;
    [Range(1, 3)] public float reloadSpeed = 1;
    [Range(1, 3)] public float equipmentSpeed = 1;
    [Range(4, 25)] public float gunDistance = 4;
    [Range(4, 8)] public float cameraDistance = 6;

    [Header("UI elements")] public Sprite weaponIcon;
    [TextArea]
    public string weaponInfo;
}