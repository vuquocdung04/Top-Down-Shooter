using System;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager instance;
    public PlayerControls controls { get; private set; }
    private Player player;
    private void Awake()
    {
        instance = this;
        controls = new PlayerControls();
    }

    private void Start()
    {
        player = GameManager.instance.player;
        
        SwitchToCharacterControls();
    }

    public void SwitchToCharacterControls()
    {
        controls.Character.Enable();
        controls.UI.Disable();
        controls.Car.Disable();
        player.SetControlsEnabledTo(true);
        
        UI.instance.inGameUI.SwitchToCharacterUI();
    }

    public void SwitchToUIControls()
    {
        controls.UI.Enable();
        controls.Car.Disable();
        controls.Character.Disable();
        player.SetControlsEnabledTo(false);
    }

    public void SwitchToCarControls()
    {
        controls.UI.Disable();
        controls.Character.Disable();
        controls.Car.Enable();
        player.SetControlsEnabledTo(false);
        
        UI.instance.inGameUI.SwitchToCarUI();
    }
}