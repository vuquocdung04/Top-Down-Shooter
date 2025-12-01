public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Enemy_Ragdoll  ragdoll;
    private bool interactionDisabled;
    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
        ragdoll = enemy.GetComponent<Enemy_Ragdoll>();
    }

    public override void EnterState()
    {
        base.EnterState();
        interactionDisabled = false;
        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;
        ragdoll.RagdollActive(true);

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
            ragdoll.RagdollActive(false);
            ragdoll.CollidersActive(false);
        }
    }
}