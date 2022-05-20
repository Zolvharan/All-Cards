using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Saved units manager
public class SaveUnitManager : MonoBehaviour
{
    public EnemyGenManager enemyGenManager;
    public UnitFormManager unitFormManager;

    public GameObject loadComponents;
    public GameObject saveComponents;

    public Dropdown loadDataList;

    public Dropdown saveDataList;
    public Text confirmText;
    public Text buttonText;
    public GameObject confirmButton;
    bool isConfirming;
    List<string> saveDataNames;

    UnitData currUnit;

    // Return to characters if not in char
    bool inFaction;

    // Inits display elements
    public void InitDisplay(UnitData data, bool isInFaction, int isEditingSaveIndex = -1)
    {
        inFaction = isInFaction;
        confirmText.text = "";

        // Get dropdown data
        UnitData[] currUnits = new UnitData[SaveData.GetUnits().Count];
        SaveData.GetUnits().CopyTo(currUnits, 0);

        // Enter loading mode if data is null
        if (data == null)
        {
            loadComponents.SetActive(true);
            saveComponents.SetActive(false);

            saveDataNames = new List<string>();
            // Init dropdown list
            foreach (UnitData unit in currUnits)
            {
                saveDataNames.Add(unit.GetName());
            }
            loadDataList.ClearOptions();
            loadDataList.AddOptions(saveDataNames);
        }
        // Enter save mode otherwise
        else
        {
            loadComponents.SetActive(false);
            saveComponents.SetActive(true);

            currUnit = data;
            isConfirming = false;

            buttonText.text = "Save";

            saveDataNames = new List<string>();
            // The first element saves a new unit
            saveDataNames.Add("Save new unit");
            // Init dropdown list
            foreach (UnitData unit in currUnits)
            {
                saveDataNames.Add(unit.GetName());
            }
            saveDataList.ClearOptions();
            saveDataList.AddOptions(saveDataNames);
        }

        // Set dropdown location to unit being edited
        if (isEditingSaveIndex != -1)
        {
            saveDataList.value = isEditingSaveIndex + 1;
        }
    }

    // Load mode
    public void LoadData()
    {
        currUnit = SaveData.GetUnits()[loadDataList.value];
        unitFormManager.ReOpenForm(currUnit);
        this.gameObject.SetActive(false);
    }

    // Save mode
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
            SaveData.SaveUnit(currUnit, saveDataList.value == 0 ? -1 : saveDataList.value - 1);

            // Return to form if in char customization, return to front page otherwise
            if (inFaction)
                enemyGenManager.DisplayUnitForm();
            else
                enemyGenManager.OpenEnemies();
            this.gameObject.SetActive(false);
        }
        else
        {
            confirmText.text = saveDataList.value == 0 ? "Save new data?" : "Overwrite " + saveDataList.captionText.text + "?";
            isConfirming = true;
        }
    }

    public void Cancel()
    {
        enemyGenManager.DisplayUnitForm();
        this.gameObject.SetActive(false);
    }
}