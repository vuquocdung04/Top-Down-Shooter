using System;
using UnityEngine;

public class Enemy_Shield_Obj : MonoBehaviour, IDamageable
{
    private Enemy_Melee enemy;
    [SerializeField] private int durability = 10;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();

        durability = enemy.shieldDurability;
    }

    private void ReduceDurability()
    {
        durability--;
        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage()
    {
        ReduceDurability();
    }
}