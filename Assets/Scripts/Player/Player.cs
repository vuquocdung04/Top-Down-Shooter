using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls { get; private set; }
    public Player_AimController AimController { get; private set; }
    
    public Player_Movement movement { get; private set; }
    
    public Player_WeaponController weapon { get; private set; }
    
    public Player_WeaponVisuals weaponVisuals  { get; private set; }
    
    public Player_Interaction playerInteraction { get; private set; }

    public Player_Health health { get; private set; }
    
    public Ragdoll ragdoll { get; private set; }
    
    public Animator anim { get; private set; }
    
    public bool controlsEnable { get;private set; }
    
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        health = GetComponent<Player_Health>();
        ragdoll = GetComponent<Ragdoll>();
        AimController = GetComponent<Player_AimController>();
        movement = GetComponent<Player_Movement>();
        weapon = GetComponent<Player_WeaponController>();
        weaponVisuals = GetComponent<Player_WeaponVisuals>();
        playerInteraction = GetComponent<Player_Interaction>();
        
        controls = ControlsManager.instance.controls;
    }
    
    private void OnEnable()
    {
        controls.Enable();
        controls.Character.UIMissionToolTipSwitch.performed += ctx => UI.instance.inGameUI.SwitchMissionToolTip();
        controls.Character.UIPause.performed += ctx => UI.instance.PauseSwitch();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    
    public void SetControlsEnabledTo(bool enabled) => controlsEnable = enabled;
}