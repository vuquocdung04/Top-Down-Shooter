using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AttackData_EnemyMelee
{
    public string attackName;
    public int attackDamage;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 2)] public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee
{
    Close = 0,
    Charge = 1,
}

public enum EnemyMelee_Type
{
    Regular = 0,
    Shield = 1,
    Dodge = 2,
    AxeThrow = 3,
}

public class Enemy_Melee : Enemy
{
    public Enemy_MeleeSFX meleeSFX { get; private set; }
    #region States

    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    private DeadState_Melee deadState { get; set; }
    public AbilityState_Melee abilityState { get; private set; }

    #endregion

    [Header("Enemy Melee Type")] public EnemyMelee_Type meleeType;
    public Enemy_MeleeWeaponType weaponType;

    [Header("Shield")] public int shieldDurability;
    public Transform shieldTransform;
    [Header("Dodge")] public float dodgeCooldown;
    private float lastTimeDodge = -10;

    [Header("Axe throw ability")] public int axeDamage;
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float animTimer;
    public float axeThrowCooldown;
    private float lastTimeAxeThrow;
    public Transform axeStartPoint;

    [Header("Attack Data")] public AttackData_EnemyMelee attackDataEnemyMelee;
    public List<AttackData_EnemyMelee> attackList;
    public Enemy_WeaponModel currentWeapon;
    
    [Space] [SerializeField] private GameObject meleeAttackFx;

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine,
            "Idle"); // Idle anim is just a place holder, we use dragdoll
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");

        meleeSFX = GetComponent<Enemy_MeleeSFX>();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        InitializePerk();
        visuals.SetupLook();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.UpdateState();

        MeleeAttackCheck(currentWeapon.damagePoints, currentWeapon.attackRadius, meleeAttackFx,attackDataEnemyMelee.attackDamage);
    }
    
    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;
        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        walkSpeed = walkSpeed * 0.6f;
        visuals.EnableWeaponModel(false);
    }

    public void UpdateAttackData()
    {
        currentWeapon = visuals.currentWeaponModel.GetComponent<Enemy_WeaponModel>();
        if (currentWeapon.weaponData != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackDatas);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    protected override void InitializePerk()
    {
        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            weaponType = Enemy_MeleeWeaponType.Throw;
        }

        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            weaponType = Enemy_MeleeWeaponType.OneHand;
        }

        if (meleeType == EnemyMelee_Type.Dodge)
        {
            weaponType = Enemy_MeleeWeaponType.Unarmed;
        }
    }

    public override void Die()
    {
        base.Die();
        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    public void ActivateDodgeRoll()
    {
        if (meleeType != EnemyMelee_Type.Dodge) return;

        if (stateMachine.currentState != chaseState) return;

        if (Vector3.Distance(transform.position, player.position) < 1.8f)
            return;
        float dodgeAnimationDuration = GetAnimationClipDuration("Sprinting Forward Roll");

        if (Time.time > dodgeCooldown + lastTimeDodge + dodgeAnimationDuration)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("Dodge");
        }
    }

    public void ThrowAxe()
    {
        GameObject newAxe = ObjectPool.instance.GetObject(axePrefab, axeStartPoint);

        newAxe.GetComponent<Enemy_Axe_Obj>().AxeSetup(axeFlySpeed, player, animTimer,axeDamage);
    }

    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow) return false;
        if (Time.time > lastTimeAxeThrow + axeThrowCooldown)
        {
            lastTimeAxeThrow = Time.time;
            return true;
        }

        return false;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (var t in clips)
            if (clipName == t.name)
                return t.length;
        Debug.Log(clipName + " animation not found!");
        return 0;
    }

    public bool PlayerInAttackRange() =>
        Vector3.Distance(transform.position, player.position) < attackDataEnemyMelee.attackRange;


    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDataEnemyMelee.attackRange);
    }
}