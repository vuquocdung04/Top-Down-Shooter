using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    public Enemy_Melee enemy { get; private set; }
    private Vector3 attackDirection;

    private float attackMoveSpeed;

    public const float MAX_ATTACK_DISTANCE = 50f;

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.UpdateAttackData();
        enemy.EnableWeaponModel(true);
        enemy.visuals.EnableWeaponTrail(true);
        
        attackMoveSpeed = enemy.enemyMeleeAttackData.moveSpeed;
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.enemyMeleeAttackData.animationSpeed);
        enemy.anim.SetFloat("AttackIndex", enemy.enemyMeleeAttackData.attackIndex);
        enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0,6));
        

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (enemy.ManualRotationActive())
        {
            enemy.FaceTarget(enemy.player.position);
            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }
        
        if (enemy.ManualMovementActive())
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection,
                attackMoveSpeed * Time.deltaTime);

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.recoveryState);
            else
                stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        SetupNextAttack();
        
        enemy.visuals.EnableWeaponTrail(false);
    }

    private void SetupNextAttack()
    {
        int recoveryIndex = PlayerClose() ? 1 : 0;
        enemy.anim.SetFloat("RecoveryIndex",recoveryIndex);

        enemy.enemyMeleeAttackData = UpdatedAttackData();
    }

    private bool PlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.position) <= 1;

    private Enemy_MeleeAttackData UpdatedAttackData()
    {
        List<Enemy_MeleeAttackData> validAttacks = new List<Enemy_MeleeAttackData>(enemy.attackList);

        if (PlayerClose())
            validAttacks.RemoveAll(parameter => parameter.attackType == AttackType_Melee.Charge);
        int rand = Random.Range(0, validAttacks.Count);
        return validAttacks[rand];
    }
}