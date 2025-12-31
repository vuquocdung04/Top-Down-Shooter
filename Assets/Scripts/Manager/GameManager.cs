using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    
    [Header("Settings")] public bool friendlyFire;
    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    public void GameStart()
    {
        SetDefaultWeaponsForPlayer();
        LevelGenerator.instance.InitializeGeneration();
    }
    
    private void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        player.weapon.SetDefaultWeapon(newList);
    }
}
