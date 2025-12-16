using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 destination;
    private float actionTimer;
    private float timeBeforeSpeedUp = 15;
    private bool speedUpActivate;

    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void EnterState()
    {
        base.EnterState();

        SpeedReset();
        enemy.agent.isStopped = false;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);

        actionTimer = enemy.actionCooldown;
    }


    public override void UpdateState()
    {
        base.UpdateState();
        actionTimer -= Time.deltaTime;
        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.inBattleMode)
        {
            if (ShouldSpeedUp())
            {
                SpeedUp();
            }

            Vector3 playerPos = enemy.player.position;
            enemy.agent.SetDestination(playerPos);

            if (actionTimer < 0)
            {
                PerformRandomAction();
            }
            else if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.attackState);
        }
        else if (Vector3.Distance(enemy.transform.position, destination) < 0.25f)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void SpeedUp()
    {
        enemy.agent.speed = enemy.runSpeed;
        enemy.anim.SetFloat("MoveAnimIndex", 1); // 1 is run anim
        speedUpActivate = true;
    }

    private void SpeedReset()
    {
        speedUpActivate = false;
        enemy.anim.SetFloat("MoveAnimIndex", 0); // 0 is walk anim
        enemy.agent.speed = enemy.walkSpeed;
    }

    private void PerformRandomAction()
    {
        actionTimer = enemy.actionCooldown;
        if (Random.Range(0, 2) == 0)
        {
            TryAbility();
        }
        else
        {
            if (enemy.CanDoJumpAttack())
                stateMachine.ChangeState(enemy.jumpAttackState);
            else if (enemy.bossWeaponType == BossWeaponType.Hummer)
                TryAbility();
        }
    }

    private void TryAbility()
    {
        if (enemy.CanDoAbility())
            stateMachine.ChangeState(enemy.abilityState);
    }

    // Check: boss will speed up if it do not attack player of lastTimeAttacked + timeBeforeSpeedUp
    // Boss will reset to walk animation when RE-ENTERING MoveState
    private bool ShouldSpeedUp()
    {
        if (speedUpActivate)
            return false;
        if (Time.time > enemy.attackState.lastTimeAttacked + timeBeforeSpeedUp)
        {
            return true;
        }

        return false;
    }
}