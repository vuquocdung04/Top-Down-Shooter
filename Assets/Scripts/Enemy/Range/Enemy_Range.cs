using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [Header("Enemy Perk")] public CoverPerk coverPerk;
    public UnstoppablePerk unstoppablePerk;
    public GrenadePerk grenadePerk;

    [Header("Grenade Perk")] public int grenadeDamage;
    public GameObject grenadePrefab;
    public float impactPower;
    public float explosionTimer = 1;
    public float timeToTarget = 1.2f;
    public float grenadeCooldown;
    private float lastTimeGrenadeThrown = -10;
    [SerializeField] private Transform grenadePoint;

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

    [Header("Aim details")] public float slowAim = 4; // slowAim is enemy's reaction when it sees the player again
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

    public ThrowGrenadeState_Range throwGrenadeState { get; private set; }

    public DeadState_Range deadState { get; private set; }

    #endregion


    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
        throwGrenadeState = new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
        deadState = new DeadState_Range(this, stateMachine, "Idle"); // idle is a place holder, we using ragdoll
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

    public override void Die()
    {
        base.Die();
        
        if(stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    public bool CanThrowGrenade()
    {
        if (grenadePerk == GrenadePerk.Unavailable) return false;

        if (Vector3.Distance(player.transform.position, transform.position) < safeDistance) return false;

        if (Time.time > grenadeCooldown + lastTimeGrenadeThrown) return true;

        return false;
    }

    public void ThrowGrenade()
    {
        lastTimeGrenadeThrown = Time.time;
        visuals.EnableGrenadeModel(false);

        GameObject newGrenade = ObjectPool.instance.GetObject(grenadePrefab,grenadePoint);
        
        Enemy_Grenade newGrenadeScript = newGrenade.GetComponent<Enemy_Grenade>();

        if (stateMachine.currentState == deadState)
        {
            newGrenadeScript.SetupGrenade(whatIsAlly,transform.position, 1, explosionTimer, impactPower, grenadeDamage);
            return;
        }

        newGrenadeScript.SetupGrenade(whatIsAlly,player.transform.position, timeToTarget, explosionTimer, impactPower, grenadeDamage);
    }

    protected override void InitializePerk()
    {
        base.InitializePerk();

        if (weaponType == Enemy_RangeWeaponType.Random)
        {
            ChooseRandomWeaponType();
        }
        if (IsUnstoppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("AdvanceAnimIndex", 1); // 1 is a slow walk
        }
    }

    private void ChooseRandomWeaponType()
    {
        List<Enemy_RangeWeaponType> validTypes = new();

        foreach (Enemy_RangeWeaponType value in System.Enum.GetValues(typeof(Enemy_RangeWeaponType)))
        {
            if (value != Enemy_RangeWeaponType.Random && value != Enemy_RangeWeaponType.Rifle)
                validTypes.Add(value);
        }
            
        int randomIndex = Random.Range(0,validTypes.Count);
        weaponType = validTypes[randomIndex];
    }

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");
        Vector3 bulletsDirection = (aim.position - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab,gunPoint);
        
        newBullet.transform.rotation = Quaternion.LookRotation(bulletsDirection);

        newBullet.GetComponent<Bullet>().BulletSetup(whatIsAlly, weaponData.bulletDamage);

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
            if (hit.transform.root == player.root)
            {
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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(aim.position, advanceStoppingDistance);
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
        float shortestDistance = float.MaxValue;

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
            if (cover != null && !collectedCovers.Contains(cover))
                collectedCovers.Add(cover);
        }

        return collectedCovers;
    }

    #endregion

    public bool IsUnstoppable() => unstoppablePerk == UnstoppablePerk.Unstoppable;
}