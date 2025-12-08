using UnityEngine;

public class AdvancePlayerState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 playerPosition;
    
    public float lastTimeAdvanced { get; private set; }
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

        if (enemy.IsUnstoppable())
        {
            enemy.visuals.EnableIk(true,false);
            stateTimer = enemy.advanceDuration;
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        playerPosition = enemy.player.position;
        enemy.UpdateAimPosition();
        
        enemy.agent.SetDestination(playerPosition);
        enemy.FaceTarget(GetNextPathPoint());

        if (CanEnterBattleState() && enemy.IsSeeingPlayer())
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void ExitState()
    {
        base.ExitState();
        lastTimeAdvanced = Time.time;
    }

    private bool CanEnterBattleState()
    {
        bool closeEnoughToPlayer =
            Vector3.Distance(enemy.transform.position, playerPosition) < enemy.advanceStoppingDistance;
        if(enemy.IsUnstoppable())
            return closeEnoughToPlayer || stateTimer < 0;
        else
            return closeEnoughToPlayer;
    }
}