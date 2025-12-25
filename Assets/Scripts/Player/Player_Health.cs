using UnityEngine;

public class Player_Health : HealthController
{
    private Player player;
    public bool hasDead;
    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);
        
        if(ShouldDie())
            Die();
    }

    private void Die()
    {
        if(hasDead) return;
        
        Debug.Log("Player was killed at " + Time.time);
        
        hasDead = true;
        player.anim.enabled = false;
        player.ragdoll.RagdollActive(true);
    }
}