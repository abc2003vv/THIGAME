using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject PanelShop,PanelDep;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenPanelInforDep()
    {
        PanelDep.SetActive(true);
    }
    //đóng Shop
    public void CloseShop()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // mở Shop
    public void NextShop()
    {
        SceneManager.LoadScene("Shop");
    }
}
