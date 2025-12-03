using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShoot = -10f;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.FaceTarget(enemy.player.position);
        if (Time.time > lastTimeShoot + 1 / enemy.fireRate)
        {
            enemy.FireSingleBullet();
            lastTimeShoot =  Time.time;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
    
    
}