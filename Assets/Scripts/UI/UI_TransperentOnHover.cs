using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TransperentOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Dictionary<Image, Color> originalImageColors = new();
    private Dictionary<TextMeshProUGUI, Color> originalTextColors = new();

    private bool hasUIWeaponSlots;
    private Player_WeaponController playerWeaponController;
    private void Start()
    {
        hasUIWeaponSlots = GetComponentInChildren<UI_WeaponSlot>();
        if (hasUIWeaponSlots)
            playerWeaponController = FindObjectOfType<Player_WeaponController>();
        
        foreach (var image in GetComponentsInChildren<Image>())
        {
            originalImageColors[image] = image.color;
        }

        foreach (var textColor in GetComponentsInChildren<TextMeshProUGUI>())
        {
            originalTextColors[textColor] = textColor.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var image in originalImageColors.Keys)
        {
            var color = image.color;
            color.a = 0.15f;
            image.color = color;
        }

        foreach (var textColor in originalTextColors.Keys)
        {
            var color = textColor.color;
            color.a = 0.15f;
            textColor.color = color;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var image in originalImageColors.Keys)
        {
            image.color = originalImageColors[image];
        }

        foreach (var textColor in originalTextColors.Keys)
        {
            textColor.color = originalTextColors[textColor];
        }
        
        playerWeaponController?.UpdateWeaponUI();
    }
}