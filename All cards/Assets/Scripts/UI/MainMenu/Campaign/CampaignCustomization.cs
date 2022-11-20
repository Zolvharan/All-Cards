using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignCustomization : MonoBehaviour
{
    public CampaignSelection campaignSelection;
    public CampaignForm campaignForm;

    public void InitDisplay()
    {
        // TODO: init campaign list
    }

    public void OpenCreationForm(bool isEditing)
    {
        if (isEditing)
        {
            return;
        }
        else
        {
            campaignForm.gameObject.SetActive(true);
            campaignForm.InitForm();
            this.gameObject.SetActive(false);
        }
    }

    public void ReturnToSelection()
    {
        campaignSelection.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
