using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorData
{
    public string tilesetName;
    public int[] tileTypeIndexes;
    public TileData[] tileTypes;
    public int length;
    public int height;

    public MapEditorData(string newName, int[][] newTypeIndexes, TileData[] newTypes, int newLength, int newHeight)
    {
        tilesetName = newName;
        tileTypeIndexes = new int[newTypeIndexes.Length * newTypeIndexes[0].Length];
        for (int i = 0; i < newTypeIndexes.Length; i++)
        {
            newTypeIndexes[i].CopyTo(tileTypeIndexes, i * newTypeIndexes[0].Length);
        }
        tileTypes = new TileData[newTypes.Length];
        newTypes.CopyTo(tileTypes, 0);
        length = newLength;
        height = newHeight;
    }

    public string GetName()
    {
        return tilesetName;
    }

    public int[] GetTypeIndexes()
    {
        return tileTypeIndexes;
    }
    public TileData[] GetTileTypes()
    {
        return tileTypes;
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
