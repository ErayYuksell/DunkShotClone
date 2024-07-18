using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gamePauseUI;
    [SerializeField] GameObject gameUI;

    void Start()
    {

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
}
