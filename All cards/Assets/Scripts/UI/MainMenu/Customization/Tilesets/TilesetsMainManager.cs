using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesetsMainManager : MonoBehaviour
{
    public MMManager mainMenuManager;
    public GameObject tilesetTypeSelection;

    public TilesetFormManager tilesetFormManager;
    public LandformFormManager landformFormManager;
    public TileFormManager tileFormManager;

    public Dropdown[] lists;

    public Text deletePrompt;
    public GameObject deleteButton;
    int listDeleteIndex;

    public void OpenFrontPage()
    {
        mainMenuManager.OpenFrontPage();
        this.gameObject.SetActive(false);
    }

    public void OpenMainManager()
    {
        tilesetTypeSelection.SetActive(false);
        this.gameObject.SetActive(true);
        InitDisplay();
    }
    // Inits display elements
    public void InitDisplay()
    {
        listDeleteIndex = -1;
        deletePrompt.text = "";
        deleteButton.SetActive(false);
        RefreshTilesets();
        RefreshLandforms();
        RefreshTiles();
    }

    public void RefreshTilesets()
    {
        List<string> tilesetNames = new List<string>();
        foreach (ExteriorTilesetData tileset in SaveData.GetETilesets())
        {
            tilesetNames.Add(tileset.GetName());
        }
        lists[0].ClearOptions();
        lists[0].AddOptions(tilesetNames);
    }
    public void RefreshLandforms()
    {
        List<string> landformNames = new List<string>();
        foreach (LandformData landform in SaveData.GetLandforms())
        {
            landformNames.Add(landform.GetName());
        }
        lists[1].ClearOptions();
        lists[1].AddOptions(landformNames);
    }
    public void RefreshTiles()
    {
        List<string> tileNames = new List<string>();
        foreach (TileData tile in SaveData.GetTiles())
        {
            tileNames.Add(tile.GetName());
        }
        lists[2].ClearOptions();
        lists[2].AddOptions(tileNames);
    }

    public void StartDeleting(int listIndex)
    {
        listDeleteIndex = listIndex;

        // Ignore deletion if list is empty
        if (lists[listIndex].options.Count != 0)
        {
            deletePrompt.text = "Really delete " + lists[listDeleteIndex].captionText.text + "?";
            deleteButton.SetActive(true);
        }
    }
    public void StopDeleting()
    {
        listDeleteIndex = -1;

        deletePrompt.text = "";
        deleteButton.SetActive(false);
    }
    // Takes dropdown index and deletes item based on dropdown value
    public void DeleteSave()
    {
        switch (listDeleteIndex)
        {
            case 0:
                SaveData.DeleteETileset(lists[listDeleteIndex].value);
                RefreshTilesets();
                break;
            case 1:
                SaveData.DeleteLandform(lists[listDeleteIndex].value);
                RefreshLandforms();
                break;
            case 2:
                SaveData.DeleteTile(lists[listDeleteIndex].value);
                RefreshTiles();
                break;
        }
        StopDeleting();
    }

    public void OpenTilesetForm(bool isEditing)
    {
        // Cannot edit if no tileset exists
        if (!(isEditing && lists[0].options.Count == 0))
        {
            tilesetFormManager.gameObject.SetActive(true);
            tilesetFormManager.InitForm(isEditing, SaveData.GetETilesets()[lists[0].value], lists[0].value);
            this.gameObject.SetActive(false);
        }
    }
    public void OpenLandformForm(bool isEditing)
    {
        // Cannot edit if no landform exists
        if (!(isEditing && lists[1].options.Count == 0))
        {
            landformFormManager.gameObject.SetActive(true);
            if (isEditing)
                landformFormManager.OpenForm(isEditing, SaveData.GetLandforms()[lists[1].value], lists[1].value);
            else
                landformFormManager.OpenForm(isEditing);
            this.gameObject.SetActive(false);
        }
    }
    public void OpenTileForm(bool isEditing)
    {
        // Cannot edit if no tile exists
        if (!(isEditing && lists[2].options.Count == 0))
        {
            tileFormManager.gameObject.SetActive(true);
            if (isEditing)
                tileFormManager.InitForm(isEditing, this.gameObject, SaveData.GetTiles()[lists[2].value], lists[2].value);
            else
                tileFormManager.InitForm(isEditing, this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
