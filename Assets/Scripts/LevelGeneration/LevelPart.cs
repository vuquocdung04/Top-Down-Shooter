using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{

    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint(); // Enter của LevelPart MỚI
        AlignTo(entrancePoint, targetSnapPoint); // alignment should be before position snapping
        SnapTo(entrancePoint, targetSnapPoint); // targetSnapPoint = Exit của LevelPart CŨ
    }

    private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // LƯU Ý: Enter và Exit SnapPoint đều phải có trục Z hướng RA NGOÀI
        // - Enter: trục Z hướng ra = hướng mà Exit sẽ snap vào
        // - Exit: trục Z hướng ra = hướng mà Enter tiếp theo sẽ snap vào
        
        // Lưu góc lệch giữa SnapPoint và LevelPart
        var rotationOffset = ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
    
        // Copy rotation của target
        transform.rotation = targetSnapPoint.transform.rotation;
    
        // Xoay 180° để đối diện (Exit → gặp ← Enter)
        transform.Rotate(0, 180, 0);
    
        // Bù lại offset vì SnapPoint đã bị xoay theo
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
}