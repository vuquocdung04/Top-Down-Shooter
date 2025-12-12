using UnityEngine;

public class JumpAttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 lastPlayerPos;
    private float jumpAttackMovementSpeed;

    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }


    public override void EnterState()
    {
        base.EnterState();
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector2.zero;
        
        lastPlayerPos = enemy.player.position;

        float distanceToPlayer = Vector3.Distance(lastPlayerPos, enemy.transform.position);
        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget; // v = s/t
        
        enemy.FaceTarget(lastPlayerPos, 1000);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        Vector3 myPos = enemy.transform.position;
        enemy.agent.enabled = !enemy.ManualMovementActive(); // fake behavior jump
        
        if (enemy.ManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(myPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime);
            
        }

        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}