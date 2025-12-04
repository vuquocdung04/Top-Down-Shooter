using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShoot = -10f;
    private int bulletsShoot = 0;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.visuals.EnableIk(true);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.FaceTarget(enemy.player.position);
        if (WeaponOutOfBullets())
        {
            if (WeaponOnCoolDown())
                AttemptToResetWeapon();
            return;
        }
        
        if (CanShoot())
        {
            Shoot();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.visuals.EnableIk(false);
    }
    private void AttemptToResetWeapon() => bulletsShoot = 0;

    private bool WeaponOnCoolDown() => Time.time > lastTimeShoot + enemy.weaponCooldown;

    private bool WeaponOutOfBullets() => bulletsShoot >= enemy.bulletToShoot;
    private bool CanShoot() => Time.time > lastTimeShoot + 1 / enemy.fireRate;

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShoot =  Time.time;
        bulletsShoot++;
    }

    
    
}