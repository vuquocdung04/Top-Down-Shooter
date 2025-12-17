using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour
{
    public LayerMask whatIsAlly;
    public LayerMask whatIsPlayer;
    [Space]
    public int heathPoints = 20;

    [Header("Idle Data")] public float idleTime;
    public float aggressionRange;

    [Header("Move Data")] public float walkSpeed = 1.5f;
    public float runSpeed = 3;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointPositions;
    private int currentPatrolIndex;
    public bool inBattleMode { get; private set; }
    protected bool isMeleeAttackReady;
    
    
    public Transform player { get; private set; }

    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }
    public Enemy_Visuals visuals { get; private set; }

    public Ragdoll ragdoll { get; private set; }

    public Enemy_Health health { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        health = GetComponent<Enemy_Health>();
        ragdoll = GetComponent<Ragdoll>();
        visuals = GetComponent<Enemy_Visuals>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    protected virtual void InitializePerk()
    {
    }

    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAggressionRange() && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit()
    {
        health.ReduceHealth();
        if (health.ShouldDie())
            Die();

        EnterBattleMode();
    }

    public virtual void Die()
    {
    }

    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject fx)
    {
        if (isMeleeAttackReady == false) return;
        foreach (var attackPoint in damagePoints)
        {
            Collider[] detectedHits =
                Physics.OverlapSphere(attackPoint.position, attackCheckRadius, whatIsPlayer);
            
            for (int i = 0; i < detectedHits.Length; i++)
            {
                IDamageable damageable = detectedHits[i].GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage();
                    isMeleeAttackReady = false;
                    GameObject newAttackFx = ObjectPool.instance.GetObject(fx, attackPoint);
                    ObjectPool.instance.ReturnObject(newAttackFx,1);
                    return;
                }
            }
        }
    }
    
    public void EnableMeleeAttackCheck(bool enable) => isMeleeAttackReady = enable;
    
    public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        if (health.ShouldDie())
            StartCoroutine(BulletImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator BulletImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target, float turnSpeed = 0f)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;

        if (turnSpeed == 0)
            turnSpeed = this.turnSpeed;

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    public bool IsPlayerInAggressionRange() => Vector3.Distance(transform.position, player.position) < aggressionRange;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
    }

    #region Animation Events

    public void ActivateManualMovement(bool state) => manualMovement = state;
    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotation(bool state) => manualRotation = state;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    #endregion

    #region Patrol Logic

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointPositions[currentPatrolIndex];
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointPositions = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointPositions[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    #endregion
}