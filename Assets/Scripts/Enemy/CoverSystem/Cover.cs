using System;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [Header("Cover Point")] [SerializeField]
    private GameObject coverPointPrefab;

    [SerializeField] private List<CoverPoint> coverPoints;
    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private float zOffset = 1f;


    private void Start()
    {
        GenerateCoverPoints();
    }

    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints =
        {
            new Vector3(0, yOffset, zOffset), //Front
            new Vector3(0, yOffset, -zOffset), // Back
            new Vector3(xOffset,yOffset,0), // Right
            new Vector3(-xOffset, yOffset,0), // Left
        };
        
        foreach(Vector3 localCoverPoint in localCoverPoints){
        {
            Vector3 worldPoint = transform.TransformPoint(localCoverPoint);
            CoverPoint coverPoint = Instantiate(coverPointPrefab, worldPoint, Quaternion.identity).GetComponent<CoverPoint>();
            coverPoints.Add(coverPoint);
        }}
    }

    public List<CoverPoint> GetCoverPoints() => coverPoints;
}