using System;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamageable
{
    private Car_Controller carController;
    public int maxHealth = 100;
    public int currentHealth;

    private bool carBroken;
    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
    }

    private void ReduceHealth(int damage)
    {
        if(carBroken) return;
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
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage);
    }
}
