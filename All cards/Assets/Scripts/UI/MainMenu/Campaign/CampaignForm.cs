using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignForm : MonoBehaviour
{
    public CampaignCustomization campaignCustomization;
    public BattleForm battleForm;
    public CampaignComponents campaignComponents;

    public Dropdown battleList;

    List<BattleData> currBattles;

    public void InitForm()
    {
        currBattles = new List<BattleData>();
        campaignComponents.InitForm();
    }

    public void OpenBattleForm(bool isEditing)
    {
        if (isEditing)
        {
            return;
        }
        else
        {
            battleForm.gameObject.SetActive(true);
            battleForm.InitForm(false);
            this.gameObject.SetActive(false);
        }
    }

    public void ReturnToCustomization()
    {
        campaignCustomization.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OpenCampaignComponents()
    {
        campaignComponents.gameObject.SetActive(true);
        campaignComponents.OpenForm();
        this.gameObject.SetActive(false);
    }

    public List<BattleData> GetBattles()
    {
        return currBattles;
    }
}
