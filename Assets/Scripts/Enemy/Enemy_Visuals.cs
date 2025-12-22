using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

public enum Enemy_MeleeWeaponType
{
    OneHand = 0,
    Throw = 1,
    Unarmed = 2,
}

public enum Enemy_RangeWeaponType
{
    Pistol = 0,
    Revolver = 1,
    Shotgun = 2,
    AutoRifle = 3,
    Rifle = 4,
    Random = 5,
}

public class Enemy_Visuals : MonoBehaviour
{
    public GameObject currentWeaponModel { get; private set; }
    public GameObject grenadeModel;

    [Header("Corruption visuals")] [SerializeField]
    private GameObject[] corruptionCrystals;

    [SerializeField] private int corruptionAmount;

    [Header("Color")] [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Rig references")] [SerializeField]
    private Transform leftHandIK;

    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private MultiAimConstraint weaponAimConstraint;

    private float leftHandTargetWeight;
    private float weaponAimTargetWeight;
    private float rigChangeRate;


    private void Update()
    {
        // leftHandIK and WeaponAim for enemy range, not for enemy melee
        if (leftHandIKConstraint != null)
            leftHandIKConstraint.weight = AdjustIKWeight(leftHandIKConstraint.weight, leftHandTargetWeight);
        if (weaponAimConstraint != null)
            weaponAimConstraint.weight = AdjustIKWeight(weaponAimConstraint.weight, weaponAimTargetWeight);
    }

    public void EnableGrenadeModel(bool active) => grenadeModel?.SetActive(active);
    public void EnableWeaponModel(bool active) => currentWeaponModel?.SetActive(active);
    public void EnableSecondaryWeaponModel(bool active) => FindSecondaryWeaponModel()?.SetActive(active);

    public void EnableWeaponTrail(bool enable)
    {
        Enemy_WeaponModel currentWeaponScript = currentWeaponModel.GetComponent<Enemy_WeaponModel>();
        currentWeaponScript.EnableTrailEffect(enable);
    }

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<Enemy_Range>() != null;

        if (thisEnemyIsRange)
            currentWeaponModel = FindRangeWeaponModel();
        if (thisEnemyIsMelee)
            currentWeaponModel = FindMeleeWeaponModel();

        currentWeaponModel.SetActive(true);

        OverrideAnimatorControllerIfCan();
    }

    private GameObject FindRangeWeaponModel()
    {
        Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_RangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                SwitchAnimationLayer((int)weaponModel.weaponHoldType);
                SetLeftHandIK(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                return weaponModel.gameObject;
            }
        }

        Debug.Log("No range weapon found");
        return null;
    }

    private GameObject FindMeleeWeaponModel()
    {
        Enemy_WeaponModel[] weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        Enemy_MeleeWeaponType weaponType = GetComponent<Enemy_Melee>().weaponType;
        List<Enemy_WeaponModel> filteredWeaponModels = new();
        foreach (var w in weaponModels)
        {
            if (w.weaponType == weaponType)
                filteredWeaponModels.Add(w);
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);
        return filteredWeaponModels[randomIndex].gameObject;
    }

    private void OverrideAnimatorControllerIfCan()
    {
        AnimatorOverrideController overrideController =
            currentWeaponModel.GetComponent<Enemy_WeaponModel>()?.overrideController;
        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);
        Material newMat = new Material(skinnedMeshRenderer.material);

        newMat.mainTexture = colorTextures[randomIndex];
        skinnedMeshRenderer.material = newMat;
    }

    private void SetupRandomCorruption()
    {
        List<int> availableIndexs = new();
        corruptionCrystals = CollectCorruptionCrystals();
        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            availableIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for (int i = 0; i < corruptionAmount; i++)
        {
            if (availableIndexs.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableIndexs.Count);
            int objectIndex = availableIndexs[randomIndex];
            corruptionCrystals[objectIndex].SetActive(true);
            availableIndexs.RemoveAt(randomIndex);
        }
    }

    private GameObject[] CollectCorruptionCrystals()
    {
        Enemy_CorruptionCrystal[] crystalsComponents = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);
        GameObject[] corruptionCrystals = new GameObject[crystalsComponents.Length];

        for (int i = 0; i < crystalsComponents.Length; i++)
        {
            corruptionCrystals[i] = crystalsComponents[i].gameObject;
        }

        return corruptionCrystals;
    }

    private GameObject FindSecondaryWeaponModel()
    {
        Enemy_SecondaryRangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_SecondaryRangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponentInParent<Enemy_Range>().weaponType;
        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
                return weaponModel.gameObject;
        }

        return null;
    }


    private void SwitchAnimationLayer(int layerIndex)
    {
        Animator anim = transform.GetComponentInChildren<Animator>();
        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0f);
        }

        anim.SetLayerWeight(layerIndex, 1f);
    }


    public void EnableIk(bool enableLeftHand, bool enableAim, float changeRate = 10)
    {
        rigChangeRate = changeRate;
        leftHandTargetWeight = enableLeftHand ? 1f : 0f;
        weaponAimTargetWeight = enableAim ? 1f : 0f;
    }

    private void SetLeftHandIK(Transform leftHandTarget, Transform leftElbowTarget)
    {
        leftHandIK.localPosition = leftHandTarget.localPosition;
        leftHandIK.localRotation = leftHandTarget.localRotation;

        leftElbowIK.localPosition = leftElbowTarget.localPosition;
        leftElbowIK.localRotation = leftElbowTarget.localRotation;
    }

    private float AdjustIKWeight(float currentWeight, float targetWeight)
    {
        if (Mathf.Abs(currentWeight - targetWeight) > 0.05f)
            return Mathf.Lerp(currentWeight, targetWeight, rigChangeRate * Time.deltaTime);
        return targetWeight;
    }
}