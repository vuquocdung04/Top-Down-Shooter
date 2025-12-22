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
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnValidate()
    {
        gameObject.name = "SnapPoint - " + pointType;
    }
}