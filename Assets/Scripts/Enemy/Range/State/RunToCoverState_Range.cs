using UnityEngine;

public class RunToCoverState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 destination;
    public RunToCoverState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.visuals.EnableIk(true,false);
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.runSpeed;

        destination = enemy.AttemptToFindCover().position;
        enemy.agent.SetDestination(destination);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(enemy.transform.position, destination) < 0.5f)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}