using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignSelection : MonoBehaviour
{
    public MainMenuManager mainMenuManager;
    public CampaignCustomization campaignCustomization;

    public void InitDisplay()
    {
        // TODO: init campaign list
    }

    public void OpenCustomization()
    {
        campaignCustomization.gameObject.SetActive(true);
        campaignCustomization.InitDisplay();
        this.gameObject.SetActive(false);
    }

    public void ReturnToFrontPage()
    {
        mainMenuManager.OpenFrontPage();
        this.gameObject.SetActive(false);
    }
}
