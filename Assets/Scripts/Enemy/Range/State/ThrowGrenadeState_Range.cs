public class ThrowGrenadeState_Range : EnemyState
{
    private Enemy_Range enemy;
    public bool finishedThrowingGrenade { get; private set; } = true;

    public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();
        
        finishedThrowingGrenade = false;
        
        enemy.visuals.EnableWeaponModel(false);
        enemy.visuals.EnableIk(false,false);
        enemy.visuals.EnableSecondaryWeaponModel(true);
        enemy.visuals.EnableGrenadeModel(true);
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
        enemy.visuals.EnableWeaponModel(true);
        enemy.visuals.EnableSecondaryWeaponModel(false);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        finishedThrowingGrenade = true;
        enemy.ThrowGrenade();
    }
}