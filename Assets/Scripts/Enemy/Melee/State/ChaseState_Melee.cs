using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private float lastTimeUpdatedDistination;
    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.chaseSpeed;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if(enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);
        
        enemy.transform.rotation = enemy.FaceTarget(GetNextPathPoint());
        
        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDistination + 0.25f)
        {
            lastTimeUpdatedDistination =  Time.time;
            return true;
        }
        return false;
    }
}