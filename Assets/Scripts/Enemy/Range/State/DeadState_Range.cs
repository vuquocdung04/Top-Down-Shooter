public class DeadState_Range : EnemyState
{
    private Enemy_Range enemy;
    private bool interactionDisabled;
    
    
    public DeadState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();

        if (enemy.throwGrenadeState.finishedThrowingGrenade == false)
        {
            enemy.ThrowGrenade();
        }
        
        interactionDisabled = false;
        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;
        enemy.ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        DisableInteractionIfShould();
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