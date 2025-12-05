using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShoot = -10f;
    private int bulletsShoot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;
    
    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();

        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
        
        enemy.visuals.EnableIk(true,true);
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
        enemy.visuals.EnableIk(false,false);
    }
    private void AttemptToResetWeapon()
    {
        bulletsShoot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
    }

    private bool WeaponOnCoolDown() => Time.time > lastTimeShoot + weaponCooldown;

    private bool WeaponOutOfBullets() => bulletsShoot >= bulletsPerAttack;
    private bool CanShoot() => Time.time > lastTimeShoot + 1 / enemy.weaponData.fireRate;

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShoot =  Time.time;
        bulletsShoot++;
    }

    
    
}