using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShoot = -10f;
    private int bulletsShoot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    private float coverCheckTimer;
    
    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void EnterState()
    {
        base.EnterState();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
        
        enemy.visuals.EnableIk(true,true);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        //if player in aggression range  = false
        // change state to advance player state
        if (!enemy.IsPlayerInAggressionRange())
        {
            stateMachine.ChangeState(enemy.AdvancePlayerState);
        }
        ChangeCoverIfShould();
        
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

    private void ChangeCoverIfShould()
    {
        if(enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
            return;
        
        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = 0.5f;
            if (IsPlayInClearSight() || IsPlayerClose())
            {
                if(enemy.CanGetCover())
                    stateMachine.ChangeState(enemy.runToCoverState);
            }
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.visuals.EnableIk(false,false);
    }

    #region Cover system region

    private bool IsPlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.position) < enemy.safeDistance;

    private bool IsPlayInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.position - enemy.transform.position;

        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            return hit.collider.gameObject.GetComponentInParent<Player>();
        }

        return false;
    }

    #endregion
    
    
    #region Weapons region
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
    #endregion
    
}