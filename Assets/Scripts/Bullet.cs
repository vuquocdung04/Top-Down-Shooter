using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;

    private Vector3 startPosition;
    private float flyDistance;

    private bool bulletDisabled;

    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float fly_distance = 100, float impact_force = 100)
    {
        impactForce = impact_force;
        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;
        
        trailRenderer.Clear();
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        flyDistance =
            fly_distance +
            0.5f; // magic number 0.5f is a length of tip of the laser (Check method UpdateVisuals on PlayerAim)
    }

    protected virtual void Update()
    {
        FadeTrailIfNeeded();

        DisableBulletIfNeeded();

        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    protected void ReturnBulletToPool()
    {
        ObjectPool.instance.ReturnObject(gameObject);
    }

    protected void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        Enemy enemy = other.transform.GetComponentInParent<Enemy>();
        Enemy_Shield_Obj shieldObj = other.gameObject.GetComponent<Enemy_Shield_Obj>();
        if (shieldObj)
        {
            shieldObj.ReduceDurability();
            return;
        }

        if (enemy)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidbody = other.collider.attachedRigidbody;
            enemy.GetHit();
            enemy.DeathImpact(force, other.contacts[0].point, hitRigidbody);
        }

        CreateImpactFX();
        ReturnBulletToPool();
    }

    protected void CreateImpactFX()
    {
        GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX, transform);
        ObjectPool.instance.ReturnObject(newImpactFX, 1);
    }
}