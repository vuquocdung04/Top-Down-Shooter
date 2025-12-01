using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Enemy_MeleeWeaponType
{
    OneHand = 0,
    Throw = 1,
}

public class Enemy_Visuals : MonoBehaviour
{
    [Header("Weapon model")] [SerializeField]
    private Enemy_WeaponModel[] weaponModels;

    private Enemy_MeleeWeaponType weaponType;
    public GameObject currentWeaponModel;
    
    [Header("Color")] [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Start()
    {
        weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
    }

    public void SetupWeaponType(Enemy_MeleeWeaponType type)
    {
        weaponType = type;
    }
    
    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
    }

    private void SetupRandomWeapon()
    {
        foreach (var w in weaponModels)
        {
            w.gameObject.SetActive(false);
        }

        List<Enemy_WeaponModel> filteredWeaponModels = new();
        foreach (var w in weaponModels)
        {
            if(w.weaponType == weaponType)
                filteredWeaponModels.Add(w);
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);
        currentWeaponModel = filteredWeaponModels[randomIndex].gameObject;
        currentWeaponModel.gameObject.SetActive(true);
    }
    private void SetupRandomColor()
    {
        int randomIndex =  Random.Range(0, colorTextures.Length);
        Material newMat = new  Material(skinnedMeshRenderer.material);
        
        newMat.mainTexture = colorTextures[randomIndex];
        skinnedMeshRenderer.material = newMat;
    }
}