using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the character creation and editing menus
public class CharacterForm : CreationForm
{
    public GameObject charactersFrontPage;
    public CharacterImageForm imageSelectionManager;
    public SaveLoadForm saveLoadForm;

    public InputField nameField;
    public Image portrait;
    protected byte[] portraitData;
    public Image battleSprite;
    protected byte[] battleSpriteData;

    protected int maxCapacity;
    const int MAX_CAPACITY = 50;
    protected int currCapacity;
    public Text capacityText;
    public Toggle flyingToggle;
    protected const int FLYING_COST = 5;
    protected const int ABILITY_CAPACITY_COST = 2;
    public Text[] statTextNums;
    protected int[] statNums;
    // The highest and lowest numbers that stats can be
    // order is: MoveSpeed, AttackRange, Strength, EnergyRegen, Precision, Dexterity, Defense, Resistance
    protected readonly int[] MIN_STAT_NUMS = { 0, 0, 0, 0, 0, 0, 0, 0 };
    protected readonly int[] MAX_STAT_NUMS = { 10, 10, 10, 5, 10, 5, 5, 5 };
    protected readonly int[] DEFAULT_STAT_NUMS = { 1, 1, 1, 1, 0, 0, 0, 0 };

    public Dropdown abilityList;
    public List<AbilityData> currAbilities;

    protected bool isPortrait;
    protected const string PORTRAIT_PATH = ".\\SavedData\\Portraits";
    protected const string BATTLE_SPRITE_PATH = ".\\SavedData\\BattleSprites";

    protected bool editing;
    protected bool removingAbility;
    public Text alertText;

    protected Dropdown currDropdown;

    void OnEnable()
    {
        RefreshAbilities();
    }
    public void OpenCreationForm(bool isEditing, Dropdown newDropdown)
    {
        maxCapacity = MAX_CAPACITY;
        currDropdown = newDropdown;
        if (isEditing)
        {
            // Cannot edit if no characters are present
            if (currDropdown.options.Count == 0)
                return;

            editing = true;
            // Get character selected
            CharacterData currCharacter = SaveData.GetCharacters()[currDropdown.value];

            nameField.text = currCharacter.GetName();
            statNums = new int[currCharacter.stats.Length];
            currCharacter.stats.CopyTo(statNums, 0);
            flyingToggle.isOn = currCharacter.flying;
            currAbilities = new List<AbilityData>();

            // Set ability dropdown
            for (int i = 0; i < currCharacter.abilities.Length; i++)
            {
                currAbilities.Add(currCharacter.abilities[i]);
            }

            // Set image data
            portraitData = currCharacter.portrait;
            battleSpriteData = currCharacter.battleSprite;
            portrait.sprite = CharacterImageForm.ConstructImage(portraitData);
            battleSprite.sprite = CharacterImageForm.ConstructImage(battleSpriteData);
        }
        else
        {
            editing = false;
            nameField.text = "";
            statNums = new int[8];
            DEFAULT_STAT_NUMS.CopyTo(statNums, 0);
            flyingToggle.isOn = false;
            currAbilities = new List<AbilityData>();

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

        this.gameObject.SetActive(true);
    }
    public void ReturnToFront()
    {
        charactersFrontPage.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void SaveItem()
    {
        // Turn ability list into array
        AbilityData[] abilitiesToSave = new AbilityData[currAbilities.Count];
        currAbilities.CopyTo(abilitiesToSave);
        CharacterData newCharacter = new CharacterData(nameField.text, abilitiesToSave, portraitData, battleSpriteData, statNums, flyingToggle.isOn);
        SaveData.SaveCharacter(newCharacter, editing ? currDropdown.value : -1);
        charactersFrontPage.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public virtual void OpenSaveForm()
    {
        // Save is forbidden if curr capacity exceeds capacity
        if (currCapacity > maxCapacity)
        {
            removingAbility = false;
            alertText.text = "Capacity exceeded, cannot save.";
        }
        else
        {
            List<string> names = new List<string>();
            foreach (CharacterData character in SaveData.GetCharacters())
            {
                names.Add(character.GetName());
            }

            // If editing, init save location to character being edited
            saveLoadForm.gameObject.SetActive(true);
            saveLoadForm.InitDisplay(this, names, false, editing ? currDropdown.value : -1);
            this.gameObject.SetActive(false);
        }
    }

    public void OpenImages(bool openPortrait)
    {
        isPortrait = openPortrait;
        imageSelectionManager.gameObject.SetActive(true);
        imageSelectionManager.InitDisplay(isPortrait ? PORTRAIT_PATH : BATTLE_SPRITE_PATH, this);
        this.gameObject.SetActive(false);
    }
    // Sets portrait or battleSprite if saving, and closes image selection
    public override void SetImage(bool saveChanges, Sprite newSprite, byte[] newData)
    {
        this.gameObject.SetActive(true);

        if (saveChanges)
        {
            if (isPortrait)
            {
                portrait.sprite = newSprite;
                portraitData = newData;
            }
            else
            {
                battleSprite.sprite = newSprite;
                battleSpriteData = newData;
            }
        }
    }

    public AbilityData GetAbility()
    {
        if (abilityList.options.Count != 0)
            return currAbilities[abilityList.value];
        else
            return null;
    }
    public int[] GetAbilityStats()
    {
        int[] abilityStats = {statNums[2], statNums[1], statNums[4]};
        return abilityStats;
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

    public int GetStrength()
    {
        return statNums[2];
    }
    public int GetAttackRange()
    {
        return statNums[1];
    }
    public int GetPrecision()
    {
        return statNums[4];
    }
}
