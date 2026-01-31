using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static GameMenu Instance;
    
    public GameObject PauseMenuPanel;
    
    public GameObject GameOverPanel;
    public TextMeshProUGUI GameOverReasonText;
    public TextMeshProUGUI GameOverMoneyText;

    private void Awake()
    {
        Instance = this;
    }

    // private void Update()
    // {
    //     if (PauseMenuPanel.activeSelf)
    //     {
    //         if (PlayerController.Instance.Input.PauseWasPerformedThisFrame)
    //             Resume();
    //         if (PlayerController.Instance.Input.HitWasPerformedThisFrame)
    //             Retry();
    //     }
    //     
    //     if (GameOverPanel.activeSelf)
    //     {
    //         if (PlayerController.Instance.Input.PauseWasPerformedThisFrame)
    //             Resume();
    //         if (PlayerController.Instance.Input.HitWasPerformedThisFrame)
    //             Retry();
    //     }
    // }

    private void TogglePauseMenu(bool value)
    {
        Instance.PauseMenuPanel.SetActive(value);
    }

    public static void ShowGameOverMenu(string reason, float money)
    {
        Instance.GameOverPanel.SetActive(true);
        Instance.GameOverReasonText.text = reason;
        Instance.GameOverMoneyText.text = $"${money}";
    }
    
    public static void TogglePauseMenu()
    {
        if (Time.timeScale == 0)
            Instance.Resume();
        else
            Instance.Pause();
    }
    
    public void Pause()
    {
        Time.timeScale = 0;
        TogglePauseMenu(true);
    }
    
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void Resume()
    {
        Time.timeScale = 1;
        TogglePauseMenu(false);
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
