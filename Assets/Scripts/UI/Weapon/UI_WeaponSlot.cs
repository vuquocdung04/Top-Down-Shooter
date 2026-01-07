using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_WeaponSlot : MonoBehaviour
{
    public Image weaponIcon;
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        weaponIcon = GetComponentInChildren<Image>(true);
        ammoText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void UpdateWeaponSlot(Weapon myWeapon, bool activeWeapon)
    {
        if (myWeapon == null)
        {
            weaponIcon.color = Color.clear;
            ammoText.text = "";
            return;
        }

        Color newColor = activeWeapon ? Color.white : new Color(1, 1, 1, 0.35f);
        
        weaponIcon.color = newColor;

        if (myWeapon.Data == null)
        {
            Debug.Log("My Weapon data is null");
        }
        
        weaponIcon.sprite = myWeapon.Data.weaponIcon;

        ammoText.text = myWeapon.bulletsInMagazine + "/" + myWeapon.totalReserveAmmo;
        ammoText.color = Color.white;
    }
}