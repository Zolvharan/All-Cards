using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Manages character, ability, item, faction, enemy unit, and tileset data
public static class SaveData
{
    static List<CharacterData> savedCharacters;
    static List<AbilityData> savedAbilities;
    static List<ItemData> savedItems;
    const string characterPath = ".\\SavedData\\CharacterData.json";
    const string abilityPath = ".\\SavedData\\AbilityData.json";
    const string itemPath = ".\\SavedData\\ItemData.json";

    static List<FactionData> savedFactions;
    static List<UnitData> savedUnits;
    const string factionPath = ".\\SavedData\\FactionData.json";
    const string unitPath = ".\\SavedData\\UnitData.json";

    static List<ExteriorTilesetData> savedETilesets;
    static List<LandformData> savedLandforms;
    static List<TileData> savedTiles;
    const string ETilesetPath = ".\\SavedData\\ETilesetsData.json";
    const string landformPath = ".\\SavedData\\LandformData.json";
    const string tilePath = ".\\SavedData\\TileData.json";

    static List<MapEditorData> savedMapTilesets;
    const string mapTilesetPath = ".\\SavedData\\MapTilesetsData.json";

    // Loads data into savedCharacters, savedAbilities, savedItems, savedFactions, and savedUnits
    static public void LoadData()
    {
        savedCharacters = new List<CharacterData>();
        savedAbilities = new List<AbilityData>();
        savedItems = new List<ItemData>();
        savedFactions = new List<FactionData>();
        savedUnits = new List<UnitData>();
        savedETilesets = new List<ExteriorTilesetData>();
        savedLandforms = new List<LandformData>();
        savedTiles = new List<TileData>();
        savedMapTilesets = new List<MapEditorData>();

        GetData(savedCharacters, characterPath);
        GetData(savedAbilities, abilityPath);
        GetData(savedItems, itemPath);
        GetData(savedFactions, factionPath);
        GetData(savedUnits, unitPath);
        GetData(savedETilesets, ETilesetPath);
        GetData(savedLandforms, landformPath);
        GetData(savedTiles, tilePath);
        GetData(savedMapTilesets, mapTilesetPath);
    }
    static void GetData<T>(List<T> dataList, string filePath)
    {
        string inString = "";
        foreach (string inLine in File.ReadAllLines(filePath))
        {
            inString += inLine;
        }
        foreach (string objString in inString.Split('|'))
        {
            if (objString != "")
                dataList.Add(JsonUtility.FromJson<T>(objString));
        }
    }

    static void WriteData<T>(List<T> dataList, string filePath)
    {
        // Store the items as json strings, and separate with |
        // The items are taken out split by |
        string outString = "";
        for (int i = 0; i < dataList.Count; i++)
        {
            outString += JsonUtility.ToJson(dataList[i]);
            if (i != dataList.Count - 1)
                outString += '|';
        }
        File.WriteAllText(filePath, outString);
    }

    static void SaveDataItem<T>(List<T> dataList, T dataItem, string filePath, int overwriteIndex)
    {
        // -1 index means new save
        if (overwriteIndex != -1)
        {
            dataList.Insert(overwriteIndex, dataItem);
            dataList.RemoveAt(overwriteIndex + 1);
        }
        else
            dataList.Add(dataItem);

        WriteData(dataList, filePath);
    }
    // Character data
    static public void SaveCharacter(CharacterData newCharacter, int overwriteIndex = -1)
    {
        SaveDataItem(savedCharacters, newCharacter, characterPath, overwriteIndex);
    }
    static public void DeleteCharacter(int saveIndex)
    {
        savedCharacters.RemoveAt(saveIndex);
        WriteData(savedCharacters, characterPath);
    }
    static public List<CharacterData> GetCharacters()
    {
        return savedCharacters;
    }

    // Ability data
    static public void SaveAbility(AbilityData newAbility, int overwriteIndex = -1)
    {
        SaveDataItem(savedAbilities, newAbility, abilityPath, overwriteIndex);
    }
    static public void DeleteAbility(int saveIndex)
    {
        savedAbilities.RemoveAt(saveIndex);
        WriteData(savedAbilities, abilityPath);
    }
    static public List<AbilityData> GetAbilities()
    {
        return savedAbilities;
    }

    // Item data
    static public void SaveItem(ItemData newItem, int overwriteIndex = -1)
    {
        SaveDataItem(savedItems, newItem, itemPath, overwriteIndex);
    }
    static public void DeleteItem(int saveIndex)
    {
        savedItems.RemoveAt(saveIndex);
        WriteData(savedItems, itemPath);
    }
    static public List<ItemData> GetItems()
    {
        return savedItems;
    }

    // Faction data
    static public void SaveFaction(FactionData newFaction, int overwriteIndex = -1)
    {
        SaveDataItem(savedFactions, newFaction, factionPath, overwriteIndex);
    }
    static public void DeleteFaction(int saveIndex)
    {
        savedFactions.RemoveAt(saveIndex);
        WriteData(savedFactions, factionPath);
    }
    static public List<FactionData> GetFactions()
    {
        return savedFactions;
    }

    // Unit data
    static public void SaveUnit(UnitData newUnit, int overwriteIndex = -1)
    {
        SaveDataItem(savedUnits, newUnit, unitPath, overwriteIndex);
    }
    static public void DeleteUnit(int saveIndex)
    {
        savedUnits.RemoveAt(saveIndex);
        WriteData(savedUnits, unitPath);
    }
    static public List<UnitData> GetUnits()
    {
        return savedUnits;
    }

    // Exterior Tileset data
    static public void SaveETileset(ExteriorTilesetData newETileset, int overwriteIndex = -1)
    {
        SaveDataItem(savedETilesets, newETileset, ETilesetPath, overwriteIndex);
    }
    static public void DeleteETileset(int saveIndex)
    {
        savedETilesets.RemoveAt(saveIndex);
        WriteData(savedETilesets, ETilesetPath);
    }
    static public List<ExteriorTilesetData> GetETilesets()
    {
        return savedETilesets;
    }

    // Landform data
    static public void SaveLandform(LandformData newLandform, int overwriteIndex = -1)
    {
        SaveDataItem(savedLandforms, newLandform, landformPath, overwriteIndex);
    }
    static public void DeleteLandform(int saveIndex)
    {
        savedLandforms.RemoveAt(saveIndex);
        WriteData(savedLandforms, landformPath);
    }
    static public List<LandformData> GetLandforms()
    {
        return savedLandforms;
    }

    // Tile data
    static public void SaveTile(TileData newTile, int overwriteIndex = -1)
    {
        SaveDataItem(savedTiles, newTile, tilePath, overwriteIndex);
    }
    static public void DeleteTile(int saveIndex)
    {
        savedTiles.RemoveAt(saveIndex);
        WriteData(savedTiles, tilePath);
    }
    static public List<TileData> GetTiles()
    {
        return savedTiles;
    }

    // Map Editor data
    static public void SaveMapTileset(MapEditorData newMapTileset, int overwriteIndex = -1)
    {
        SaveDataItem(savedMapTilesets, newMapTileset, mapTilesetPath, overwriteIndex);
    }
    static public void DeleteMapTileset(int saveIndex)
    {
        savedMapTilesets.RemoveAt(saveIndex);
        WriteData(savedMapTilesets, mapTilesetPath);
    }
    static public List<MapEditorData> GetMapTilesets()
    {
        return savedMapTilesets;
    }
}
