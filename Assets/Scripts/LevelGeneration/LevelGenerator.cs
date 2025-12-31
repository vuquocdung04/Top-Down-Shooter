using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;
    
    
    private List<Enemy> enemyList;
    
    [SerializeField] private NavMeshSurface navMeshSurface;
    
    [Space]
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    private List<Transform> generatedLevelParts = new();
    
    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;
    
    [Space] [SerializeField] private float generationCooldown;

    private float coolDownTimer;
    private bool generationOver = true;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemyList = new();
        defaultSnapPoint = nextSnapPoint;
    }

    private void Update()
    {
        if (generationOver) return;

        coolDownTimer -= Time.deltaTime;
        if (coolDownTimer < 0)
        {
            if (currentLevelParts.Count > 0)
            {
                coolDownTimer = generationCooldown;
                GenerateNextLevelPart();
            }
            else if (generationOver == false)
            {
                FinishGeneration();
            }
        }
    }
    public void InitializeGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        currentLevelParts = new (levelParts);

        DestroyOldLevelPartAndEnemies();
    }

    private void DestroyOldLevelPartAndEnemies()
    {
        foreach(var enemy in enemyList)
            Destroy(enemy.gameObject);
        
        foreach (Transform t in generatedLevelParts)
        {
            Destroy(t.gameObject);
        }

        generatedLevelParts.Clear();
        enemyList.Clear();
    }

    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart();
        
        navMeshSurface.BuildNavMesh();

        foreach (var enemy in enemyList)
        {
            // because damage system is work
            enemy.transform.parent = null;
            enemy.gameObject.SetActive(true);
        }
        
        MissionManager.instance.StartMission();
    }

    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;

        if (generationOver)
            newPart = Instantiate(lastLevelPart);
        else
            newPart = Instantiate(ChooseRandomPart());

        generatedLevelParts.Add(newPart);
        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);
        if (levelPartScript.IntersectionDetected())
        {
            InitializeGeneration();
            return;
        }

        nextSnapPoint = levelPartScript.GetExitPoint();
        enemyList.AddRange(levelPartScript.MyEnemies());
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform choosePart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex);
        return choosePart;
    }

    public Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyList.Count);
        return enemyList[randomIndex];
    }

    public List<Enemy> GetEnemyList() => enemyList;
}