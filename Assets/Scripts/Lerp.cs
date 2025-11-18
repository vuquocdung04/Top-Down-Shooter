using System;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    public float value;
    public float maxValue;

    [Range(0, 1)] public float step;

    private void Update()
    {
        value = Mathf.Lerp(value, maxValue, step);
    }
}