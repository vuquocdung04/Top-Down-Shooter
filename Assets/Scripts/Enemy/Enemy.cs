using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected int heathPoints = 20;
    
    [Header("Idle Data")] public float idleTime;
    public float aggressionRange;
    
    [Header("Move Data")] public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;
    
    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointPositions;
    private int currentPatrolIndex;
    public bool inBattleMode { get; private set; }
    
    public Transform player { get; private set; }

    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
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

    protected bool ShouldEnterBattleMode()
    {
        bool inAggresionRange = Vector3.Distance(transform.position, player.position) < aggressionRange;
        if (inAggresionRange && !inBattleMode)
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
        EnterBattleMode();
        heathPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }
    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    public void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero)
        {
            return;
        }
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }
    
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