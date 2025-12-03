using UnityEngine;

public class Enemy_Range : Enemy
{
    public Transform weaponHolder;
    public Enemy_RangeWeaponType weaponType;
    
    public float fireRate = 1; // bullet per second
    public GameObject bulletPrefab;
    public Transform gunPoint;
    public float bulletSpeed = 20;
    public float bulletToShoot = 5; // Bullet to shoot before weapon goes on cooldown
    public float weaponCooldown = 1.5f; // Weapon cooldown after all bullets shoot
    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        visuals.SetupLook();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.UpdateState();
    }

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");
        Vector3 bulletsDirection = ((player.position + Vector3.up) - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(bulletsDirection);
        
        newBullet.GetComponent<Enemy_Bullet>().BulletSetup();
        
        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        rbNewBullet.mass = 20 / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
        Debug.Log("Velocity: " + rbNewBullet.velocity);
        Debug.Log("AngularVe: " +  rbNewBullet.angularVelocity);
        Debug.Log("Max velo" + rbNewBullet.maxLinearVelocity);

    }

    public override void EnterBattleMode()
    {
        if(inBattleMode) return;
        base.EnterBattleMode();
        stateMachine.ChangeState(battleState);
    }
}