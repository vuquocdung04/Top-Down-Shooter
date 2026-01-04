using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelPart : MonoBehaviour
{
    [Header("Intersection Check")]
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionCheckColliders;
    [SerializeField] private Transform intersectionCheckParent;

    [ContextMenu("Set static to environment layer")]
    private void AdjustLayerForStaticObjects()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
        {
            if (child.gameObject.isStatic)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Environment");
            }
        }
    }
    
    private void Start()
    {
        if (intersectionCheckColliders.Length <= 0)
        {
            intersectionCheckColliders = intersectionCheckParent.GetComponentsInChildren<Collider>();
        }
    }

    public bool IntersectionDetected()
    {
        Physics.SyncTransforms();
        foreach (var col in intersectionCheckColliders)
        {
            Collider[] hitColliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents, Quaternion.identity, intersectionLayer);
            foreach (var hitCollider in hitColliders)
            {
                IntersectionCheck intersectionCheck = hitCollider.GetComponentInParent<IntersectionCheck>();
                if (intersectionCheck != null && intersectionCheck.transform != intersectionCheckParent)
                    return true;
            }
        }
        return false;
    }
    
    
    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        if (targetSnapPoint == null)
        {
            Debug.LogError($"[LevelPart] targetSnapPoint is null for {gameObject.name}");
            return;
        }

        SnapPoint entrancePoint = GetEntrancePoint();
    
        if (entrancePoint == null)
        {
            Debug.LogError($"[LevelPart] No entrance point found on {gameObject.name}");
            return;
        }

        AlignTo(entrancePoint, targetSnapPoint);
        SnapTo(entrancePoint, targetSnapPoint);
    }

    private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // Thêm null check
        if (ownSnapPoint == null || targetSnapPoint == null)
        {
            Debug.LogError("[LevelPart] SnapPoint is null in AlignTo");
            return;
        }

        if (ownSnapPoint.transform == null || targetSnapPoint.transform == null)
        {
            Debug.LogError("[LevelPart] SnapPoint.transform is null in AlignTo");
            return;
        }

        var rotationOffset = ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        transform.rotation = targetSnapPoint.transform.rotation;
        transform.Rotate(0, 180, 0);
        transform.Rotate(0, -rotationOffset, 0);
    }
    
    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        var offset = transform.position - ownSnapPoint.transform.position;
        var newPosition = targetSnapPoint.transform.position + offset;
        transform.position = newPosition;
    }
    
    // entrance = cong vao  =))
    public SnapPoint GetEntrancePoint() => GetSnapPointOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointOfType(SnapPointType.Exit);
    
    private SnapPoint GetSnapPointOfType(SnapPointType type)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new();
        foreach (var snapPoint in snapPoints)
        {
            if(snapPoint.pointType == type)
                filteredSnapPoints.Add(snapPoint);
        }
        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }
        return null;
    }

    public Enemy[] MyEnemies() => GetComponentsInChildren<Enemy>(true);
}