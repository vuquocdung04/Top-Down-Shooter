using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private void OnCollisionEnter(Collision other)
    {
        CreateImpactFX(other);
        Destroy(gameObject);
    }

    private void CreateImpactFX(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            ContactPoint contact = other.contacts[0];
            GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));
            
            Destroy(newImpactFX, 1f);
        }
    }
}