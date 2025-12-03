using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Enemy_MeleeWeaponType
{
    OneHand = 0,
    Throw = 1,
    Unarmed = 2,
}

public class Enemy_Visuals : MonoBehaviour
{
    [Header("Weapon Visual")] [SerializeField]
    private Enemy_WeaponModel[] weaponModels;

    private Enemy_MeleeWeaponType weaponType;
    public GameObject currentWeaponModel { get; private set; }

    [Header("Corruption visuals")] [SerializeField]
    private GameObject[] corruptionCrystals;

    [SerializeField] private int corruptionAmount;
    
    [Header("Color")] [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    
    private void Awake()
    {
        weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        CollectCorruptionCrystals();
    }

    public void SetupWeaponType(Enemy_MeleeWeaponType type)
    {
        weaponType = type;
    }
    
    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
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
        currentWeaponModel.SetActive(true);

        OverideAnimatorControllerIfCan();
    }

    private void OverideAnimatorControllerIfCan()
    {
        AnimatorOverrideController overrideController =
            currentWeaponModel.GetComponent<Enemy_WeaponModel>().overrideController;
        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }

    private void SetupRandomColor()
    {
        int randomIndex =  Random.Range(0, colorTextures.Length);
        Material newMat = new  Material(skinnedMeshRenderer.material);
        
        newMat.mainTexture = colorTextures[randomIndex];
        skinnedMeshRenderer.material = newMat;
    }

    private void SetupRandomCorruption()
    {
        List<int> availableIndexs = new();
        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            availableIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for (int i = 0; i < corruptionAmount; i++)
        {
            if(availableIndexs.Count ==0)
                break;
            
            int randomIndex = Random.Range(0, availableIndexs.Count);
            int objectIndex = availableIndexs[randomIndex];
            corruptionCrystals[objectIndex].SetActive(true);
            availableIndexs.RemoveAt(randomIndex);
        }
    }
    
    private void CollectCorruptionCrystals()
    {
        Enemy_CorruptionCrystal[] crystalsComponents = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);
        corruptionCrystals = new GameObject[crystalsComponents.Length];

        for (int i = 0; i < crystalsComponents.Length; i++)
        {
            corruptionCrystals[i] = crystalsComponents[i].gameObject;
        }
    }

}