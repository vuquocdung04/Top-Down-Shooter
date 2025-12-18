using System;
using UnityEngine;

public class FlameThrow_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy;
    private float damageCooldown;
    private float lastTimeDamaged;
    private int flameDamage;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();
        damageCooldown = enemy.flameDamageCooldown;
        flameDamage = enemy.flameDamage;
    }

    private void OnTriggerStay(Collider other)
    {
        if(enemy.flameThrowActive == false) return;
        
        if(Time.time - lastTimeDamaged < damageCooldown)
            return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(flameDamage); 
            lastTimeDamaged = Time.time; // Update the last time damage was applied
            damageCooldown = enemy.flameDamageCooldown; // for easier testing I'm updating 
                                                        // cooldown everytime we damage enemy
        }
    }
}