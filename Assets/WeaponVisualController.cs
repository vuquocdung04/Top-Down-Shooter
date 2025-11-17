
using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunsTransform;
    
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolter;
    [SerializeField] private Transform autoRife;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

    private Transform currentGun;
    [Header("Left hand IK")]
    [SerializeField] private Transform leftHand;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchOn(pistol);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchOn(revolter);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchOn(autoRife);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchOn(shotgun);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchOn(sniper);

    }

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
}