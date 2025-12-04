using System;
using UnityEngine;

public enum Enemy_RangeWeaponHoldType
{
    Common = 0,
    LowHold = 1,
    HighHold = 2,
}

public class Enemy_RangeWeaponModel : MonoBehaviour
{
    public Enemy_RangeWeaponType weaponType;
    public Enemy_RangeWeaponHoldType weaponHoldType;
    
    public Transform leftHandTarget;
    public Transform leftElbowTarget;
}