using UnityEngine;

public class MoveState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 destination;
    
    public MoveState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.agent.speed = enemy.walkSpeed;

        enemy.visuals.EnableIk(false,false);
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