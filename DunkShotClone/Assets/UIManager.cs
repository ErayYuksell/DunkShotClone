using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] GameObject gamePauseUI;
    [SerializeField] GameObject gameUI;

    [SerializeField] TextMeshProUGUI scoreText;
    int score = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void TapOpenGamePauseMenu()
    {
        Time.timeScale = 0;
        gameUI.SetActive(false);
        gamePauseUI.SetActive(true);
    }
    public void TapCloseGamePauseMenu()
    {
        Time.timeScale = 1;
        gamePauseUI.SetActive(false);
        gamePauseUI.SetActive(true);
    }
    public void TapRestartButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void IncreaseScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }
}
