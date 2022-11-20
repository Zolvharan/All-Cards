using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleForm : MonoBehaviour
{
    public CampaignForm campaignForm;
    public TilesetLoadForm tilesetLoadForm;
    public PlaceUnitsDisplay placeUnitsDisplay;
    public ObjectiveForm objectiveForm;
    public CampaignComponents campaignComponents;

    public Dropdown enemyFactions;
    public Button placeUnits;
    public Text tilesetLabel;

    ExteriorTilesetData currExteriorTileset;
    MapEditorData currMapEditTileset;
    int currTileType;

    public void InitForm(bool isEditing)
    {
        if (isEditing)
        {

        }
        else
        {
            tilesetLabel.text = "None";
            currExteriorTileset = null;
            currMapEditTileset = null;
            placeUnits.interactable = false;
        }

        enemyFactions.ClearOptions();
        enemyFactions.AddOptions(campaignComponents.GetFactionOptions());
        enemyFactions.value = 0;
    }

    public void ReturnToCampaignForm(bool saveChanges)
    {
        if (saveChanges)
        {

        }

        campaignForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OpenTilesetLoadForm()
    {
        tilesetLoadForm.OpenForm();
        this.gameObject.SetActive(false);
    }
    public void SetTileset(ExteriorTilesetData newTileset)
    {
        currTileType = 0;
        currExteriorTileset = newTileset;
        currMapEditTileset = null;
        tilesetLabel.text = currExteriorTileset.tilesetName;
        if (enemyFactions.options.Count > 0)
            placeUnits.interactable = true;
    }
    public void SetTileset(MapEditorData newTileset)
    {
        currTileType = 1;
        currExteriorTileset = null;
        currMapEditTileset = newTileset;
        tilesetLabel.text = currMapEditTileset.tilesetName;
        if (enemyFactions.options.Count > 0)
            placeUnits.interactable = true;
    }

    public void OpenPlaceUnitsDisplay()
    {
        placeUnitsDisplay.gameObject.SetActive(true);
        switch (currTileType)
        {
            case 0:
                placeUnitsDisplay.OpenDisplay(currExteriorTileset, campaignComponents.GetEnemyFactions()[enemyFactions.value].units);
                break;
            case 1:
                placeUnitsDisplay.OpenDisplay(currMapEditTileset, campaignComponents.GetEnemyFactions()[enemyFactions.value].units);
                break;
        }

        this.gameObject.SetActive(false);
    }

    public void OpenObjectiveForm(bool isEditing)
    {
        if (isEditing)
        {
            
        }
        else
        {
            objectiveForm.gameObject.SetActive(true);
            objectiveForm.InitForm(null);
            this.gameObject.SetActive(false);
        }
    }
}
