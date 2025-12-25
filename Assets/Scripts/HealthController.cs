using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    private bool _isDead;
    
    protected virtual void Awake()
    {
        currentHealth = maxHealth;   
    }

    public virtual void ReduceHealth(int damage)
    {
        currentHealth -= damage;
    }

    public virtual void IncreaseHealth()
    {
        currentHealth++;
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public bool ShouldDie()
    {
        if(_isDead) return false;
        if (currentHealth < 0)
        {
            _isDead = true;
            Debug.Log(gameObject.name + " is dead");
            return true;
        }
        return false;
    }
}