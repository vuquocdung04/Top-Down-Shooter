using UnityEngine;

public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    public float lastTimeAttacked { get; private set;}
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

        stateTimer = 1f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        if(stateTimer > 0)
            enemy.FaceTarget(enemy.player.position,20f);

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
        lastTimeAttacked = Time.time;
    }
}