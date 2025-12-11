using UnityEngine;

public class Enemy_Bullet : Bullet
{
    protected override void OnCollisionEnter(Collision other)
    {
        CreateImpactFX();
        ReturnBulletToPool();
        
        Player player = other.gameObject.GetComponentInParent<Player>();
        // if(player != null)
        //     Debug.Log("Shot player");
    }
}