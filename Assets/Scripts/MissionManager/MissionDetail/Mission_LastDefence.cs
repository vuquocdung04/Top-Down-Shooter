using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "New Defence Mission", menuName = "Missions/Defence Mission")]
public class Mission_LastDefence : Mission
{
    public bool defenceBegun;
    [Header("Cooldown and duration")] public float defenceDuration = 120;
    private float defenceTimer;
    public float waveCooldown = 15;
    private float waveTimer;


    [Header("Respawn details")] public int amountOfRespawnPoints = 2;
    public List<Transform> respawnPoints;
    private Vector3 defencePoint;
    [Space] public int enemiesPerWave;
    public GameObject[] possibleEnemies;

    private string defenceTimerText;

    // OnEnable in so is always called when the object is loaded or the unity engine is recompiled
    private void OnEnable() => defenceBegun = false;

    public override void StartMission()
    {
        defencePoint = FindObjectOfType<MissionEnd_Trigger>().transform.position;
        respawnPoints = new(ClosestPoints(amountOfRespawnPoints));

        UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point.");
    }

    public override void UpdateMission()
    {
        if (defenceBegun == false) return;

        waveTimer -= Time.deltaTime;
        if (defenceTimer > 0)
            defenceTimer -= Time.deltaTime;

        if (waveTimer < 0)
        {
            CreateNewEnemies(enemiesPerWave);
            waveTimer = waveCooldown;
        }

        defenceTimerText = System.TimeSpan.FromSeconds(defenceTimer).ToString("mm':'ss");

        string missionText = "Defend yourself till plane is ready to take off.";
        string missionDetails = "Time left: " + defenceTimerText;
        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    public override bool MissionCompleted()
    {
        if (defenceBegun == false)
        {
            StartDefenceEvent();
            return false;
        }

        return defenceTimer < 0;
    }

    private void StartDefenceEvent()
    {
        waveTimer = 0.5f;
        defenceTimer = defenceDuration;
        defenceBegun = true;
    }

    private void CreateNewEnemies(int amount)
    {
        // amount here means: the number of enemies in each attack wave (enemy count per wave)
        for (int i = 0; i < amount; i++)
        {
            // get random Enemy + position respawn
            int randomEnemyIndex = Random.Range(0, possibleEnemies.Length);
            int randomRespawnIndex = Random.Range(0, respawnPoints.Count);

            // choose enemy + respawn
            Transform randomRespawnPoint = respawnPoints[randomRespawnIndex];
            GameObject randomEnemy = possibleEnemies[randomEnemyIndex];

            // init aggressionRange - 100 too big, so enemy is always advancing to the player
            randomEnemy.GetComponent<Enemy>().aggressionRange = 100;
            ObjectPool.instance.GetObject(randomEnemy, randomRespawnPoint);
        }
    }

    private List<Transform> ClosestPoints(int amount)
    {
        List<Transform> closetPoints = new();
        List<MissionObject_EnemyRespawnPoint> allPoints = new(FindObjectsOfType<MissionObject_EnemyRespawnPoint>());

        while (closetPoints.Count < amount && allPoints.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            MissionObject_EnemyRespawnPoint closetPoint = null;

            foreach (var point in allPoints)
            {
                float distance = Vector3.Distance(point.transform.position, defencePoint);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closetPoint = point;
                }
            }

            if (closetPoint != null)
            {
                closetPoints.Add(closetPoint.transform);
                allPoints.Remove(closetPoint);
            }
        }

        return closetPoints;
    }
}