using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals _playerWeaponVisuals;

    private void Start()
    {
        _playerWeaponVisuals = GetComponentInParent<PlayerWeaponVisuals>();
    }

    public void ReloadOver()
    {
        _playerWeaponVisuals.MaximizeRigWeight();
        
        //refill bullet
    }

    public void ReturnRig()
    {
        _playerWeaponVisuals.MaximizeRigWeight();
        _playerWeaponVisuals.MaximizeLeftHandWeight();
    }
    
    public void WeaponGrabOver()
    {
        
        _playerWeaponVisuals.SetBusyGrabbingWeaponTo(false);
    }
}