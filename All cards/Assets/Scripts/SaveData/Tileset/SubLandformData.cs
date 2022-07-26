using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubLandformData
{
    public string landformName;
    public int minX;
    public int minY;
    public int maxX;
    public int maxY;
    public float snakiness;
    public TileData tile;

    // Sub landform data
    public int varX;
    public int varY;
    public int minCount;
    public int maxCount;

    public SubLandformData(string newName, int newMinX, int newMinY, int newMaxX, int newMaxY, float newSnakiness, TileData newTile,
        int newVarX = 0, int newVarY = 0, int newMinCount = 0, int newMaxCount = 0)
    {
        landformName = newName;
        minX = newMinX;
        minY = newMinY;
        maxX = newMaxX;
        maxY = newMaxY;
        snakiness = newSnakiness;
        tile = newTile;

        varX = newVarX;
        varY = newVarY;
        minCount = newMinCount;
        maxCount = newMaxCount;
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

    // Sub landform methods
    public static SubLandformData ConstructDefault()
    {
        return new SubLandformData("", 0, 0, 0, 0, 0, TileFormManager.GetDefaultTile(), 0, 0, 0, 0);
    }

    public int GetVarX()
    {
        return varX;
    }
    public int GetVarY()
    {
        return varY;
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
