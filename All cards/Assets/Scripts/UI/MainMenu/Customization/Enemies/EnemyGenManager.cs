using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGenManager : MonoBehaviour
{
    public FactionFormManager factionformManager;
    public UnitFormManager unitFormManager;
    public AbilityForm abilityForm;
    public SaveFactionManager factionSaves;

    public Dropdown[] lists;

    public Text deletePrompt;
    public GameObject deleteButton;
    int listDeleteIndex;

    void OnEnable()
    {
        InitDisplay();
    }
    public void OpenEnemies()
    {
        this.gameObject.SetActive(true);
        InitDisplay();
    }
    // Inits display elements
    public void InitDisplay()
    {
        listDeleteIndex = -1;
        deletePrompt.text = "";
        deleteButton.SetActive(false);
        RefreshFactions();
        RefreshUnits();
        RefreshAbilities();
    }

    public void RefreshFactions()
    {
        List<string> factionNames = new List<string>();
        foreach (FactionData faction in SaveData.GetFactions())
        {
            factionNames.Add(faction.GetName());
        }
        lists[0].ClearOptions();
        lists[0].AddOptions(factionNames);
    }
    public void RefreshUnits()
    {
        List<string> unitNames = new List<string>();
        foreach (UnitData unit in SaveData.GetUnits())
        {
            unitNames.Add(unit.GetName());
        }
        lists[1].ClearOptions();
        lists[1].AddOptions(unitNames);
    }
    public void RefreshAbilities()
    {
        List<string> abilityNames = new List<string>();
        foreach (AbilityData ability in SaveData.GetAbilities())
        {
            abilityNames.Add(ability.GetName());
        }
        lists[2].ClearOptions();
        lists[2].AddOptions(abilityNames);
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
                SaveData.DeleteFaction(lists[listDeleteIndex].value);
                RefreshFactions();
                break;
            case 1:
                SaveData.DeleteUnit(lists[listDeleteIndex].value);
                RefreshUnits();
                break;
            case 2:
                SaveData.DeleteAbility(lists[listDeleteIndex].value);
                RefreshAbilities();
                break;
        }
        StopDeleting();
    }

    // Form managing
    public void OpenFactionForm(bool isEditing)
    {
        // Cannot edit if no faction exists
        if (!(isEditing && lists[0].options.Count == 0))
        {
            factionformManager.gameObject.SetActive(true);
            factionformManager.InitDisplay(isEditing);
            this.gameObject.SetActive(false);
        }
    }
    public void DisplayFactionForm()
    {
        factionformManager.gameObject.SetActive(true);
    }
    public void OpenUnitForm(bool isEditing)
    {
        // Cannot edit if no unit exists
        if (!(isEditing && lists[1].options.Count == 0))
        {
            unitFormManager.gameObject.SetActive(true);
            unitFormManager.OpenCreationForm(isEditing);
            this.gameObject.SetActive(false);
        }
    }
    public void DisplayUnitForm()
    {
        unitFormManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void OpenAbilityForm(bool isEditing)
    {
        if (!isEditing || lists[1].options.Count != 0)
        {
            abilityForm.OpenAbilityForm(isEditing, lists[1], SaveData.GetAbilities()[lists[1].value], this.gameObject, false);
            this.gameObject.SetActive(false);
        }
    }
    public void OpenAbilityFormInCharacter(bool isEditing)
    {
        if (!isEditing || unitFormManager.GetAbility() != null)
        {
            abilityForm.OpenAbilityForm(isEditing, lists[1], unitFormManager.GetAbility(), this.gameObject, true, unitFormManager, unitFormManager.GetAbilityStats());
            unitFormManager.gameObject.SetActive(false);
        }
    }
    public void DisplayAbilityForm()
    {
        abilityForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void OpenSavedFactionData(FactionData data, int isEditingSaveIndex = -1)
    {
        factionformManager.gameObject.SetActive(false);
        factionSaves.gameObject.SetActive(true);
        factionSaves.InitDisplay(data, isEditingSaveIndex);
    }
}
