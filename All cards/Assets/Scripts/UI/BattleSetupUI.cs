using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSetupUI : MonoBehaviour
{
    public MMManager mainManager;
    public EquipFormUI equipForm;

    public Dropdown[] charLists;
    List<Item>[] currItemsArray;
    public Transform basePrefab;

    public LevelGenerator generator;
    public PlayerControl player;

    public void InitDisplay()
    {
        currItemsArray = new List<Item>[charLists.Length];
        List<CharacterData> allCharacters = SaveData.GetCharacters();
        List<string> charNames = new List<string>();
        charNames.Add("Empty");
        foreach (CharacterData character in allCharacters)
        {
            charNames.Add(character.characterName);
        }

        foreach (Dropdown list in charLists)
        {
            list.ClearOptions();
            list.AddOptions(charNames);
            list.value = 0;
        }

        for (int i = 0; i < currItemsArray.Length; i++)
        {
            currItemsArray[i] = new List<Item>();
        }
    }

    public void StartCombat()
    {
        // Close menu
        mainManager.StartCombat();

        // Construct set of new character objects
        List<CharacterStats> newCharacters = new List<CharacterStats>();

        for (int i = 0; i < charLists.Length; i++)
        {
            // If not empty
            if (charLists[i].value != 0)
            {
                // Create object, initialize character stats and add to list
                Transform newCharacter = Instantiate(basePrefab);
                SaveData.GetCharacters()[charLists[i].value - 1].ConstructCharacter(newCharacter.GetComponent<CharacterStats>(), currItemsArray[i]);
                newCharacters.Add(newCharacter.GetComponent<CharacterStats>());
            }
        }

        generator.GenerateLevel(15, 10, newCharacters);
        player.StartTurn();
    }

    // Equip form methds
    public void OpenEquipForm(int currEquipIndex)
    {
        equipForm.gameObject.SetActive(true);
        equipForm.InitDisplay(currEquipIndex);
        this.gameObject.SetActive(false);
    }
    public List<Item> GetItems(int itemsIndex)
    {
        return currItemsArray[itemsIndex];
    }
    // Takes an item list and adds it to currItemsArray
    public void SetItems(List<Item> items, int itemsIndex)
    {
        currItemsArray[itemsIndex] = items;
    }
}
