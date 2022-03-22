using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SIManager : MonoBehaviour
{
    public MMManager mainManager;

    public Dropdown saveDataList;
    public Text confirmText;
    public Text buttonText;
    public GameObject confirmButton;
    bool isConfirming;
    List<string> saveDataNames;

    ItemData currItem;

    // Inits display elements
    public void InitDisplay(ItemData data)
    {
        confirmText.text = "";

        // Get dropdown data
        ItemData[] currItems = new ItemData[SaveData.GetItems().Count];
        SaveData.GetItems().CopyTo(currItems, 0);
        isConfirming = false;
        buttonText.text = "Save";

        saveDataNames = new List<string>();
        // The first element saves a new item
        saveDataNames.Add("Save new item");
        // Init dropdown list
        foreach (ItemData item in currItems)
        {
            saveDataNames.Add(item.GetName());
        }
        saveDataList.ClearOptions();
        saveDataList.AddOptions(saveDataNames);

        currItem = data;
    }

    public void DropdownChange()
    {
        isConfirming = false;
        confirmText.text = "";
        buttonText.text = saveDataList.value == 0 ? "Save" : "Overwrite";
    }
    // Prompts the user to confirm by pressing again, if button has not been pressed since last dropdown interaction
    public void Confirm()
    {
        if (isConfirming)
        {
            SaveData.SaveItem(currItem, saveDataList.value == 0 ? -1 : saveDataList.value - 1);
            // Use cancel function to return
            Cancel();
        }
        else
        {
            confirmText.text = saveDataList.value == 0 ? "Save new data?" : "Overwrite " + saveDataList.captionText.text + "?";
            isConfirming = true;
        }
    }

    public void Cancel()
    {
        mainManager.OpenCharacters();
        this.gameObject.SetActive(false);
    }
}
