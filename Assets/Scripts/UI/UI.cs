using System;
using System.Collections;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;
    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection {get; private set;}
    public UI_GameOver gameOverUI { get; private set; }
    public GameObject pauseUI;
    [SerializeField] private GameObject[] UIElements;
    
    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
    }

    private void Start()
    {
        AssignUIInputs();
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

    public void StartTheGame()
    {
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();
    }
    public void QuitTheGame() => Application.Quit();

    // reason we used method because we have one scene.
    public void RestartTheGame() => GameManager.instance.RestartScene();

    public void PauseSwitch()
    {
        bool gamePaused = pauseUI.activeSelf;
        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
        }
    }

    public void ShowGameOverUI(string message = "Game Over")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
    }

    private void AssignUIInputs()
    {
        PlayerControls controls = GameManager.instance.player.controls;
        
        controls.UI.UIPause.performed += ctx => PauseSwitch();
    }


}