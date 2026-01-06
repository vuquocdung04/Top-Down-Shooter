using System;
using UnityEngine;

public class Car_DamageZone : MonoBehaviour
{
    private Car_Controller carController;

    [SerializeField] private float minSpeedToDamage = 1.5f;
    [SerializeField] private int carDamage;
    [SerializeField] private float impactForce = 150;
    [SerializeField] private float upwardsMultiplier = 3f;

    private void Awake()
    {
        carController = GetComponentInParent<Car_Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (carController.rb.velocity.magnitude < minSpeedToDamage)
        {
            Debug.Log(carController.rb.velocity.magnitude);
            return;
        }
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null) return;
        
        damageable.TakeDamage(carDamage);

        Rigidbody rb = other.GetComponent<Rigidbody>();
        
        if(rb != null)
            ApplyForce(rb);
    }

    private void ApplyForce(Rigidbody rb)
    {
        if(rb == null) return;

        rb.isKinematic = false;
        rb.AddExplosionForce(impactForce, transform.position,3, upwardsMultiplier, ForceMode.Impulse);
    }
    
}