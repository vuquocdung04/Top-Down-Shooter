using UnityEngine;

public enum EquipType
{
    SideEquipAnimation = 0,
    BackEquipAnimation = 1,
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
    public EquipType equipAnimationType; // lay
    public HoldType holdType; // cach cam sung, sung luc cam bth, highhold cam cao, ...
    
    public Transform gunPoint;
    public Transform holdPoint;
}