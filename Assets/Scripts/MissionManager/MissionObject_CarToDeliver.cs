using System;
using UnityEngine;

// Action
public class MissionObject_CarToDeliver : MonoBehaviour
{
    public static event Action OnCarDelivery;
    
    public void InvokeOnCarDelivery() =>  OnCarDelivery?.Invoke();
}