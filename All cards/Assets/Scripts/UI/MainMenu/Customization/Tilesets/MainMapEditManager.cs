using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMapEditManager : MonoBehaviour
{
    public MMManager mainMenuManager;
    public GameObject tilesetTypeSelection;

    public MapEditorManager mapEditorManager;

    public Dropdown list;

    public Text deletePrompt;
    public GameObject deleteButton;

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
        deletePrompt.text = "";
        deleteButton.SetActive(false);
        RefreshTilesets();
    }

    public void RefreshTilesets()
    {
        List<string> tilesetNames = new List<string>();
        foreach (MapEditorData tileset in SaveData.GetMapTilesets())
        {
            tilesetNames.Add(tileset.GetName());
        }
        list.ClearOptions();
        list.AddOptions(tilesetNames);
    }

    public void StartDeleting()
    {
        // Ignore deletion if list is empty
        if (list.options.Count != 0)
        {
            deletePrompt.text = "Really delete " + list.captionText.text + "?";
            deleteButton.SetActive(true);
        }
    }
    public void StopDeleting()
    {
        deletePrompt.text = "";
        deleteButton.SetActive(false);
    }
    // Takes dropdown index and deletes item based on dropdown value
    public void DeleteSave()
    {
        SaveData.DeleteMapTileset(list.value);
        RefreshTilesets();
        StopDeleting();
    }

    public void OpenTilesetForm(bool isEditing)
    {
        // Cannot edit if no tileset exists
        if (!(isEditing && list.options.Count == 0))
        {
            mapEditorManager.gameObject.SetActive(true);
            mapEditorManager.OpenMapEditor(isEditing, SaveData.GetMapTilesets()[list.value], isEditing ? list.value : -1);
            this.gameObject.SetActive(false);
        }
    }
}
