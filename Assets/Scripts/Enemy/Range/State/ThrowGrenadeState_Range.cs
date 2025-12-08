public class ThrowGrenadeState_Range : EnemyState
{
    private Enemy_Range enemy;
    public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        enemy.FaceTarget(enemy.player.position);
        
        if(triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        enemy.ThrowGrenade();
    }
}