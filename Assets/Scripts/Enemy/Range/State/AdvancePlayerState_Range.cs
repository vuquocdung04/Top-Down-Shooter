using UnityEngine;

public class AdvancePlayerState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 playerPosition;
    public AdvancePlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.visuals.EnableIk(true,false);
        
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.advanceSpeed;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        playerPosition = enemy.player.position;
        
        enemy.agent.SetDestination(playerPosition);
        enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(enemy.transform.position, playerPosition) < enemy.advanceStoppingDistance)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
    
    
}