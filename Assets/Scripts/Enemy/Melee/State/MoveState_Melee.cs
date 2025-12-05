using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;
    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
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
        
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.05f)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("I exit move state");
    }


}