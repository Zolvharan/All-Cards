using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSetupUI : MonoBehaviour
{
    public MMManager mainManager;
    public EquipFormUI equipForm;

    public Dropdown charList;
    List<Item>[] currItemsArray;
    public Dropdown factionList;
    public Dropdown enemyList;
    public Transform baseCharPrefab;
    public Transform baseEnemyPrefab;

    public Dropdown tileTypeList;
    public Dropdown tilesetList;

    int[] charIndexes;
    int currCharIndex;
    const int MAX_CHARACTERS = 6;
    public Text selectCharText;
    int[] enemyIndexes;
    int currEnemyIndex;
    const int MAX_ENEMIES = 8;
    public Text selectEnemyText;

    bool isSelectingCharacter;
    bool isSelectingEnemy;

    public LevelGenerator generator;

    public Text alertText;

    public void InitDisplay()
    {
        alertText.text = "";
        selectCharText.text = "Select";
        selectEnemyText.text = "Select";
        isSelectingCharacter = false;
        isSelectingEnemy = false;

        charIndexes = new int[MAX_CHARACTERS];
        // Initialize indexes to -1 (empty)
        for (int i = 0; i < MAX_CHARACTERS; i++)
        {
            charIndexes[i] = -1;
        }
        enemyIndexes = new int[MAX_ENEMIES];

        currItemsArray = new List<Item>[MAX_CHARACTERS];

        List<string> charNames = new List<string>();
        for (int i = 0; i < MAX_CHARACTERS; i++)
        {
            charNames.Add("Empty");
        }
        charList.ClearOptions();
        charList.AddOptions(charNames);
        charList.value = 0;

        for (int i = 0; i < currItemsArray.Length; i++)
        {
            currItemsArray[i] = new List<Item>();
        }

        // Init faction list
        List<FactionData> allFactions = SaveData.GetFactions();
        List<string> factionNames = new List<string>();
        foreach (FactionData faction in allFactions)
        {
            factionNames.Add(faction.GetName());
        }
        factionList.ClearOptions();
        factionList.AddOptions(factionNames);
        factionList.value = 0;
        SelectFaction();

        // Init tileset selection
        SetTileType();
    }

    public void StartCombat()
    {
        // Needs at least one player character and one enemy
        bool characterReady = false;
        bool enemyReady = false;
        foreach (int index in charIndexes)
        {
            if (index != -1)
            {
                characterReady = true;
                break;
            }
        }
        foreach (int index in enemyIndexes)
        {
            if (index != -1)
            {
                enemyReady = true;
                break;
            }
        }
        if (!characterReady)
            alertText.text = "NO PLAYERS";
        else if (!enemyReady)
            alertText.text = "NO ENEMIES";
        else
        {
            // Construct set of new character and enemy objects
            List<CharacterStats> newCharacters = new List<CharacterStats>();
            List<Enemy> newEnemies = new List<Enemy>();

            // Create characters
            for (int i = 0; i < MAX_CHARACTERS; i++)
            {
                // If not empty
                if (charIndexes[i] != -1)
                {
                    // Create object, initialize character stats and add to list
                    Transform newCharacter = Instantiate(baseCharPrefab);
                    SaveData.GetCharacters()[charIndexes[i]].ConstructCharacter(newCharacter.GetComponent<CharacterStats>(), currItemsArray[i], true);
                    newCharacters.Add(newCharacter.GetComponent<CharacterStats>());
                }
            }

            UnitData[] currUnits = SaveData.GetFactions()[factionList.value].GetUnits();
            // Create enemies
            for (int i = 0; i < MAX_ENEMIES; i++)
            {
                // If not empty
                if (enemyIndexes[i] != -1)
                {
                    // Create object, initialize character stats and add to list
                    Transform newEnemy = Instantiate(baseEnemyPrefab);
                    currUnits[enemyIndexes[i]].ConstructUnit(newEnemy.GetComponent<Enemy>(), false);
                    newEnemies.Add(newEnemy.GetComponent<Enemy>());
                }
            }

            switch (tileTypeList.value)
            {
                case 0:
                    generator.GenerateExteriorLevel(newCharacters, newEnemies, GetExteriorTileset());
                    break;
                case 1:
                    generator.GenerateMapLevel(newCharacters, newEnemies, GetMapTileset());
                    break;
            }
            this.gameObject.SetActive(false);
        }
    }
    public ExteriorTilesetData GetExteriorTileset()
    {
        return SaveData.GetETilesets()[tilesetList.value];
    }
    public MapEditorData GetMapTileset()
    {
        return SaveData.GetMapTilesets()[tilesetList.value];
    }

    // Character and enemy list functions
    public void OpenCharacterList()
    {
        List<string> charNames = new List<string>();
        // Get list of all characters
        if (isSelectingCharacter)
        {
            selectCharText.text = "Select";
            for (int i = 0; i < MAX_CHARACTERS; i++)
            {
                charNames.Add(charIndexes[i] == -1 ? "Empty" : SaveData.GetCharacters()[charIndexes[i]].GetName());
            }

        }
        // Get list of selected characters
        else
        {
            selectCharText.text = "Cancel";
            currCharIndex = charList.value;
            charNames.Add("Select character");
            foreach (CharacterData character in SaveData.GetCharacters())
            {
                charNames.Add(character.GetName());
            }
            charNames.Add("Clear unit");
        }
        isSelectingCharacter = !isSelectingCharacter;
        charList.ClearOptions();
        charList.AddOptions(charNames);
        charList.value = 0;
    }
    // Dropdown change, save character to list
    public void SelectCharacter()
    {
        // Only save if selecting character
        if (isSelectingCharacter)
        {
            // Clear unit if last element is selected
            if (charList.value >= charList.options.Count - 1)
                charIndexes[currCharIndex] = -1;
            // Go back one because of prompt item
            else
                charIndexes[currCharIndex] = charList.value - 1;
            OpenCharacterList();
        }
    }
    // Dropdown change
    public void SelectFaction()
    {
        List<string> enemyNames = new List<string>();
        for (int i = 0; i < MAX_ENEMIES; i++)
        {
            enemyNames.Add("Empty");
            enemyIndexes[i] = -1;
        }
        enemyList.ClearOptions();
        enemyList.AddOptions(enemyNames);
        enemyList.value = 0;
        isSelectingEnemy = false;
        selectEnemyText.text = "Select";
    }
    public void OpenEnemyList()
    {
        UnitData[] currEnemies = SaveData.GetFactions()[factionList.value].GetUnits();

        List<string> enemyNames = new List<string>();
        // Get list of current enemies
        if (isSelectingEnemy)
        {
            selectEnemyText.text = "Select";
            for (int i = 0; i < MAX_ENEMIES; i++)
            {
                enemyNames.Add(enemyIndexes[i] == -1 ? "Empty" : currEnemies[enemyIndexes[i]].GetName());
            }
        }
        // Get list of selected enemies
        else
        {
            selectEnemyText.text = "Cancel";
            currEnemyIndex = enemyList.value;
            enemyNames.Add("Select enemy");
            foreach (UnitData enemy in currEnemies)
            {
                enemyNames.Add(enemy.GetName());
            }
            enemyNames.Add("Clear unit");
        }
        isSelectingEnemy = !isSelectingEnemy;
        enemyList.ClearOptions();
        enemyList.AddOptions(enemyNames);
        enemyList.value = 0;
    }
    // Dropdown change, save enemy to list
    public void SelectEnemy()
    {
        // Only save if selecting enemy
        if (isSelectingEnemy)
        {
            // Clear unit if last element is selected
            if (enemyList.value >= enemyList.options.Count - 1)
                enemyIndexes[currEnemyIndex] = -1;
            // Go back one because of prompt item
            else
                enemyIndexes[currEnemyIndex] = enemyList.value - 1;
            OpenEnemyList();
        }
    }

    // Equip form methds
    public void OpenEquipForm()
    {
        equipForm.gameObject.SetActive(true);
        equipForm.InitDisplay(charList.value);
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

    // Tileset methods
    // Dropdown change
    public void SetTileType()
    {
        switch (tileTypeList.value)
        {
            case 0:
                GetExteriorTilesets();
                break;
            case 1:
                GetMapTilesets();
                break;
        }
    }

    public void GetExteriorTilesets()
    {
        List<ExteriorTilesetData> tilesets = SaveData.GetETilesets();
        List<string> tileNames = new List<string>();
        foreach (ExteriorTilesetData tileset in tilesets)
        {
            tileNames.Add(tileset.GetName());
        }
        tilesetList.ClearOptions();
        tilesetList.AddOptions(tileNames);
    }
    public void GetMapTilesets()
    {
        List<MapEditorData> tilesets = SaveData.GetMapTilesets();
        List<string> tileNames = new List<string>();
        foreach (MapEditorData tileset in tilesets)
        {
            tileNames.Add(tileset.GetName());
        }
        tilesetList.ClearOptions();
        tilesetList.AddOptions(tileNames);
    }
}
