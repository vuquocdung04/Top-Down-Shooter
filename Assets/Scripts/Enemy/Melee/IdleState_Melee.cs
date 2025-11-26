using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void EnterState()
    {
        base.EnterState();
        stateTimer = enemyBase.idleTime;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (stateTimer <= 0)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}