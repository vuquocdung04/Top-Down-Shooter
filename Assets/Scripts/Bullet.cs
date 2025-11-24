using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private BoxCollider cd;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;
    
    private Vector3 startPosition;
    private float flyDistance;

    private bool bulletDisabled;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float fly_distance)
    {
        bulletDisabled = false;
        
        cd.enabled = true;
        meshRenderer.enabled = true;
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        flyDistance = fly_distance + 0.5f; // magic number 0.5f is a length of tip of the laser (Check method UpdateVisuals on PlayerAim)
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        
        DisableBulletIfNeeded();
        
        ReturnToPoolIfNeeded();
            
    }

    private void ReturnToPoolIfNeeded()
    {
        if(trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    private void ReturnBulletToPool()
    {
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        CreateImpactFX(other);
        ReturnBulletToPool();
    }

    private void CreateImpactFX(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            ContactPoint contact = other.contacts[0];
            GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFX.transform.position = contact.point;
            ObjectPool.instance.ReturnObject(newImpactFX, 1);
        }
    }
}