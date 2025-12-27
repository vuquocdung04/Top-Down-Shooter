using UnityEngine;


[CreateAssetMenu(fileName = "New Key Mission", menuName = "Missions/Key Mission")]
public class Mission_KeyFind : Mission
{
    [SerializeField] private GameObject key;
    private bool keyFound;
    
    public override void StartMission()
    {
        MissionObject_Key.OnKeyPickedUp += PickUpKey;
        
        UI.instance.inGameUI.UpdateMissionInfo("Find a key holder. Retrieve the key.");
        
        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        // give key to random enemy
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key);
        // Enemy with the key is always stronger compared to common enemies.
        enemy.MakeEnemyVip();
    }
    
    public override bool MissionCompleted()
    {
        return keyFound;
    }

    private void PickUpKey()
    {
        keyFound = true;
        MissionObject_Key.OnKeyPickedUp -= PickUpKey;
        
        UI.instance.inGameUI.UpdateMissionInfo("You are got a key! \n Get to the evacuation point.");
    }
}