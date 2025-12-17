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

    private LayerMask allyLayerMask;
    
    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(LayerMask allyLayer, float fly_distance = 100, float impact_force = 100)
    {
        impactForce = impact_force;
        allyLayerMask = allyLayer;
        
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

    protected void ReturnBulletToPool(float delay = 0)
    {
        ObjectPool.instance.ReturnObject(gameObject, delay);
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
        // ban nham dong doi
        if (FriendlyFire() == false)
        {
            // other.gameObject.layer == LayerMask.NameToLayer("Ally") kieu vậy, cach duoi toi uu hon " bitwise "
            // use a bitwise and to check if the collision layer is in the allyLayerMask
            if ((allyLayerMask.value & (1 << other.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(10);
                return;
            }
        }
        
        CreateImpactFX();
        ReturnBulletToPool();
        
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage();

        ApplyBulletImpactToEnemy(other);
    }

    private void ApplyBulletImpactToEnemy(Collision other)
    {
        Enemy enemy = other.transform.GetComponentInParent<Enemy>();
        if (enemy)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidbody = other.collider.attachedRigidbody;
            enemy.BulletImpact(force, other.contacts[0].point, hitRigidbody);
        }
    }

    protected void CreateImpactFX()
    {
        GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX, transform);
        ObjectPool.instance.ReturnObject(newImpactFX, 1);
    }
    
    public bool FriendlyFire() => GameManager.instance.friendlyFire;
}