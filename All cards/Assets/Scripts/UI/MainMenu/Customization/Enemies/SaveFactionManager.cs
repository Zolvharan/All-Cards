using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Saved characters manager
public class SaveFactionManager : MonoBehaviour
{
    public EnemyGenManager enemyGenManager;
    public GameObject factionForm;

    public Text confirmText;
    public GameObject newSaveButton;
    public GameObject overwriteButton;
    bool isSavingNew;
    bool isOverwriting;

    FactionData currFaction;
    int currSaveIndex;
    string oldFactionName;

    // Inits display elements
    public void InitDisplay(FactionData data, int isEditingSaveIndex = -1)
    {
        confirmText.text = "";
        currFaction = data;
        isSavingNew = false;
        isOverwriting = false;

        // Cannot overwrite if not editing
        overwriteButton.SetActive(isEditingSaveIndex != -1);
        currSaveIndex = isEditingSaveIndex;
        if (isEditingSaveIndex != -1)
            oldFactionName = SaveData.GetFactions()[isEditingSaveIndex].GetName();
    }

    // Prompts the user to confirm by pressing again
    public void SaveNewData()
    {
        if (isSavingNew)
        {
            SaveData.SaveFaction(currFaction, -1);
            enemyGenManager.OpenEnemies();
            this.gameObject.SetActive(false);
        }
        else
        {
            confirmText.text = "Save new data?";
            isSavingNew = true;
            isOverwriting = false;
        }
    }
    public void OverwriteData()
    {
        if (isOverwriting)
        {
            SaveData.SaveFaction(currFaction, currSaveIndex);
            enemyGenManager.OpenEnemies();
            this.gameObject.SetActive(false);
        }
        else
        {
            confirmText.text = "Overwrite " + oldFactionName + "?";
            isOverwriting = true;
            isSavingNew = false;
        }
    }

    public void Cancel()
    {
        factionForm.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
