using System;
using UnityEngine;

public enum SnapPointType
{
    Enter = 0,
    Exit = 1,
}

public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType;

    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        
        if(boxCollider != null)
            boxCollider.enabled = false;
        
        if(meshCollider != null)
            meshCollider.enabled = false;
    }

    private void OnValidate()
    {
        gameObject.name = "SnapPoint - " + pointType;
    }
}