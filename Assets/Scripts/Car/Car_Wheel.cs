using System;
using UnityEngine;

public enum AxelType
{
    Front = 0,
    Back = 1,
}

[RequireComponent(typeof(WheelCollider))]
public class Car_Wheel : MonoBehaviour
{
    public AxelType axelType;
    public WheelCollider cd { get; private set; }
    public GameObject model { get; private set; }

    private float defaultSlideStiffness;
    
    private void Awake()
    {
        cd =  GetComponent<WheelCollider>();
        model = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    public void SetDefaultStiffness(float newValue)
    {
        defaultSlideStiffness = newValue;
        RestoreDefaultStiffness();
    }
    
    public void RestoreDefaultStiffness()
    {
        WheelFrictionCurve sidewayFriction = cd.sidewaysFriction;
        sidewayFriction.stiffness = defaultSlideStiffness;
        cd.sidewaysFriction = sidewayFriction;
    }
}