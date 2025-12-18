using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data/Range Weapon Data", order = 1)]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon details")]
    
    public Enemy_RangeWeaponType weaponType;
    public float fireRate = 1; // bullets per second

    public int minBulletsPerAttack = 1;
    public int maxBulletsPerAttack = 1;

    public float minWeaponCooldown = 2;
    public float maxWeaponCooldown = 3;

    [Header("Bullet details")] public int bulletDamage;
    [Space]
    public float bulletSpeed = 20f;
    public float weaponSpread = 0.1f;
    
    public int GetBulletsPerAttack() => Random.Range(minBulletsPerAttack, maxBulletsPerAttack);

    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);
    
    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizedValue = Random.Range(-weaponSpread, weaponSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        return spreadRotation * originalDirection;
    }
}