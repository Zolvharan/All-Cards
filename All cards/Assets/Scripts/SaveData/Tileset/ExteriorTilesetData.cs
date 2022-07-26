using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExteriorTilesetData
{
    public string tilesetName;
    public LandformData[] landforms;
    public float[] tileWeights;
    public TileData[] defaultTiles;
    public int length;
    public int height;

    public ExteriorTilesetData(string newName, LandformData[] newLandforms, float[] newTileWeights, TileData[] newTiles, int newLength, int newHeight)
    {
        tilesetName = newName;
        landforms = new LandformData[newLandforms.Length];
        newLandforms.CopyTo(landforms, 0);
        tileWeights = new float[newTileWeights.Length];
        newTileWeights.CopyTo(tileWeights, 0);
        defaultTiles = new TileData[newTiles.Length];
        newTiles.CopyTo(defaultTiles, 0);
        length = newLength;
        height = newHeight;
    }

    public string GetName()
    {
        return tilesetName;
    }

    public LandformData[] GetLandforms()
    {
        return landforms;
    }
    public float[] GetTileWeights()
    {
        return tileWeights;
    }
    public TileData[] GetTiles()
    {
        return defaultTiles;
    }
    public int GetLength()
    {
        return length;
    }
    public int GetHeight()
    {
        return height;
    }
}
