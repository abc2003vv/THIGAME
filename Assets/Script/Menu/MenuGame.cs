using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGame : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
