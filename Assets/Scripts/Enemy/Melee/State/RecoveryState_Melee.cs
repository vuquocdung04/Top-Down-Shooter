using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.agent.isStopped = true;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.FaceTarget(enemy.player.position);
        if (triggerCalled)
        {
            if (enemy.CanThrowAxe())
                stateMachine.ChangeState(enemy.abilityState);
            else if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.attackState);
            else
                stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}