using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public Weapon_Data weaponData;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponInfo;

    private void Start()
    {
        weaponData = null;
        UpdateSlotInfo(null);
    }

    public void SetWeaponSlot(Weapon_Data newWeaponData)
    {
        weaponData = newWeaponData;
        UpdateSlotInfo(weaponData);
    }

    
    public void UpdateSlotInfo(Weapon_Data weapon)
    {
        if (weapon == null)
        {
            weaponIcon.color = Color.clear;
            weaponInfo.text = "Select a weapon...";
            return;
        }
        
        weaponIcon.color = Color.white;
        weaponIcon.sprite = weapon.weaponIcon;
        weaponInfo.text = weapon.weaponInfo;
    }
    
    public bool IsEmpty() => weaponData  == null;
}