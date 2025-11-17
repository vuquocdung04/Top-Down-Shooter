using System;
using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunsTransform;
    
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolter;
    [SerializeField] private Transform autoRife;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

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
    }

    private void SwitchOffGun()
    {
        for (int i = 0; i < gunsTransform.Length; i++)
        {
            gunsTransform[i].gameObject.SetActive(false);
        }
    }
}