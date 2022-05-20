using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FactionData
{
    public string factionName;
    public byte[] banner;
    public UnitData[] units;

    public FactionData(string newName, byte[] newBanner, UnitData[] newUnits)
    {
        factionName = newName;
        banner = new byte[newBanner.Length];
        newBanner.CopyTo(banner, 0);
        units = new UnitData[newUnits.Length];
        newUnits.CopyTo(units, 0);
    }

    public string GetName()
    {
        return factionName;
    }
    public byte[] GetBanner()
    {
        return banner;
    }
    public UnitData[] GetUnits()
    {
        return units;
    }
}