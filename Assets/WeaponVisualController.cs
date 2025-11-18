using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    private Animator anim;
    
    [SerializeField] private Transform[] gunsTransform;
    
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolter;
    [SerializeField] private Transform autoRife;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

    private Transform currentGun;
    [Header("Rig")] [SerializeField] private float rigIncreaseStep;
    private bool rigShouldBeIncreased;
    
    [Header("Left hand IK")]
    [SerializeField] private Transform leftHand;

    private Rig rig;
    private void Start()
    {
        SwitchOn(pistol);
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        CheckWeaponSwitch();
        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");
            rig.weight = 0.15f;
        }

        if (rigShouldBeIncreased)
        {
            rig.weight += rigIncreaseStep * Time.deltaTime;
            if(rig.weight >= 1)
                rigShouldBeIncreased = false;
        }
    }
    public void ReturnRigWeightToOne() => rigShouldBeIncreased = true;
    
    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGun();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;
        AttachLeftHand();
    }

    private void SwitchOffGun()
    {
        for (int i = 0; i < gunsTransform.Length; i++)
        {
            gunsTransform[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        
        leftHand.localPosition = targetTransform.localPosition;
        leftHand.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }
        anim.SetLayerWeight(layerIndex, 1);
    }
    
    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
            SwitchAnimationLayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(revolter);
            SwitchAnimationLayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRife);
            SwitchAnimationLayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotgun);
            SwitchAnimationLayer(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(sniper);
            SwitchAnimationLayer(3);
        }
    }

}