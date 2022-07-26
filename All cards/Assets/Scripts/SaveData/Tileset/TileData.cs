using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public string tileName;
    public float moveWeight;
    public bool impassable;
    public byte[] imageData;

    public TileData(string newName, float newMoveWeight, bool isImpassable, byte[] newImageData)
    {
        tileName = newName;
        moveWeight = newMoveWeight;
        impassable = isImpassable;
        imageData = new byte[newImageData.Length];
        newImageData.CopyTo(imageData, 0);
    }

    public string GetName()
    {
        return tileName;
    }
    public float GetMoveWeight()
    {
        return moveWeight;
    }
    public bool GetImpassable()
    {
        return impassable;
    }
    public byte[] GetImage()
    {
        return imageData;
    }
}
