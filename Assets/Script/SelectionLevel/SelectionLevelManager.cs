using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class SelectionLevelManager : MonoBehaviour
{
    public Button[] levelButtons;
    public Image Lock;
    public Image Done;
    int highestLevel;

    void Start()
    {
        highestLevel = PlayerPrefs.GetInt("highestLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNum = i + 1;
            if (levelNum > highestLevel)
            {
                levelButtons[i].interactable = false; //chua mo khoa
                levelButtons[i].GetComponent<Image>().sprite = Lock.sprite;
                levelButtons[i].GetComponentInChildren<TMP_Text>().text = "";
            }
            else
            {
                //levelButtons[i].interactable = true
            }
        }
    }

    void Update()
    {

    }

    public void onClickBackGame()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("MainMenu");
    }
}
