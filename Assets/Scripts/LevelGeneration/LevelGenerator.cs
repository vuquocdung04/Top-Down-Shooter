using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    [SerializeField] private SnapPoint nextSnapPoint;

    [Space] [SerializeField] private float generationCooldown;

    private float coolDownTimer;
    private bool generationOver;

    private void Start()
    {
        currentLevelParts = levelParts;
    }

    private void Update()
    {
        if(generationOver) return;
        
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

    private void FinishGeneration()
    {
        generationOver = true;

        Transform levelPart = Instantiate(lastLevelPart);
        LevelPart levelPartScript = levelPart.GetComponent<LevelPart>();
        
        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);
    }

    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = Instantiate(ChooseRandomPart());
        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);
        if (levelPartScript.IntersectionDetected())
        {
            Debug.LogWarning("Intersection detected");
        }
        
        nextSnapPoint = levelPartScript.GetExitPoint();
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform choosePart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex);
        return choosePart;
    }
}