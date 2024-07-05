using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SelectionLevelManager : MonoBehaviour
{

    //Choose level
    public void onClickLevel(int levelNum)
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void onClickBackGame()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("MainMenu");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
