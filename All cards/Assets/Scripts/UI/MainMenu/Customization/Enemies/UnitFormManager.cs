using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the character creation and editing menus
public class UnitFormManager : CharacterForm
{
    public EnemyGenManager enemyGenManager;
    public FactionFormManager factionFormManager;

    public Dropdown enemyTypes;

    bool isInFaction;
    // Can only load and apply if in faction
    public GameObject factionComponents;
    readonly int[] ENEMY_LEVEL_CAPACITIES = { 10, 20, 40 };

    public void OpenCreationForm(bool isEditing)
    {
        isInFaction = false;
        InitForm(isEditing);
    }
    public void OpenCreationFormInFaction(bool isEditing)
    {
        // Cannot open if editing and no unit exists
        if (!(isEditing && factionFormManager.unitList.options.Count == 0))
        {
            isInFaction = true;
            factionFormManager.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
            InitForm(isEditing);
        }
    }
    void InitForm(bool isEditing)
    {
        factionComponents.SetActive(isInFaction);

        if (isEditing)
        {
            editing = true;
            // Get unit selected
            UnitData currUnit;
            if (isInFaction)
                currUnit = factionFormManager.currUnits[factionFormManager.unitList.value];
            else
                currUnit = SaveData.GetUnits()[enemyGenManager.lists[1].value];

            nameField.text = currUnit.GetName();
            statNums = new int[currUnit.stats.Length];
            currUnit.stats.CopyTo(statNums, 0);
            flyingToggle.isOn = currUnit.flying;
            currAbilities = new List<AbilityData>();

            // Set ability dropdown
            for (int i = 0; i < currUnit.abilities.Length; i++)
            {
                currAbilities.Add(currUnit.abilities[i]);
            }
            RefreshAbilities();

            // Set image data
            portraitData = currUnit.portrait;
            battleSpriteData = currUnit.battleSprite;
            portrait.sprite = CharacterImageForm.ConstructImage(portraitData);
            battleSprite.sprite = CharacterImageForm.ConstructImage(battleSpriteData);

            maxCapacity = ENEMY_LEVEL_CAPACITIES[currUnit.GetEnemyLevel() - 1];
            enemyTypes.value = currUnit.GetEnemyLevel() - 1;
        }
        else
        {
            editing = false;
            nameField.text = "";
            statNums = new int[8];
            DEFAULT_STAT_NUMS.CopyTo(statNums, 0);
            flyingToggle.isOn = false;
            currAbilities = new List<AbilityData>();
            RefreshAbilities();
            maxCapacity = ENEMY_LEVEL_CAPACITIES[0];
            enemyTypes.value = 0;

            // Init images
            portraitData = CharacterImageForm.GetTheEmpty();
            portrait.sprite = CharacterImageForm.ConstructImage(portraitData);
            battleSpriteData = CharacterImageForm.GetTheEmpty();
            battleSprite.sprite = CharacterImageForm.ConstructImage(battleSpriteData);
        }
        // Initialize stat nums, and curr capacity
        currCapacity = 0;
        for (int i = 0; i < statNums.Length; i++)
        {
            statTextNums[i].text = statNums[i].ToString();
            currCapacity += statNums[i];
        }
        if (flyingToggle.isOn)
            currCapacity += 4;
        capacityText.text = currCapacity.ToString() + '/' + maxCapacity;

        removingAbility = false;
        alertText.text = "";
    }
    // Save changes used by faction
    public void ExitForm(bool saveChanges)
    {
        if (isInFaction)
        {
            if (saveChanges)
            {
                // Save is forbidden if curr capacity exceeds or is under capacity
                if (currCapacity != maxCapacity)
                {
                    removingAbility = false;
                    if (currCapacity > maxCapacity)
                        alertText.text = "Capacity exceeded, cannot save.";
                    else
                        alertText.text = "Capacity not reached, cannot save";
                }
                else
                {
                    if (editing)
                        factionFormManager.currUnits[factionFormManager.unitList.value] = BuildUnit();
                    else
                        factionFormManager.currUnits.Add(BuildUnit());
                    factionFormManager.RefreshUnits();
                    enemyGenManager.DisplayFactionForm();
                    this.gameObject.SetActive(false);
                }
            }
            else
            {
                enemyGenManager.DisplayFactionForm();
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            enemyGenManager.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    public void ReOpenForm(UnitData newUnit)
    {
        if (editing)
            factionFormManager.currUnits[factionFormManager.unitList.value] = newUnit;
        else
            factionFormManager.currUnits.Add(newUnit);
        factionFormManager.RefreshUnits();

        factionFormManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void SaveItem()
    {
        SaveData.SaveUnit(BuildUnit(), saveLoadForm.GetSaveValue() == 0 ? -1 : saveLoadForm.GetSaveValue() - 1);
        this.gameObject.SetActive(true);
    }
    public override void LoadItem()
    {
        ReOpenForm(SaveData.GetUnits()[saveLoadForm.GetLoadValue()]);
        this.gameObject.SetActive(false);
    }
    public override void OpenSaveForm()
    {
        // Save is forbidden if curr capacity exceeds or is under capacity
        if (currCapacity != maxCapacity)
        {
            removingAbility = false;
            if (currCapacity > maxCapacity)
                alertText.text = "Capacity exceeded, cannot save.";
            else
                alertText.text = "Capacity not reached, cannot save";
        }
        else
        {
            saveLoadForm.gameObject.SetActive(true);
            List<string> names = new List<string>();
            foreach (UnitData unit in SaveData.GetUnits())
            {
                names.Add(unit.GetName());
            }
            // If editing from main unit list, init save location to ability being edited
            saveLoadForm.InitDisplay(this, names, false, !isInFaction && editing ? enemyGenManager.lists[1].value : -1);
            this.gameObject.SetActive(false);
        }
    }
    // Takes form data and builds unit
    UnitData BuildUnit()
    {
        // Turn ability list into array
        AbilityData[] abilitiesToSave = new AbilityData[currAbilities.Count];
        currAbilities.CopyTo(abilitiesToSave);
        UnitData newUnit = new UnitData(nameField.text, abilitiesToSave, portraitData, battleSpriteData, statNums, flyingToggle.isOn, enemyTypes.value + 1);

        return newUnit;
    }
    // Opens unit saving form in load
    public void OpenLoadForm()
    {
        saveLoadForm.gameObject.SetActive(true);
        List<string> names = new List<string>();
        foreach (UnitData unit in SaveData.GetUnits())
        {
            names.Add(unit.GetName());
        }
        saveLoadForm.InitDisplay(this, names, true);
        this.gameObject.SetActive(false);
    }

    public void UpdateEnemyType()
    {
        maxCapacity = ENEMY_LEVEL_CAPACITIES[enemyTypes.value];
        capacityText.text = currCapacity.ToString() + '/' + maxCapacity;
    }
}