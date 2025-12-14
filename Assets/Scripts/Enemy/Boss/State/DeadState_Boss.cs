public class DeadState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private bool interactionDisabled;
    
    public DeadState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void EnterState()
    {
        base.EnterState();
        
        enemy.abilityState.DisableFlameThrower();
        
        interactionDisabled = false;
        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;
        enemy.ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
    
    private void DisableInteractionIfShould()
    {
        if (stateTimer <= 0 && !interactionDisabled)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false);
            enemy.ragdoll.CollidersActive(false);
        }
    }
}