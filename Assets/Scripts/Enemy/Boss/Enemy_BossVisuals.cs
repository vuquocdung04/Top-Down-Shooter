using System;
using UnityEngine;

public class Enemy_BossVisuals : MonoBehaviour
{
    private Enemy_Boss enemy;

    [SerializeField] private float landingOffset = 1;
    [SerializeField] private ParticleSystem landingZoneFx;
    [SerializeField] private GameObject[] weaponTrails;
    
    [Header("Batteries")]
    [SerializeField] private GameObject[] batteries;
    [SerializeField] private float initalBatteryScaleY = 0.2f;
    
    private float disChargeSpeed; // tieu hao khi flamethrower
    private float reChargeSpeed; // nap

    private bool isRecharging;
    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();
        landingZoneFx.transform.parent = null;
        landingZoneFx.Stop();
        
        ResetBatteries();
        
        EnableWeaponTrail(false);
    }

    private void Update()
    {
        UpdateBatteriesScale();
    }

    public void EnableWeaponTrail(bool active)
    {
        if(weaponTrails.Length <= 0) return;
        
        foreach (var trail in weaponTrails)
        {
            trail.gameObject.SetActive(active);
        }
    }
    
    public void PlaceLandingZone(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector3 offset = direction * landingOffset;
        
        landingZoneFx.transform.position = target + offset;
        landingZoneFx.Clear();
        
        var mainModule = landingZoneFx.main;
        // traveltime is jump distance to target
        mainModule.startLifetime = enemy.travelTimeToTarget * 2f;
        
        landingZoneFx.Play();
    }
    
    
    private void UpdateBatteriesScale()
    {
        if(batteries.Length <=0) return;

        foreach (GameObject battery in batteries)
        {
            if (battery.activeSelf)
            {
                float scaleChange = (isRecharging ? reChargeSpeed : -disChargeSpeed) * Time.deltaTime;
                float newScaleY = Mathf.Clamp(battery.transform.localScale.y + scaleChange, 0, initalBatteryScaleY);
                battery.transform.localScale = new Vector3(0.15f, newScaleY, 0.15f); // 0.15 is config scene 

                if (battery.transform.localScale.y <= 0)
                {
                    battery.SetActive(false);
                }
            }
        }
    }
    
    public void ResetBatteries()
    {
        isRecharging = true;
        reChargeSpeed = initalBatteryScaleY / enemy.abilityCooldown;    // sac
        disChargeSpeed = initalBatteryScaleY / (enemy.flameThrowDuration * 0.75f); // phong, 0.75 is magic number config

        foreach (GameObject battery in batteries)
        {
            battery.SetActive(true);
        }
    }
    
    public void DisChargeBatteries() => isRecharging = false;
    
}