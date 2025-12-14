using System;
using UnityEngine;

public class Enemy_BossVisuals : MonoBehaviour
{
    private Enemy_Boss enemy;
    [SerializeField] private GameObject[] batteries;
    [SerializeField] private float initalBatteryScaleY = 0.2f;
    
    private float disChargeSpeed; // tieu hao khi flamethrower
    private float reChargeSpeed; // nap

    private bool isRecharging;
    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();
        ResetBatteries();
    }

    private void Update()
    {
        UpdateBatteriesScale();
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