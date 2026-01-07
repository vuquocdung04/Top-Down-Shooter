using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIToSwitchOn;
    public UI_SelectedWeaponWindow[] selectedWeapons;

    [Header("Warning Info")] [SerializeField]
    private TextMeshProUGUI warningText;

    [SerializeField] private float disaperaingSpeed = 0.25f;
    
    private float currentWarningAlpha;
    private float targetWarningAlpha;
    
    private void Start()
    {
        selectedWeapons = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    private void Update()
    {
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= Time.deltaTime * disaperaingSpeed;
            warningText.color = new Color(1,1,1,currentWarningAlpha);
        }
    }

    public void ConfirmWeaponSelection()
    {
        if (AtLeastOneWeaponSelected())
        {
            UI.instance.SwitchTo(nextUIToSwitchOn);
            UI.instance.StartLevelGeneration();
        }
        else
            ShowWarningMessage("Select at least one weapon");
    }
    
    private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;
    
    // method is post data weapon ui selected to in game.
    public List<Weapon_Data> SelectedWeaponData()
    {
        List<Weapon_Data> selectedData = new();
        
        foreach(UI_SelectedWeaponWindow weapon in selectedWeapons)
        {
            if(weapon.weaponData != null)
                selectedData.Add(weapon.weaponData);
        }
        return selectedData;
    }

    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        for (int i = 0; i < selectedWeapons.Length; i++)
        {
            if(selectedWeapons[i].IsEmpty())
                return selectedWeapons[i];
        }
        return null;
    }

    public UI_SelectedWeaponWindow FindSlowWithWeaponOfType(Weapon_Data weaponData)
    {
        for (int i = 0; i < selectedWeapons.Length; i++)
        {
            if(selectedWeapons[i].weaponData == weaponData) return selectedWeapons[i];
        }
        return null;
    }

    public void ShowWarningMessage(string message)
    {
        warningText.text = message;
        warningText.color = Color.white;

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0;
    }
    
}
