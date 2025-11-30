public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        if(triggerCalled)
            stateMachine.ChangeState(enemy.recoveryState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}