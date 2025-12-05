using System;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Cover Point")] [SerializeField]
    private GameObject coverPointPrefab;

    [SerializeField] private List<CoverPoint> coverPoints;
    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private float zOffset = 1f;


    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindObjectOfType<Player>().transform;
    }

    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints =
        {
            new Vector3(0, yOffset, zOffset), //Front
            new Vector3(0, yOffset, -zOffset), // Back
            new Vector3(xOffset, yOffset, 0), // Right
            new Vector3(-xOffset, yOffset, 0), // Left
        };

        foreach (Vector3 localCoverPoint in localCoverPoints)
        {
            {
                Vector3 worldPoint = transform.TransformPoint(localCoverPoint);
                CoverPoint coverPoint = Instantiate(coverPointPrefab, worldPoint, Quaternion.identity)
                    .GetComponent<CoverPoint>();
                coverPoints.Add(coverPoint);
            }
        }
    }

    public List<CoverPoint> GetValidCoverPoints(Transform enemyTrans)
    {
        List<CoverPoint> validCoverPoints = new();
        foreach (CoverPoint coverPoint in coverPoints)
        {
            if (IsValidCoverPoint(coverPoint, enemyTrans))
                validCoverPoints.Add(coverPoint);
        }

        return validCoverPoints;
    }

    private bool IsFurthestFromPlayer(CoverPoint coverPoint)
    {
        CoverPoint furthestCoverPoint = null;
        float furthestDistance = 0;

        foreach (CoverPoint point in coverPoints)
        {
            float distance = Vector3.Distance(point.transform.position, playerTransform.position);
            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestCoverPoint = point;
            }
        }
        
        return furthestCoverPoint == coverPoint;
    }
    
    private bool IsValidCoverPoint(CoverPoint coverPoint, Transform enemyTrans)
    {
        if (coverPoint.occupied)
            return false;
        
        if(!IsFurthestFromPlayer(coverPoint))
            return false;

        if (IsCoverCloseToPlayer(coverPoint))
            return false;

        if (IsCoverBehindPlayer(coverPoint, enemyTrans))
            return false;
        
        if (IsCoverCloseToLastCover(coverPoint, enemyTrans))
            return false;
        
        return true;
    }
    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemyTrans)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemyTrans.position);

        return distanceToPlayer < distanceToEnemy;
    }

    private bool IsCoverCloseToPlayer(CoverPoint coverPoint)
    {
        return Vector3.Distance(coverPoint.transform.position, playerTransform.position) < 2;
    }

    private bool IsCoverCloseToLastCover(CoverPoint coverPoint, Transform enemyTrans)
    {
        CoverPoint lastCoverPoint = enemyTrans.GetComponent<Enemy_Range>().lastCover;
        return lastCoverPoint != null &&
               Vector3.Distance(coverPoint.transform.position, lastCoverPoint.transform.position) < 3;
    }
}