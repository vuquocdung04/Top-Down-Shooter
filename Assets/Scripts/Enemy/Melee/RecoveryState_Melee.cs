using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
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
        enemy.transform.rotation = enemy.FaceTarget(enemy.player.position);
        if(triggerCalled)
            Debug.Log("Im supposed to be triggered");
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}