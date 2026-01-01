using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WeaponSelectionButton : UI_Button
{
    private UI_WeaponSelection weaponSelectionUI;

    [SerializeField] private Weapon_Data weaponData;
    [SerializeField] private Image weaponIcon;

    private UI_SelectedWeaponWindow emptySlot;

    private void OnValidate()
    {
        gameObject.name = "Button - Select Weapon: " + weaponData.weaponType;
    }


    protected override void Start()
    {
        base.Start();
        weaponSelectionUI = GetComponentInParent<UI_WeaponSelection>();
        weaponIcon.sprite = weaponData.weaponIcon;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        weaponIcon.color = Color.yellow;

        emptySlot = weaponSelectionUI.FindEmptySlot();
        emptySlot?.UpdateSlotInfo(weaponData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        weaponIcon.color = Color.white;

        emptySlot?.UpdateSlotInfo(null);
        emptySlot = null;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        weaponIcon.color = Color.white;

        // Tìm slot đang chứa weapon này (nếu có)
        UI_SelectedWeaponWindow slotWithThisWeapon = weaponSelectionUI.FindSlowWithWeaponOfType(weaponData);

        // Nếu weapon này đã được trang bị → Bỏ trang bị
        if (slotWithThisWeapon != null)
        {
            slotWithThisWeapon.SetWeaponSlot(null);
            emptySlot = null;
            return;
        }

        // Nếu không có slot trống → Hiện warning
        UI_SelectedWeaponWindow emptySlotAvailable = weaponSelectionUI.FindEmptySlot();
        if (emptySlotAvailable == null)
        {
            weaponSelectionUI.ShowWarningMessage("No empty slots...");
            return;
        }

        // Trang bị weapon vào slot trống
        emptySlotAvailable.SetWeaponSlot(weaponData);
        emptySlot = null;
    }
    // public override void OnPointerDown(PointerEventData eventData)
    // {
    //     base.OnPointerDown(eventData);
    //     weaponIcon.color = Color.white;
    //
    //     bool noMoreEmptySlot = weaponSelectionUI.FindEmptySlot() == null;
    //     bool noThisWeaponInSlot = weaponSelectionUI.FindSlowWithWeaponOfType(weaponData);
    //
    //     if (noMoreEmptySlot && noThisWeaponInSlot)
    //     {
    //         weaponSelectionUI.ShowWarningMessage("No empty slots...");
    //         return;
    //     }
    //     
    //     UI_SelectedWeaponWindow slotBusyWithThisWeapon = weaponSelectionUI.FindSlowWithWeaponOfType(weaponData);
    //
    //     if (slotBusyWithThisWeapon != null)
    //     {
    //         slotBusyWithThisWeapon.SetWeaponSlot(null);
    //     }
    //     else
    //     {
    //         emptySlot = weaponSelectionUI.FindEmptySlot();
    //         emptySlot?.SetWeaponSlot(weaponData);
    //     }
    //
    //     emptySlot = null;
    // }
}