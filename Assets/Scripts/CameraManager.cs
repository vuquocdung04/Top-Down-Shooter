using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;
    [SerializeField] private float distanceChangeRate;
    private float targetCameraDistance;

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
        //UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        float currentDistance = transposer.m_CameraDistance;
        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.1f) return;
        transposer.m_CameraDistance = Mathf.Lerp(transposer.m_CameraDistance, targetCameraDistance,
            Time.deltaTime * distanceChangeRate);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
}