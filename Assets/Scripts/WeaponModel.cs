using UnityEngine;

public enum GrabType
{
    SideGrab,
    BackGrab
}

public enum HoldType
{
    CommonHold = 1,
    LowHold = 2,
    HighHold = 3
}
public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public GrabType grabType; // lay
    public HoldType holdType;
    
    public Transform gunPoint;
    public Transform holdPoint;
}