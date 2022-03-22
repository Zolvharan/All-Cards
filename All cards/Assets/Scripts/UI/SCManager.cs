using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Saved characters manager
public class SCManager : MonoBehaviour
{
    public MMManager mainManager;

    public GameObject saveComponents;

    public Dropdown saveDataList;
    public Text confirmText;
    public Text buttonText;
    public GameObject confirmButton;
    bool isConfirming;
    List<string> saveDataNames;

    CharacterData currCharacter;

    // Inits display elements
    public void InitDisplay(CharacterData data, int isEditingSaveIndex = -1)
    {
        confirmText.text = "";

        // Get dropdown data
        CharacterData[] currCharacters = new CharacterData[SaveData.GetCharacters().Count];
        SaveData.GetCharacters().CopyTo(currCharacters, 0);

        currCharacter = data;
        isConfirming = false;

        buttonText.text = "Save";

        saveDataNames = new List<string>();
        // The first element saves a new character
        saveDataNames.Add("Save new character");
        // Init dropdown list
        foreach (CharacterData character in currCharacters)
        {
            saveDataNames.Add(character.GetName());
        }
        saveDataList.ClearOptions();
        saveDataList.AddOptions(saveDataNames);

        // Set dropdown location to character being edited
        if (isEditingSaveIndex != -1)
        {
            saveDataList.value = isEditingSaveIndex + 1;
        }
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
            SaveData.SaveCharacter(currCharacter, saveDataList.value == 0 ? -1 : saveDataList.value - 1);
            mainManager.OpenCharacters();
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
        mainManager.DisplayCharForm();
        this.gameObject.SetActive(false);
    }
}
