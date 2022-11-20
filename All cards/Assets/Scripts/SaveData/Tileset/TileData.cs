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

    public int[] rawEffects;
    public int[] percentageEffects;
    public AbilityData tileAbility;
    public bool hasAbility;

    public TileData(string newName, float newMoveWeight, bool isImpassable, byte[] newImageData, int[] newRawEffects, int[] newPercentageEffects, AbilityData newAbility)
    {
        tileName = newName;
        moveWeight = newMoveWeight;
        impassable = isImpassable;
        imageData = new byte[newImageData.Length];
        newImageData.CopyTo(imageData, 0);

        rawEffects = new int[10];
        newRawEffects.CopyTo(rawEffects, 0);
        percentageEffects = new int[8];
        newPercentageEffects.CopyTo(percentageEffects, 0);
        tileAbility = newAbility;
        if (tileAbility == null)
            hasAbility = false;
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

    public int[] GetRawEffects()
    {
        return rawEffects;
    }
    public int[] GetPercentageEffects()
    {
        return percentageEffects;
    }
    public AbilityData GetAbility()
    {
        return tileAbility;
    }
    public bool HasAbility()
    {
        return hasAbility;
    }
}
