using UnityEngine;

public class Enemy_DropController : MonoBehaviour
{
    public void DropItems()
    {
        Debug.Log("Dropped some items");
    }
    
    private void CreateItem(GameObject go)
    {
        GameObject newItem = Instantiate(go, transform.position + Vector3.up, Quaternion.identity);
    }
}