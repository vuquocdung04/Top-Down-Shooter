public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private bool interactionDisabled;
    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void EnterState()
    {
        base.EnterState();
        interactionDisabled = false;
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