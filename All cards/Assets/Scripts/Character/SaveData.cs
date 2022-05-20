using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Manages character, ability, item, faction, and enemy unit data
public static class SaveData
{
    static List<CharacterData> savedCharacters;
    static List<AbilityData> savedAbilities;
    static List<ItemData> savedItems;

    static List<FactionData> savedFactions;
    static List<UnitData> savedUnits;

    // Loads data into savedCharacters, savedAbilities, and savedItems
    static public void LoadData()
    {
        savedCharacters = new List<CharacterData>();
        savedAbilities = new List<AbilityData>();
        savedItems = new List<ItemData>();
        savedFactions = new List<FactionData>();
        savedUnits = new List<UnitData>();
        string inString = "";

        // Load characters
        foreach (string inLine in File.ReadAllLines(".\\SavedData\\CharacterData.json"))
        {
            inString += inLine;
        }
        foreach (string objString in inString.Split('|'))
        {
            if (objString != "")
                savedCharacters.Add(JsonUtility.FromJson<CharacterData>(objString));
        }

        inString = "";
        // Load abilities
        foreach (string inLine in File.ReadAllLines(".\\SavedData\\AbilityData.json"))
        {
            inString += inLine;
        }
        foreach (string objString in inString.Split('|'))
        {
            if (objString != "")
                savedAbilities.Add(JsonUtility.FromJson<AbilityData>(objString));
        }

        inString = "";
        // Load items
        foreach (string inLine in File.ReadAllLines(".\\SavedData\\ItemData.json"))
        {
            inString += inLine;
        }
        foreach (string objString in inString.Split('|'))
        {
            if (objString != "")
                savedItems.Add(JsonUtility.FromJson<ItemData>(objString));
        }

        inString = "";
        // Load Factions
        foreach (string inLine in File.ReadAllLines(".\\SavedData\\FactionData.json"))
        {
            inString += inLine;
        }
        foreach (string objString in inString.Split('|'))
        {
            if (objString != "")
                savedFactions.Add(JsonUtility.FromJson<FactionData>(objString));
        }

        inString = "";
        // Load Units
        foreach (string inLine in File.ReadAllLines(".\\SavedData\\UnitData.json"))
        {
            inString += inLine;
        }
        foreach (string objString in inString.Split('|'))
        {
            if (objString != "")
                savedUnits.Add(JsonUtility.FromJson<UnitData>(objString));
        }
    }

    // Character data
    static public void SaveCharacter(CharacterData newCharacter, int overwriteIndex = -1)
    {
        // -1 index means new save
        if (overwriteIndex != -1)
        {
            savedCharacters.Insert(overwriteIndex, newCharacter);
            savedCharacters.RemoveAt(overwriteIndex + 1);
        }
        else
            savedCharacters.Add(newCharacter);

        WriteCharacters();
    }
    static public void DeleteCharacter(int saveIndex)
    {
        savedCharacters.RemoveAt(saveIndex);
        WriteCharacters();
    }
    static public List<CharacterData> GetCharacters()
    {
        return savedCharacters;
    }
    static void WriteCharacters()
    {
        // Store the items as json strings, and separate with |
        // The items are taken out split by |
        string outString = "";
        for (int i = 0; i < savedCharacters.Count; i++)
        {
            outString += JsonUtility.ToJson(savedCharacters[i]);
            if (i != savedCharacters.Count - 1)
                outString += '|';
        }
        File.WriteAllText(".\\SavedData\\CharacterData.json", outString);
    }

    // Ability data
    static public void SaveAbility(AbilityData newAbility, int overwriteIndex = -1)
    {
        // -1 index means new save
        if (overwriteIndex != -1)
        {
            savedAbilities.Insert(overwriteIndex, newAbility);
            savedAbilities.RemoveAt(overwriteIndex + 1);
        }
        else
            savedAbilities.Add(newAbility);

        WriteAbilities();
    }
    static public void DeleteAbility(int saveIndex)
    {
        savedAbilities.RemoveAt(saveIndex);
        WriteAbilities();
    }
    static public List<AbilityData> GetAbilities()
    {
        return savedAbilities;
    }
    static void WriteAbilities()
    {
        // Store the items as json strings, and separate with |
        // The items are taken out split by |
        string outString = "";
        for (int i = 0; i < savedAbilities.Count; i++)
        {
            outString += JsonUtility.ToJson(savedAbilities[i]);
            if (i != savedAbilities.Count - 1)
                outString += '|';
        }
        File.WriteAllText(".\\SavedData\\AbilityData.json", outString);
    }

    // Item data
    static public void SaveItem(ItemData newItem, int overwriteIndex = -1)
    {
        // -1 index means new save
        if (overwriteIndex != -1)
        {
            savedItems.Insert(overwriteIndex, newItem);
            savedItems.RemoveAt(overwriteIndex + 1);
        }
        else
            savedItems.Add(newItem);

        WriteItems();
    }
    static public void DeleteItem(int saveIndex)
    {
        savedItems.RemoveAt(saveIndex);
        WriteItems();
    }
    static public List<ItemData> GetItems()
    {
        return savedItems;
    }
    static void WriteItems()
    {
        // Store the items as json strings, and separate with |
        // The items are taken out split by |
        string outString = "";
        for (int i = 0; i < savedItems.Count; i++)
        {
            outString += JsonUtility.ToJson(savedItems[i]);
            if (i != savedItems.Count - 1)
                outString += '|';
        }
        File.WriteAllText(".\\SavedData\\ItemData.json", outString);
    }


    // Faction data
    static public void SaveFaction(FactionData newFaction, int overwriteIndex = -1)
    {
        // Change all unit banners
        foreach (UnitData unit in newFaction.GetUnits())
        {
            unit.SetBanner(newFaction.GetBanner());
        }

        // -1 index means new save
        if (overwriteIndex != -1)
        {
            savedFactions.Insert(overwriteIndex, newFaction);
            savedFactions.RemoveAt(overwriteIndex + 1);
        }
        else
            savedFactions.Add(newFaction);

        WriteFactions();
    }
    static public void DeleteFaction(int saveIndex)
    {
        savedFactions.RemoveAt(saveIndex);
        WriteFactions();
    }
    static public List<FactionData> GetFactions()
    {
        return savedFactions;
    }
    static void WriteFactions()
    {
        // Store the items as json strings, and separate with |
        // The items are taken out split by |
        string outString = "";
        for (int i = 0; i < savedFactions.Count; i++)
        {
            outString += JsonUtility.ToJson(savedFactions[i]);
            if (i != savedFactions.Count - 1)
                outString += '|';
        }
        File.WriteAllText(".\\SavedData\\FactionData.json", outString);
    }

    // Unit data
    static public void SaveUnit(UnitData newUnit, int overwriteIndex = -1)
    {
        // -1 index means new save
        if (overwriteIndex != -1)
        {
            savedUnits.Insert(overwriteIndex, newUnit);
            savedUnits.RemoveAt(overwriteIndex + 1);
        }
        else
            savedUnits.Add(newUnit);

        WriteUnits();
    }
    static public void DeleteUnit(int saveIndex)
    {
        savedUnits.RemoveAt(saveIndex);
        WriteUnits();
    }
    static public List<UnitData> GetUnits()
    {
        return savedUnits;
    }
    static void WriteUnits()
    {
        // Store the items as json strings, and separate with |
        // The items are taken out split by |
        string outString = "";
        for (int i = 0; i < savedUnits.Count; i++)
        {
            outString += JsonUtility.ToJson(savedUnits[i]);
            if (i != savedUnits.Count - 1)
                outString += '|';
        }
        File.WriteAllText(".\\SavedData\\UnitData.json", outString);
    }
}
