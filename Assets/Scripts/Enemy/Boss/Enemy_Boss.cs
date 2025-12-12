using UnityEngine;

public class Enemy_Boss : Enemy
{
    public float attackRange;

    [Header("Jump Attack")] public float travelTimeToTarget = 1;
    public float jumpAttackCooldown = 10;
    private float lastTimeJumped = -10f;
    public float minJumpDistanceRequired;
    
    [Space] [SerializeField] private LayerMask whatToIgnore;
    
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState {get; private set;}
    public JumpAttackState_Boss  jumpAttackState { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.UpdateState();
        
    }

    public override void EnterBattleMode()
    {
        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if(distanceToPlayer < minJumpDistanceRequired)
            return false;
        
        if (Time.time > lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight())
        {
            lastTimeJumped = Time.time;
            return true;
        }
        return false;
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 myPos = transform.position + new Vector3(0,1.5f,0);
        Vector3 playerPos = player.position + Vector3.up;
        
        Vector3 directionToPlayer = (playerPos - myPos).normalized;

        if (Physics.Raycast(myPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            if(hit.transform == player || hit.transform.parent == player)
                return true;
        }

        return false;
    }
    
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;
    
    
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Vector3 myPos = transform.position + new Vector3(0,1.5f,0);
            Vector3 playerPos = player.position + Vector3.up;
            Gizmos.color = Color.magenta;
            
            Gizmos.DrawLine(myPos, playerPos);
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);
    }
}