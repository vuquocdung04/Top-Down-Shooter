using UnityEngine;

public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.agent.isStopped = true;
        enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 2)); // we have two attack
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.idleState);
            else
                stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}