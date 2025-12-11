using UnityEngine;

public class IdleState_Range : EnemyState
{
    private Enemy_Range enemy;

    public IdleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();

        enemy.anim.SetFloat("IdleAnimIndex", Random.Range(0, 3));

        enemy.visuals.EnableIk(true, false);
        if (enemy.weaponType == Enemy_RangeWeaponType.Pistol)
            enemy.visuals.EnableIk(false, false);

        stateTimer = enemy.idleTime;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}