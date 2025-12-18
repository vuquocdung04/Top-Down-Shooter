using System;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public int currentHealth;
    public int maxHealth = 100;

    [Space]
    public MeshRenderer mesh;
    public Material whileMaterial;
    public Material redMaterial;
    [Space] public float refreshCooldown;
    private float lastTimeDamaged;
    
    
    private void Start() => Refesh();

    private void Update()
    {
        if (Time.time > refreshCooldown + lastTimeDamaged || Input.GetKeyDown(KeyCode.B))
            Refesh();
    }

    private void Refesh()
    {
        currentHealth = maxHealth;
        mesh.sharedMaterial = whileMaterial;
    }

    public void TakeDamage(int damage)
    {
        lastTimeDamaged = Time.time;
        currentHealth -= damage;
        
        if(currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        mesh.sharedMaterial = redMaterial;
    }
}