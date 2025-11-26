using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;
    protected Animator anim;

    protected string animBoolName;
    protected float stateTimer;

    protected bool triggerCalled;

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void EnterState()
    {
        enemyBase.anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void UpdateState()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void ExitState()
    {
        enemyBase.anim.SetBool(animBoolName,false);
    }
    
    public void AnimationTrigger() => triggerCalled = true;
    
    protected Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent;
        NavMeshPath path = agent.path;

        if (path.corners.Length < 2)
            return agent.destination;
        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1f)
                return path.corners[i + 1];
        }
        return Vector3.zero;
    }
}