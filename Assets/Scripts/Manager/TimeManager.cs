using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [SerializeField] private float resumeRate = 3;
    [SerializeField] private float pauseRate = 7;
    
    private float targetTimeScale = 1;
    private float timeAjustRate;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Mathf.Abs(Time.timeScale - targetTimeScale) > 0.05f)
        {
            float adjustRate = timeAjustRate * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, adjustRate);
        }
        else
        {
            Time.timeScale = targetTimeScale;
        }
    }

    public void PauseTime()
    {
        timeAjustRate = pauseRate;
        targetTimeScale = 0;
    }

    public void ResumeTime()
    {
        timeAjustRate = resumeRate;
        targetTimeScale = 1;
    }

    public void SlowMotionFor(float seconds) => StartCoroutine(SlowTimeCo(seconds));
    
    private IEnumerator SlowTimeCo(float seconds)
    {
        Debug.Log("SlowTime");
        targetTimeScale = 0.5f;
        Time.timeScale = targetTimeScale;
        yield return new WaitForSecondsRealtime(seconds);
        ResumeTime();
    }
}