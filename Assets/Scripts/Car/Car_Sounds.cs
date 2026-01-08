using System;
using UnityEngine;
public class Car_Sounds : MonoBehaviour
{
    private Car_Controller car;

    [SerializeField] private float engineVolume = 0.07f;
    [SerializeField] private AudioSource engineStart;
    [SerializeField] private AudioSource engineOff;
    [SerializeField] private AudioSource workingEngine;

    private float minSpeed = 0;
    private float maxSpeed = 10;

    [SerializeField] private float minPitch = 0.75f;
    [SerializeField] private float maxPitch = 1.5f;

    private bool allowCarSounds;

    private void Start()
    {
        car = GetComponent<Car_Controller>();
        Invoke(nameof(AllowCarSounds), 1f);
    }

    private void Update()
    {
        UpdateEngineSound();
    }

    private void UpdateEngineSound()
    {
        float currentSpeed = car.speed;
        float pitch = Mathf.Lerp(minPitch, maxPitch, currentSpeed / maxSpeed);
        workingEngine.pitch = pitch;

    }

    public void ActivateCarSFX(bool activate)
    {
        if(allowCarSounds == false) return;
        
        if (activate)
        {
            engineStart.Play();
            AudioManager.instance.SFXDelayAndFade(workingEngine, true,engineVolume, 1);
        }
        else
        {
            AudioManager.instance.SFXDelayAndFade(workingEngine, false,engineVolume,0, 0.25f);
            engineOff.Play();
        }
    }

    private void AllowCarSounds() =>  allowCarSounds = true;
    
}