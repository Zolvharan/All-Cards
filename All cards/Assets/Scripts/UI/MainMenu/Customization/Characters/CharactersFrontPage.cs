using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Character component manager
// Used for Character selection
public class CharactersFrontPage : MonoBehaviour
{
    // 0 : characters, 1 : abilities, 2 : items
    public Dropdown[] lists;

    public Text deletePrompt;
    public GameObject deleteButton;
    int listDeleteIndex;

    // Manages other character forms as well
    public CharacterForm charForm;
    public AbilityForm abilityForm;
    public CharacterImageForm imageSelection;
    public ItemForm itemForm;

    void OnEnable()
    {
        InitDisplay();
    }

    public void OpenCharacters()
    {
        this.gameObject.SetActive(true);
        InitDisplay();
    }
    // Inits display elements
    void InitDisplay()
    {
        listDeleteIndex = -1;
        deletePrompt.text = "";
        deleteButton.SetActive(false);
        RefreshCharacters();
        RefreshAbilities();
        RefreshItems();
    }

    public void RefreshCharacters()
    {
        List<string> characterNames = new List<string>();
        foreach (CharacterData character in SaveData.GetCharacters())
        {
            characterNames.Add(character.GetName());
        }
        lists[0].ClearOptions();
        lists[0].AddOptions(characterNames);
    }
    public void RefreshAbilities()
    {
        List<string> abilityNames = new List<string>();
        foreach (AbilityData ability in SaveData.GetAbilities())
        {
            abilityNames.Add(ability.GetName());
        }
        lists[1].ClearOptions();
        lists[1].AddOptions(abilityNames);
    }
    public void RefreshItems()
    {
        List<string> itemNames = new List<string>();
        foreach (ItemData item in SaveData.GetItems())
        {
            itemNames.Add(item.GetName());
        }
        lists[2].ClearOptions();
        lists[2].AddOptions(itemNames);
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
                SaveData.DeleteCharacter(lists[listDeleteIndex].value);
                RefreshCharacters();
                break;
            case 1:
                SaveData.DeleteAbility(lists[listDeleteIndex].value);
                RefreshAbilities();
                break;
            case 2:
                SaveData.DeleteItem(lists[listDeleteIndex].value);
                RefreshItems();
                break;
        }
        StopDeleting();
    }

    // Other form managing methods
    public void OpenCharForm(bool isEditing)
    {
        charForm.OpenCreationForm(isEditing, lists[0]);
        this.gameObject.SetActive(false);
    }
    // Primary initialization is done in CharacterForm
    public void DisplayCharForm()
    {
        charForm.gameObject.SetActive(true);
        charForm.RefreshAbilities();
        this.gameObject.SetActive(false);
        abilityForm.gameObject.SetActive(false);
    }

    // Primary initialization is done in AbilityForm
    public void OpenAbilityForm(bool isEditing)
    {
        if (!isEditing || lists[1].options.Count != 0)
        {
            abilityForm.OpenAbilityForm(isEditing, lists[1], SaveData.GetAbilities()[lists[1].value], this.gameObject, false);
            this.gameObject.SetActive(false);
        }
    }
    // Used by charForm
    public void OpenAbilityFormInCharacter(bool isEditing)
    {
        if (!isEditing || charForm.GetAbility() != null)
        {
            abilityForm.OpenAbilityForm(isEditing, lists[1], charForm.GetAbility(), this.gameObject, true, charForm, charForm.GetAbilityStats());
            charForm.gameObject.SetActive(false);
        }
    }

    public void OpenItemForm()
    {
        itemForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
