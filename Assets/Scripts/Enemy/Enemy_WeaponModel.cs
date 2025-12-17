using System;
using UnityEngine;
public class Enemy_WeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponType;
    public AnimatorOverrideController overrideController;
    public Enemy_MeleeWeaponData weaponData;
    
    [SerializeField] private GameObject[] trailEffects;

    [Header("Damage Attribute")] public Transform[] damagePoints;
    public float attackRadius;

    [ContextMenu("Assign damage point transforms")]
    private void GetDamagePoints()
    {
        damagePoints = new Transform[trailEffects.Length];
        for (int i = 0; i < damagePoints.Length; i++)
        {
            damagePoints[i] = trailEffects[i].transform;
        }
    }
    public void EnableTrailEffect(bool enable)
    {
        foreach (var trailEffect in trailEffects)
        {
            trailEffect.SetActive(enable);
        }
    }

    private void OnDrawGizmos()
    {
        if (damagePoints.Length > 0)
        {
            foreach (var point in damagePoints)
            {
                Gizmos.DrawWireSphere(point.position,attackRadius);
            }
        }
    }
}