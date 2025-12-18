using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFx;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardsMultiplier = 1;
    private float impactPower;
    private Rigidbody rb;
    private float timer;
    private void Awake() => rb = GetComponent<Rigidbody>();

    private LayerMask allyLayerMask;
    private bool canExplode = true;
    private int grenadeDamage;
    
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0 && canExplode)
            Explode();
    }

    private void Explode()
    {
        canExplode = false;
        PlayExplosionFx();

        HashSet<GameObject> uniqueEntities = new();
        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (var hit in colliders)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (IsTargetValid(hit) == false)
                    continue;
            
                GameObject rootEntity = hit.transform.root.gameObject;
                if(uniqueEntities.Add(rootEntity) == false)
                    continue;
                
                damageable.TakeDamage(grenadeDamage);
            }
            
            ApplyPhysicalForceTo(hit);
        }
    }

    private void ApplyPhysicalForceTo(Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if(rb != null)
            rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMultiplier, ForceMode.Impulse);
    }
    private void PlayExplosionFx()
    {
        GameObject newFx = ObjectPool.instance.GetObject(explosionFx, transform);
        ObjectPool.instance.ReturnObject(newFx,1);
        ObjectPool.instance.ReturnObject(gameObject);
    }

    public void SetupGrenade(LayerMask allyLayerMask,Vector3 target, float timeToTarget, float countDown, float impactPower, int damage)
    {
        canExplode = true;
        
        grenadeDamage = damage;
        this.allyLayerMask = allyLayerMask;
        rb.velocity = CalculateLaunchVelocity(target, timeToTarget);
        timer = countDown + timeToTarget;
        this.impactPower = impactPower;
    }

    private bool IsTargetValid(Collider col)
    {
        // If friendly fire is enabled, all colliders are valid targets
        if(GameManager.instance.friendlyFire)
            return true;
        
        // If collider is on allyLayer, target is not valid
        if ((allyLayerMask.value & (1 << col.gameObject.layer)) > 0)
            return false;
        
        return true;
    }
    
    // Nem xien
    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);
        
        Vector3 velocityXZ = directionXZ/timeToTarget;
        float velocityY = (direction.y - Physics.gravity.y * Mathf.Pow(timeToTarget,2)/2)/ timeToTarget;
        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;
        return launchVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}