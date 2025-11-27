using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Idle Data")] public float idleTime;
    public float aggressionRange;
    
    [Header("Move Data")] public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;
    
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;
    
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

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    protected virtual void Update()
    {
         
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
    }

    public void ActivateManualMovement(bool state) => manualMovement = state; 
    public bool ManualMovementActive() => manualMovement;
    
    public void ActivateManualRotation(bool state) => manualRotation = state;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    public bool PlayerInAggressionRange() => Vector3.Distance(transform.position, player.position) < aggressionRange;
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;
        return destination;
    }
    public Quaternion FaceTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero)
            return transform.rotation;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        return Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }
}