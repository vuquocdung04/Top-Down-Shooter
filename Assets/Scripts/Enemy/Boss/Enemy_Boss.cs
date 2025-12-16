using UnityEngine;

public enum BossWeaponType
{
    Flamethrower = 0,
    Hummer = 1,
}

public class Enemy_Boss : Enemy
{
    [Header("Boss Details")]
    public BossWeaponType bossWeaponType;
    public float actionCooldown = 10;
    public float attackRange;

    [Header("Ability")] public float minAbilityDistance;
    public float abilityCooldown;
    private float lastTimeUsedAbility;
    
    [Header("Flamethrower")]
    public ParticleSystem flameThrower;
    public float flameThrowDuration = 10;
    public bool flameThrowActive { get; private set; }
    
    [Header("Hummer")]
    public GameObject activationPrefab;

    
    
    [Header("Jump Attack")]
    [Space]
    public float travelTimeToTarget = 1;
    public float jumpAttackCooldown = 10;
    private float lastTimeJumped = -10f;
    public float minJumpDistanceRequired;
    [Space]
    public float impactRadius = 2.5f;
    public float impactPower = 5;
    public Transform impactPoint;
    [SerializeField] private float upforceMultiplier = 10; // chi anh huong toi truc y
    
    [Space] [SerializeField] private LayerMask whatToIgnore;
    
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState {get; private set;}
    public JumpAttackState_Boss  jumpAttackState { get; private set; }
    public AbilityState_Boss  abilityState { get; private set; }
    public DeadState_Boss  deadState { get; private set; }
    
    public Enemy_BossVisuals bossVisuals { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();

        bossVisuals = GetComponent<Enemy_BossVisuals>();
        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle"); // Idle is just a placeholder we use ragdoll
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

    public override void GetHit()
    {
        base.GetHit();
        if(heathPoints <= 0 && stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    public override void EnterBattleMode()
    {
        if(inBattleMode)return;
        
        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public void ActivateFlameThrower(bool activate)
    {
        flameThrowActive = activate;
        if (!activate)
        {
            flameThrower.Stop();
            anim.SetTrigger("StopFlamethrower");
            return;
        }

        var mainModule = flameThrower.main;
        var extraModule = flameThrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        
        mainModule.duration = flameThrowDuration;
        extraModule.duration = flameThrowDuration;
        
        flameThrower.Clear();
        flameThrower.Play();
    }

    public void ActivateHummer()
    {
        GameObject newActivation = ObjectPool.instance.GetObject(activationPrefab, impactPoint);
        ObjectPool.instance.ReturnObject(newActivation,1);
    }
    
    public bool CanDoAbility()
    {
        bool playerWithinDistance = Vector3.Distance(transform.position,player.position) < minAbilityDistance;
        
        if(playerWithinDistance == false) return false;
        return Time.time > lastTimeUsedAbility + abilityCooldown;
    }

    public void SetAbilityOnCooldown() => lastTimeUsedAbility = Time.time;


    public void JumpImpact()
    {
        Transform imPoint = this.impactPoint;
        if (imPoint == null)
            imPoint = transform;
        
        Collider[] colliders = Physics.OverlapSphere(imPoint.position, impactRadius);

        foreach (var col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if(rb != null)
                rb.AddExplosionForce(impactPower, transform.position, impactRadius, upforceMultiplier, ForceMode.Impulse);
        }
    }
    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if(distanceToPlayer < minJumpDistanceRequired)
            return false;
        
        return Time.time > lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight();
    }
 
    public void SetJumpAttackCooldown() => lastTimeJumped = Time.time;
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
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);
    }
}