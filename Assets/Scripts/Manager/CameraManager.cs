using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;
    [Header("Camera Distance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    [SerializeField] private float targetCameraDistance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        if(!canChangeCameraDistance) return;
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        float currentDistance = transposer.m_CameraDistance;
        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.1f) return;
        transposer.m_CameraDistance = Mathf.Lerp(transposer.m_CameraDistance, targetCameraDistance,
            Time.deltaTime * distanceChangeRate);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
    
    public void ChangeCameraTarget(Transform target, float cameraDistance = 10, float newLookAHeadTime = 0)
    {
        virtualCamera.Follow = target;
        transposer.m_LookaheadTime  = newLookAHeadTime;
        ChangeCameraDistance(cameraDistance);
    }
}