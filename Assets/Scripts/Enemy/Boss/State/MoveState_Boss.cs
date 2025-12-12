using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 destination;
    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.agent.speed = enemy.walkSpeed;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.FaceTarget(GetNextPathPoint());
        
        if(enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.5f)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}