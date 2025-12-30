using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;
    public UI_InGame inGameUI { get; private set; }
    
    [SerializeField] private GameObject[] UIElements;
    
    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
    } 

    // we need a switchTo method because we handle the UI on one canvas
    public void SwitchTo(GameObject uiToSwitchOn)
    {
        foreach (var go in UIElements)
        {
            go.SetActive(false);
        }
        
        uiToSwitchOn.SetActive(true);
    }

    public void SwitchToInGameUI()
    {
        SwitchTo(inGameUI.gameObject);
    }
    
    public void QuitTheGame() => Application.Quit();
}