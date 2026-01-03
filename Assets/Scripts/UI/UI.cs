using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;
    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection { get; private set; }
    public UI_GameOver gameOverUI { get; private set; }
    public GameObject victoryScreenUI;
    public GameObject pauseUI;
    [SerializeField] private GameObject[] UIElements;

    [Header("Fade Image")] [SerializeField]
    private Image fadeImage;


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
        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));
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

    public void StartTheGame() => StartCoroutine(StartGameSequence());

    public void QuitTheGame() => Application.Quit();

    public void StartLevelGeneration() => LevelGenerator.instance.InitializeGeneration();
    
    // reason we used method because we have one scene.
    public void RestartTheGame()
    {
        StartCoroutine(ChangeImageAlpha(1, 1f, delegate { GameManager.instance.RestartScene(); }));
    }

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

    public void ShowVictoryScreenUI()
    {
        StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictoryScreenUI));
    }

    private void SwitchToVictoryScreenUI()
    {
        SwitchTo(victoryScreenUI);
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;
    }
    
    private void AssignUIInputs()
    {
        PlayerControls controls = GameManager.instance.player.controls;

        controls.UI.UIPause.performed += ctx => PauseSwitch();
    }

    private IEnumerator StartGameSequence()
    {
        StartCoroutine(ChangeImageAlpha(1,1,null));
        yield return new WaitForSeconds(1f);
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();
        StartCoroutine(ChangeImageAlpha(0,1,null));
    }
    
    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action callback)
    {
        float time = 0;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        // call the completion method if it exists
        callback?.Invoke();
    }
}