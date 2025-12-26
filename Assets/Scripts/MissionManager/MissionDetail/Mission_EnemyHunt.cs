using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hunt Mission", menuName = "Missions/Hunt Mission")]
public class Mission_EnemyHunt : Mission
{
    public int amountToKill = 12;
    public EnemyType enemyType;

    private int killsToGo;
    
    public override void StartMission()
    {
        killsToGo = amountToKill;
        MissionObject_HuntTarget.OnTargetKilled += EliminateTarget;
        
        List<Enemy> validEnemies = new();

        if (enemyType == EnemyType.Random)
            validEnemies = LevelGenerator.instance.GetEnemyList();
        else
        {
            foreach (var enemy in LevelGenerator.instance.GetEnemyList())
            {
                if(enemy.enemyType == enemyType)
                    validEnemies.Add(enemy);
            }
        }
        
        for (int i = 0; i < amountToKill; i++)
        {
            if(validEnemies.Count <= 0)
                return;
            
            // add xong xoa
            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObject_HuntTarget>();
            validEnemies.RemoveAt(randomIndex);
        }
    }

    public override void UpdateMission()
    {
        base.UpdateMission();
    }

    public override bool MissionCompleted()
    {
        return killsToGo <= 0;
    }

    private void EliminateTarget()
    {
        killsToGo--;
        if (killsToGo <= 0)
            MissionObject_HuntTarget.OnTargetKilled -= EliminateTarget;
    }
}