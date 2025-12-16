using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void EnterState()
    {
        base.EnterState();
        // stateTimer is time change state
        stateTimer = enemy.flameThrowDuration;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        
        enemy.bossVisuals.EnableWeaponTrail(true);
        
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        enemy.FaceTarget(enemy.player.position);
        
        if(ShouldDisableFlameThrower())
            DisableFlameThrower();
        
        if(triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    private bool ShouldDisableFlameThrower() => stateTimer <= 0;

    public override void ExitState()
    {
        base.ExitState();
        enemy.SetAbilityOnCooldown();
        enemy.bossVisuals.ResetBatteries();
    }

    public void DisableFlameThrower()
    {
        if(enemy.bossWeaponType != BossWeaponType.Flamethrower) return;
        if(enemy.flameThrowActive == false)
            return;
        
        enemy.ActivateFlameThrower(false);
    }
    
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (enemy.bossWeaponType == BossWeaponType.Flamethrower)
        {
            enemy.ActivateFlameThrower(true);
            enemy.bossVisuals.DisChargeBatteries();
            enemy.bossVisuals.EnableWeaponTrail(false);
        }

        if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {
            enemy.ActivateHummer();
        }
        
    }
}