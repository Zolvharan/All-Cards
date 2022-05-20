using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Saved enemy abilities manager
public class SaveEnemyAbilityMgr : MonoBehaviour
{
    public EnemyGenManager enemyGenManager;
    public UnitAbilityManager abilityFormManager;

    public GameObject loadComponents;
    public GameObject saveComponents;

    public Dropdown loadDataList;

    public Dropdown saveDataList;
    public Text confirmText;
    public Text buttonText;
    public GameObject confirmButton;
    bool isConfirming;
    List<string> saveDataNames;

    AbilityData currAbility;

    // Return to characters if not in char
    bool inChar;

    // Inits display elements
    public void InitDisplay(AbilityData data, bool isInChar, int isEditingSaveIndex = -1)
    {
        inChar = isInChar;
        confirmText.text = "";

        // Get dropdown data
        AbilityData[] currAbilities = new AbilityData[SaveData.GetAbilities().Count];
        SaveData.GetAbilities().CopyTo(currAbilities, 0);

        // Enter loading mode if data is null
        if (data == null)
        {
            loadComponents.SetActive(true);
            saveComponents.SetActive(false);

            saveDataNames = new List<string>();
            // Init dropdown list
            foreach (AbilityData ability in currAbilities)
            {
                saveDataNames.Add(ability.GetName());
            }
            loadDataList.ClearOptions();
            loadDataList.AddOptions(saveDataNames);
        }
        // Enter save mode otherwise
        else
        {
            loadComponents.SetActive(false);
            saveComponents.SetActive(true);

            currAbility = data;
            isConfirming = false;

            buttonText.text = "Save";

            saveDataNames = new List<string>();
            // The first element saves a new ability
            saveDataNames.Add("Save new ability");
            // Init dropdown list
            foreach (AbilityData ability in currAbilities)
            {
                saveDataNames.Add(ability.GetName());
            }
            saveDataList.ClearOptions();
            saveDataList.AddOptions(saveDataNames);
        }

        // Set dropdown location to ability being edited
        if (isEditingSaveIndex != -1)
        {
            saveDataList.value = isEditingSaveIndex + 1;
        }
    }

    // Load mode
    public void LoadData()
    {
        currAbility = SaveData.GetAbilities()[loadDataList.value];
        abilityFormManager.ReOpenFormWithLoad(currAbility);
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
            SaveData.SaveAbility(currAbility, saveDataList.value == 0 ? -1 : saveDataList.value - 1);

            // Return to form if in char customization, return to front page otherwise
            if (inChar)
                enemyGenManager.DisplayAbilityForm();
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
        enemyGenManager.DisplayAbilityForm();
        this.gameObject.SetActive(false);
    }
}