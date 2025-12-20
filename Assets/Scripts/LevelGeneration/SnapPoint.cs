using UnityEngine;

public enum SnapPointType
{
    Enter = 0,
    Exit = 1,
}

public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType;

    private void OnValidate()
    {
        gameObject.name = "SnapPoint - " + pointType;
    }
}