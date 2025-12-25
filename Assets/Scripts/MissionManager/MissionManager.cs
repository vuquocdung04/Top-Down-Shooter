using System;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    public Mission currentMission;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentMission?.StartMission();
    }

    private void Update()
    {
        if(currentMission != null)
            currentMission.UpdateMission();
    }
    
    private void StartMission() => currentMission.StartMission();
    public bool MissionCompleted() => currentMission.MissionCompleted();

}