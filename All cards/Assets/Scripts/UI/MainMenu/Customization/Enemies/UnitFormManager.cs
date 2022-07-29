using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the character creation and editing menus
public class UnitFormManager : MonoBehaviour
{
    public EnemyGenManager enemyGenManager;
    public FactionFormManager factionFormManager;
    public CIManager imageSelectionManager;

    public InputField nameField;
    public Image portrait;
    byte[] portraitData;
    public Image battleSprite;
    byte[] battleSpriteData;

    int maxCapacity;
    public Dropdown enemyTypes;
    int currCapacity;
    public Text capacityText;
    public Toggle flyingToggle;
    const int FLYING_COST = 5;
    const int ABILITY_CAPACITY_COST = 2;
    public Text[] statTextNums;
    int[] statNums;
    // The highest and lowest numbers that stats can be
    // order is: MoveSpeed, AttackRange, Strength, EnergyRegen, Precision, Dexterity, Defense, Resistance
    readonly int[] MIN_STAT_NUMS = { 0, 0, 0, 0, 0, 0, 0, 0 };
    readonly int[] MAX_STAT_NUMS = { 10, 10, 10, 5, 10, 5, 5, 5 };
    readonly int[] DEFAULT_STAT_NUMS = { 1, 1, 1, 1, 0, 0, 0, 0 };

    public Dropdown abilityList;
    public List<AbilityData> currAbilities;

    bool isPortrait;
    const string PORTRAIT_PATH = ".\\SavedData\\Portraits";
    const string BATTLE_SPRITE_PATH = ".\\SavedData\\BattleSprites";

    bool editing;
    bool removingAbility;
    public Text alertText;

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
            portrait.sprite = CharacterData.ConstructImage(portraitData);
            battleSprite.sprite = CharacterData.ConstructImage(battleSpriteData);

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
            portraitData = CIManager.GetTheEmpty();
            portrait.sprite = CharacterData.ConstructImage(portraitData);
            battleSpriteData = CIManager.GetTheEmpty();
            battleSprite.sprite = CharacterData.ConstructImage(battleSpriteData);
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
                }
            }
            enemyGenManager.DisplayFactionForm();
            this.gameObject.SetActive(false);
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

    public void SaveUnit()
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
            // If editing from main unit list, init save location to ability being edited
            enemyGenManager.OpenSavedUnitData(BuildUnit(), isInFaction, !isInFaction && editing ? enemyGenManager.lists[1].value : -1);
        }
    }
    // Takes form data and builds unit
    public UnitData BuildUnit()
    {
        // Turn ability list into array
        AbilityData[] abilitiesToSave = new AbilityData[currAbilities.Count];
        currAbilities.CopyTo(abilitiesToSave);
        UnitData newUnit = new UnitData(nameField.text, abilitiesToSave, portraitData, battleSpriteData, statNums, flyingToggle.isOn, enemyTypes.value + 1);

        return newUnit;
    }
    // Opens unit saving form in load
    public void LoadUnit()
    {
        enemyGenManager.OpenSavedUnitData(null, isInFaction);
    }

    public void OpenImages(bool openPortrait)
    {
        isPortrait = openPortrait;
        imageSelectionManager.gameObject.SetActive(true);
        imageSelectionManager.InitDisplay(isPortrait ? PORTRAIT_PATH : BATTLE_SPRITE_PATH);
        this.gameObject.SetActive(false);
    }
    // Sets portrait or battleSprite if saving, and closes image selection
    public void SetImage(bool saveChanges)
    {
        this.gameObject.SetActive(true);
        Sprite newImage = imageSelectionManager.GetCurrImage();
        byte[] newData = imageSelectionManager.GetCurrData();
        imageSelectionManager.ExitImages();

        if (saveChanges)
        {
            if (isPortrait)
            {
                portrait.sprite = newImage;
                portraitData = newData;
            }
            else
            {
                battleSprite.sprite = newImage;
                battleSpriteData = newData;
            }
        }
    }

    // Refreshes ability list when form is opened
    public void RefreshAbilities()
    {
        abilityList.ClearOptions();
        List<string> currAbilityNames = new List<string>();
        for (int i = 0; i < currAbilities.Count; i++)
        {
            currAbilityNames.Add(currAbilities[i].GetName());
        }
        abilityList.AddOptions(currAbilityNames);
    }

    public void RemoveAbility()
    {
        // Ignore if no abilities exist
        if (currAbilities.Count > 0)
        {
            // User has to press button twice to remove
            if (removingAbility == false)
            {
                removingAbility = true;
                alertText.text = "Remove " + currAbilities[abilityList.value].GetName() + "?";
            }
            else
            {
                removingAbility = false;
                alertText.text = "";

                currAbilities.RemoveAt(abilityList.value);
                List<string> currAbilityNames = new List<string>();
                for (int i = 0; i < currAbilities.Count; i++)
                {
                    currAbilityNames.Add(currAbilities[i].GetName());
                }
                abilityList.ClearOptions();
                abilityList.AddOptions(currAbilityNames);
                AdjustCapacity(false);
            }
        }
    }
    // Adds and subracts capacity when ability is added or removed and displays current capacity
    public void AdjustCapacity(bool added)
    {
        currCapacity += added ? ABILITY_CAPACITY_COST : -ABILITY_CAPACITY_COST;
        capacityText.text = currCapacity.ToString() + '/' + maxCapacity;
    }

    // Remove halts when dropdown changes or when save is attempted
    public void StopRemoving()
    {
        removingAbility = false;
        alertText.text = "";
    }

    // Used by -+ buttons
    public void IncrementStat(int statIndex)
    {
        if (statNums[statIndex] < MAX_STAT_NUMS[statIndex])
        {
            statNums[statIndex]++;
            statTextNums[statIndex].text = statNums[statIndex].ToString();
            capacityText.text = (++currCapacity).ToString() + '/' + maxCapacity;
        }
    }
    public void DecrementStat(int statIndex)
    {
        if (statNums[statIndex] > MIN_STAT_NUMS[statIndex])
        {
            statNums[statIndex]--;
            statTextNums[statIndex].text = statNums[statIndex].ToString();
            capacityText.text = (--currCapacity).ToString() + '/' + maxCapacity;
        }
    }
    // Used by flying toggle
    public void ToggleFlying(Toggle flying)
    {
        capacityText.text = (currCapacity += flying.isOn ? FLYING_COST : -FLYING_COST).ToString() + '/' + maxCapacity;
    }
    public void UpdateEnemyType()
    {
        maxCapacity = ENEMY_LEVEL_CAPACITIES[enemyTypes.value];
        capacityText.text = currCapacity.ToString() + '/' + maxCapacity;
    }

    public int GetStrength()
    {
        return statNums[2];
    }
    public int GetAttackRange()
    {
        return statNums[0];
    }
    public int GetPrecision()
    {
        return statNums[4];
    }
}