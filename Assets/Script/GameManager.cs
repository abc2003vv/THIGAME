using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gamePauseUI;
    public GameObject gameWinUI;
    public GameObject gameOverUI;


    public void pauseGame()
    {
        gamePauseUI.SetActive(true);
    }

    public void settingGame()
    {
        SceneManager.LoadScene("settinggame");
    }

    public void reSume()
    {
        gamePauseUI.SetActive(false);
    }

    public void gameWin()
    {
        gameWinUI.SetActive(true);
    }

    public void nextLevel()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void reStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
