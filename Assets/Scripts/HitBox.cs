using UnityEngine;

public class HitBox : MonoBehaviour, IDamageable
{
    [SerializeField] protected float damageMultiplier = 1; // he so
    protected virtual void Awake()
    {
        
    }
    
    
    
    public virtual void TakeDamage(int  damage)
    {
        
    }
}