using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healtBar;

    [Header("Weapons")] [SerializeField] private UI_WeaponSlot[] weaponSlots_UI;

    [Header("Missions")] [SerializeField] private GameObject missionToolTipParent;
    [SerializeField] private GameObject missionHelpToolTip;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI missionDetails;

    private bool tooltipActive = true;
    private void Awake()
    {
        weaponSlots_UI = GetComponentsInChildren<UI_WeaponSlot>();
    }

    public void SwitchMissionToolTip()
    {
        tooltipActive = !tooltipActive;
        missionToolTipParent.SetActive(tooltipActive);
        missionHelpToolTip.SetActive(!tooltipActive);
    }
    
    
    public void UpdateMissionInfo(string txt, string details = "")
    {
        missionText.text = txt;
        missionDetails.text = details;
    }
    public void UpdateWeaponUI(List<Weapon> weaponSlots, Weapon currentWeapon)
    {
        for (int i = 0; i < weaponSlots_UI.Length; i++)
        {
            if (i < weaponSlots.Count)
            {
                // Update slot
                bool isActiveWeapon = weaponSlots[i] == currentWeapon;
                weaponSlots_UI[i].UpdateWeaponSlot(weaponSlots[i], isActiveWeapon);
            }
            else
            {
                weaponSlots_UI[i].UpdateWeaponSlot(null, false);
            }
        }
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healtBar.fillAmount = currentHealth / maxHealth;
    }
}