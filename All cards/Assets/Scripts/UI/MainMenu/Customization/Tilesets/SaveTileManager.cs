using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveTileManager : MonoBehaviour
{
    public GameObject loadComponents;
    public GameObject saveComponents;

    public Dropdown loadDataList;

    public Dropdown saveDataList;
    public Text confirmText;
    public GameObject saveButton;
    public GameObject confirmButton;

    // Inits display elements
    public void InitDisplay(List<string> saveDataNames, bool isLoading, int isEditingSaveIndex = -1)
    {
        confirmText.text = "";

        // Enter loading mode
        if (isLoading)
        {
            loadComponents.SetActive(true);
            saveComponents.SetActive(false);

            loadDataList.ClearOptions();
            loadDataList.AddOptions(saveDataNames);
        }
        // Enter save mode
        else
        {
            loadComponents.SetActive(false);
            saveComponents.SetActive(true);

            saveButton.SetActive(true);
            confirmButton.SetActive(false);

            // The first element saves a new unit
            saveDataNames.Insert(0, "Save new data");
            saveDataList.ClearOptions();
            saveDataList.AddOptions(saveDataNames);
        }

        // Set dropdown location to unit being edited
        if (isEditingSaveIndex != -1)
        {
            saveDataList.value = isEditingSaveIndex + 1;
        }
    }

    public void DropdownChange()
    {
        confirmText.text = "";
        saveButton.SetActive(saveDataList.value == 0);
        confirmButton.SetActive(!saveButton.activeSelf);
    }
    // Prompts the user to confirm by pressing again, if button has not been pressed since last dropdown interaction
    public void Confirm()
    {
        confirmText.text = saveDataList.value == 0 ? "Save new data?" : "Overwrite " + saveDataList.captionText.text + "?";
        saveButton.SetActive(false);
        confirmButton.SetActive(true);
    }

    public int GetLoadValue()
    {
        return loadDataList.value;
    }
    public int GetSaveValue()
    {
        return saveDataList.value;
    }
}
