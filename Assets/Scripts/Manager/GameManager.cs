using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    
    [Header("Settings")] public bool friendlyFire;
    [Space] public bool quickStart;
    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    public void GameStart()
    {
        Debug.Log("GameStart");
        SetDefaultWeaponsForPlayer();
    }
    
    public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void GameCompleted()
    {
        UI.instance.ShowVictoryScreenUI();
        ControlsManager.instance.controls.Character.Disable();
        player.health.currentHealth += 99999; // so player won't die in last second
    }
    
    public void GameOver()
    {
        TimeManager.instance.SlowMotionFor(1.5f);
        UI.instance.ShowGameOverUI();
        CameraManager.instance.ChangeCameraDistance(6);
    }
    
    private void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        Debug.Log(newList.Count);
        player.weapon.SetDefaultWeapon(newList);
    }
}
