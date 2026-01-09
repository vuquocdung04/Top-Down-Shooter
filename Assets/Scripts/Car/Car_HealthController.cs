using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamageable
{
    private Car_Controller carController;
    public int maxHealth = 100;
    public int currentHealth;

    private bool carBroken;

    [Header("Explosion Info")] [SerializeField]
    private int explosionDamage = 350;

    [Space]
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] private float explosionDelay = 3;
    [SerializeField] private float explosionForce = 7;
    [SerializeField] private float explosionUpwardsModifer = 2;
    [SerializeField] private Transform explosionPoint;
    [Space]
    [SerializeField] private ParticleSystem fireFx;
    [SerializeField] private ParticleSystem explosionFx;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
        
        fireFx.gameObject.SetActive(false);
        explosionFx.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(fireFx.gameObject.activeSelf)
            fireFx.transform.rotation = Quaternion.identity;
    }

    public void UpdateCarHealthUI()
    {
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth, maxHealth);
    }

    private void ReduceHealth(int damage)
    {
        if (carBroken) return;
        currentHealth -= damage;

        if (currentHealth < 0)
            BrakeTheCar();
    }

    private void BrakeTheCar()
    {
        carBroken = true;
        carController.BrakeTheCar();
        // enable smoke
        // invoke explosion

        fireFx.gameObject.SetActive(true);
        StartCoroutine(ExplosionCo(explosionDelay));
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage);
        UpdateCarHealthUI();
    }

    private IEnumerator ExplosionCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        explosionFx.gameObject.SetActive(true);
        carController.rb.AddExplosionForce(explosionForce, explosionPoint.position,
            explosionRadius, explosionUpwardsModifer, ForceMode.Impulse);

        Explore();
    }

    private void Explore()
    {
        HashSet<GameObject> unieqEntites = new();
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;
                if (unieqEntites.Add(rootEntity) == false)
                    continue;
                damageable.TakeDamage(explosionDamage);
                

                hit.GetComponentInChildren<Rigidbody>().AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius,
                    explosionUpwardsModifer, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(explosionPoint.position,explosionRadius);
    }
}