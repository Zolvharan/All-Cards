using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LandformData
{
    public string landformName;
    public int minX;
    public int minY;
    public int maxX;
    public int maxY;
    public float snakiness;
    public TileData tile;
    public SubLandformData subLandform;

    // Tileset data
    public int varPosX;
    public int varPosY;
    public int minCount;
    public int maxCount;

    public LandformData(string newName, int newMinX, int newMinY, int newMaxX, int newMaxY, float newSnakiness, TileData newTile, SubLandformData newSubLandform)
    {
        landformName = newName;
        minX = newMinX;
        minY = newMinY;
        maxX = newMaxX;
        maxY = newMaxY;
        snakiness = newSnakiness;
        tile = newTile;
        subLandform = newSubLandform;
    }

    public string GetName()
    {
        return landformName;
    }

    public int GetMinX()
    {
        return minX;
    }
    public int GetMinY()
    {
        return minY;
    }
    public int GetMaxX()
    {
        return maxX;
    }
    public int GetMaxY()
    {
        return maxY;
    }
    public float GetSnakiness()
    {
        return snakiness;
    }
    public TileData GetTile()
    {
        return tile;
    }
    public SubLandformData GetSubLandform()
    {
        return subLandform;
    }

    // Set whole set of data
    public void SetTilesetData(int newVarPosX, int newVarPosY, int newMinCount, int newMaxCount)
    {
        varPosX = newVarPosX;
        varPosY = newVarPosY;
        minCount = newMinCount;
        maxCount = newMaxCount;
    }
    // Set individual data
    public void SetVarPosX(int newVarPosX)
    {
        varPosX = newVarPosX;
    }
    public void SetVarPosY(int newVarPosY)
    {
        varPosY = newVarPosY;
    }
    public void SetMinCount(int newMinCount)
    {
        minCount = newMinCount;
    }
    public void SetMaxCount(int newMaxCount)
    {
        maxCount = newMaxCount;
    }
    public int GetVarPosX()
    {
        return varPosX;
    }
    public int GetVarPosY()
    {
        return varPosY;
    }
    public int GetMinCount()
    {
        return minCount;
    }
    public int GetMaxCount()
    {
        return maxCount;
    }
}
