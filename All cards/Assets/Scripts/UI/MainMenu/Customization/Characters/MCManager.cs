using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the character creation and editing menus
public class MCManager : MonoBehaviour
{
    public CCManager ccManager;
    public CIManager imageSelectionManager;

    public InputField nameField;
    public Image portrait;
    byte[] portraitData;
    public Image battleSprite;
    byte[] battleSpriteData;

    const int MAX_CAPACITY = 50;
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

    public void OpenCreationForm(bool isEditing)
    {
        if (isEditing)
        {
            // Cannot edit if no characters are present
            if (ccManager.lists[0].options.Count == 0)
                return;

            editing = true;
            // Get character selected
            CharacterData currCharacter = SaveData.GetCharacters()[ccManager.lists[0].value];

            nameField.text = currCharacter.characterName;
            statNums = new int[currCharacter.stats.Length];
            currCharacter.stats.CopyTo(statNums, 0);
            flyingToggle.isOn = currCharacter.flying;
            currAbilities = new List<AbilityData>();

            // Set ability dropdown
            for (int i = 0; i < currCharacter.abilities.Length; i++)
            {
                currAbilities.Add(currCharacter.abilities[i]);
            }
            RefreshAbilities();

            // Set image data
            portraitData = currCharacter.portrait;
            battleSpriteData = currCharacter.battleSprite;
            portrait.sprite = CharacterData.ConstructImage(portraitData);
            battleSprite.sprite = CharacterData.ConstructImage(battleSpriteData);
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
        capacityText.text = currCapacity.ToString() + '/' + MAX_CAPACITY;

        removingAbility = false;
        alertText.text = "";

        ccManager.DisplayCharForm();
    }

    public void SaveCharacter()
    {
        // Save is forbidden if curr capacity exceeds capacity
        if (currCapacity > MAX_CAPACITY)
        {
            removingAbility = false;
            alertText.text = "Capacity exceeded, cannot save.";
        }
        else
        {
            // Turn ability list into array
            AbilityData[] abilitiesToSave = new AbilityData[currAbilities.Count];
            currAbilities.CopyTo(abilitiesToSave);
            CharacterData newCharacter = new CharacterData(nameField.text, abilitiesToSave, portraitData, battleSpriteData, statNums, flyingToggle.isOn);

            // If editing, init save location to character being edited
            ccManager.DisplaySavedCharacterData(newCharacter, editing ? ccManager.lists[0].value : -1);
        }
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
        capacityText.text = currCapacity.ToString() + '/' + MAX_CAPACITY;
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
            capacityText.text = (++currCapacity).ToString() + '/' + MAX_CAPACITY;
        }
    }
    public void DecrementStat(int statIndex)
    {
        if (statNums[statIndex] > MIN_STAT_NUMS[statIndex])
        {
            statNums[statIndex]--;
            statTextNums[statIndex].text = statNums[statIndex].ToString();
            capacityText.text = (--currCapacity).ToString() + '/' + MAX_CAPACITY;
        }
    }
    // Used by flying toggle
    public void ToggleFlying(Toggle flying)
    {
        capacityText.text = (currCapacity += flying.isOn ? FLYING_COST : -FLYING_COST).ToString() + '/' + MAX_CAPACITY;
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
