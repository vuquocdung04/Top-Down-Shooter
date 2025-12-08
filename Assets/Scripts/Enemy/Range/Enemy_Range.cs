using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk
{
    Unavailable = 0,
    CanTakeCover = 1,
    CanTakeAndChangeCover = 2,
}

public enum UnstoppablePerk
{
    Unavailable = 0,
    Unstoppable = 1,
}

public enum GrenadePerk
{
    Unavailable = 0,
    CanThrowGrenade = 1,
}
public class Enemy_Range : Enemy
{
    [Header("Enemy Perk")]
    public CoverPerk coverPerk;
    public UnstoppablePerk unstoppablePerk;
    public GrenadePerk grenadePerk;

    [Header("Grenade Perk")] public float grenadeCooldown;
    private float lastTimeGrenadeThrown = -10;

    [Header("Advance perk")] public float advanceSpeed;
    public float advanceStoppingDistance;
    public float advanceDuration = 2.5f;

    [Header("Cover systems")] public float minCoverTime;
    public float safeDistance;
    public CoverPoint lastCover { get; private set; }
    public CoverPoint currentCover { get; private set; }

    [Header("Weapon Details")] public float attackDelay; // attack delay for unstoppable state
    public Enemy_RangeWeaponData weaponData;
    public Enemy_RangeWeaponType weaponType;
    
    [Space(5)] private Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [Header("Aim details")]
    public float slowAim = 4; // slowAim is enemy's reaction when it sees the player again
    public float fastAim = 20; // fastAim is enemy's reaction until player is out of sight
    public Transform aim;
    public Transform playersBody;
    public LayerMask whatToIgnore;
    
    [SerializeField] private List<Enemy_RangeWeaponData> availableWeaponDatas;

    #region States
    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_Range runToCoverState { get; private set; }
    public AdvancePlayerState_Range advancePlayerState { get; private set; }
    
    public ThrowGrenadeState_Range  throwGrenadeState { get; private set; }
    
    #endregion
    

    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
        throwGrenadeState =  new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
    }

    protected override void Start()
    {
        base.Start();

        playersBody = player.GetComponent<Player>().playerBody;
        aim.parent = null;
        
        InitializePerk();
        
        stateMachine.Initialize(idleState);
        visuals.SetupLook();
        SetupWeapon();
    }
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.UpdateState();
    }

    public bool CanThrowGrenade()
    {
        if(grenadePerk == GrenadePerk.Unavailable) return false;
        
        if(Vector3.Distance(player.transform.position, transform.position) < safeDistance) return false;
        
        if(Time.time > grenadeCooldown + lastTimeGrenadeThrown) return true;
        
        return false;
    }

    public void ThrowGrenade()
    {
        lastTimeGrenadeThrown = Time.time;
    }
    protected override void InitializePerk()
    {
        base.InitializePerk();
        if (IsUnstoppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("AdvanceAnimIndex",1); // 1 is a slow walk
        }
    }

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");
        Vector3 bulletsDirection = (aim.position - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(bulletsDirection);

        newBullet.GetComponent<Enemy_Bullet>().BulletSetup();

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.velocity = bulletDirectionWithSpread * weaponData.bulletSpeed;
    }
    public override void EnterBattleMode()
    {
        if (inBattleMode) return;
        base.EnterBattleMode();

        if (CanGetCover())
            stateMachine.ChangeState(runToCoverState);
        else
            stateMachine.ChangeState(battleState);
    }
    private void SetupWeapon()
    {
        List<Enemy_RangeWeaponData> filteredData = new();
        foreach (var data in availableWeaponDatas)
        {
            if (data.weaponType == weaponType)
                filteredData.Add(data);
        }

        if (filteredData.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredData.Count);
            weaponData = filteredData[randomIndex];
        }
        else
            Debug.Log("No available weapon data was found!");

        gunPoint = visuals.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;
    }

    #region Enemy's aim region
    public void UpdateAimPosition()
    {
        float aimSpeed = IsAimOnPlayer() ? fastAim : slowAim;
        aim.position = Vector3.MoveTowards(aim.position, playersBody.position, aimSpeed * Time.deltaTime);
    }
    public bool IsAimOnPlayer()
    {
        float distanceAimToPlayer = Vector3.Distance(aim.position, player.position);
        return distanceAimToPlayer < 2;
    }

    public bool IsSeeingPlayer()
    {
        Vector3 myPosition = transform.position + Vector3.up;
        Vector3 directionToPlayer = playersBody.position - myPosition;

        // ~ tuong ung ! 
        if (Physics.Raycast(myPosition, directionToPlayer, out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform == player)
            {
                Debug.Log("Player is seeing");
                UpdateAimPosition();
                return true;
            }
        }
        return false;
    }
    #endregion
    
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (playersBody == null) return;
        
        Vector3 myPosition = transform.position + Vector3.up;
        Vector3 directionToPlayer = playersBody.position - myPosition;
        
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawRay(myPosition, directionToPlayer.normalized * 100f);
    }

    #region Cover System

    public bool CanGetCover()
    {
        if (coverPerk == CoverPerk.Unavailable)
            return false;
        currentCover = AttemptToFindCover()?.GetComponent<CoverPoint>();

        if (lastCover != currentCover && currentCover != null)
            return true;
        Debug.LogWarning("No cover found!");
        return false;
        
    }
    private Transform AttemptToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new();

        foreach (Cover cover in CollectNearByCover())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }
        CoverPoint closestCoverPoint = null;
        float shortestDistance =  float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if (currentDistance < shortestDistance)
            {
                closestCoverPoint = coverPoint;
                shortestDistance = currentDistance;
            }
        }

        if (closestCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;
            
            currentCover = closestCoverPoint;
            currentCover.SetOccupied(true);
            
            return currentCover.transform;
        }
        return null;
    }
    
    private List<Cover> CollectNearByCover()
    {
        float coverRadiusCheck = 30;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRadiusCheck);
        List<Cover> collectedCovers = new();

        foreach (var col in hitColliders)
        {
            Cover cover = col.GetComponent<Cover>();
            if(cover != null && !collectedCovers.Contains(cover))
                collectedCovers.Add(cover);
        }
        return collectedCovers;
    }

    #endregion

    public bool IsUnstoppable() => unstoppablePerk == UnstoppablePerk.Unstoppable;

}