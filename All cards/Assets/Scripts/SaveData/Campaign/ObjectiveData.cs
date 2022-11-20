using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveData
{
    public enum ObjectiveType { Annihilation, UnitDefeat, TurnsElapsed, TileUsed, AffectedUnit, Confrontation };

    public string objectiveName;
    public int type;

    public bool isVictory;
    public bool isDefeat;
    public BattleData[] battleUnlocks;
    public CharacterData[] characterUnlocks;
    public EventData playedEvent;

    public string subjectName;
    public int objectiveCounter;

    // Annihilation constructor
    public ObjectiveData(int newType)
    {
        type = newType;
        isVictory = false;
        isDefeat = false;
    }
    // UnitDefeat constructor
    public ObjectiveData(int newType, string newUnitName)
    {
        subjectName = newUnitName;

        type = newType;
        isVictory = false;
        isDefeat = false;
    }
    // TurnsElapsed constructor
    public ObjectiveData(int newType, int newTurnsToElapse)
    {
        objectiveCounter = newTurnsToElapse;

        type = newType;
        isVictory = false;
        isDefeat = false;
    }
    // TileUsed constructor
    public ObjectiveData(int newType, string newTileName, int newTilesNeeded)
    {
        subjectName = newTileName;
        objectiveCounter = newTilesNeeded;

        type = newType;
        isVictory = false;
        isDefeat = false;
    }

    public void AddVictory()
    {
        isVictory = true;
        isDefeat = false;
    }
    public void AddDefeat()
    {
        isDefeat = true;
        isVictory = false;
    }
    // Battles
    public void AddReward(BattleData[] newBattles)
    {
        battleUnlocks = new BattleData[newBattles.Length];
        newBattles.CopyTo(battleUnlocks, 0);
    }
    // Characters
    public void AddReward(CharacterData[] newCharacters)
    {
        characterUnlocks = new CharacterData[newCharacters.Length];
        newCharacters.CopyTo(characterUnlocks, 0);
    }
    // Events
    public void AddReward(EventData newEvent)
    {
        playedEvent = newEvent;
    }
}
