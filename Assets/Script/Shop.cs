using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject Panelcharacter, PanelShop,Panel_Character,Panel_Potion,PanelDepLao,Panel_Increaseddamage,Coin_Shop,PanelChest;
    //OpenShop
    public void OpenChest()
    {
        PanelChest.SetActive(true);
        CloseIncreaseddamage();
        ClosePanelDepLao();
        ClosePotion();
        ClosePanelCharacter();
    }
    public void OpenPanelShop ()
    {
        PanelShop.SetActive(true);
    }
    public void OpenPotion()
    {
        Panel_Potion.SetActive(true);
         CloseChest();
         CloseIncreaseddamage();
         ClosePanelDepLao();
         ClosePanelCharacter();
    }
    public void OpenPanelDepLao()
    {
        PanelDepLao.SetActive(true);
        CloseChest();
        CloseIncreaseddamage();
        ClosePotion();
        ClosePanelCharacter();
    }
    public void OpenIncreaseddamage()
    {
        Panel_Increaseddamage.SetActive(true);
        CloseChest();
        ClosePanelDepLao();
        ClosePotion();
        ClosePanelCharacter();
    }
    //close Shop
    public void CloseChest()
    {
        PanelChest.SetActive(false);
    }
     public void ClosePotion()
    {
        Panel_Potion.SetActive(false);
    }
    public void ClosePanelDepLao()
    {
        PanelDepLao.SetActive(false);
    }
    public void CloseIncreaseddamage()
    {
        Panel_Increaseddamage.SetActive(false);
        
    }
    public void CloseShop()
    {
        PanelShop.SetActive(false);
    }
    public void ClosePanelCharacter()
    {
        Panel_Character.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
