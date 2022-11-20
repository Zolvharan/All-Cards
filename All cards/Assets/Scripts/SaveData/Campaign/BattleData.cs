using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleData
{
    public string battleName;
    public TilesetData tileset;
    public Enemy[] enemies;
    public int[] playerLocationsX;
    public int[] playerLocationsY;
    public int[] enemyLocationsX;
    public int[] enemyLocationsY;
    public ObjectiveData[] objectives;
    // Cutscenes?

    public BattleData(string newName, TilesetData newTileset, Enemy[] newEnemies, int[] newPlayerLocationsX, int[] newPlayerLocationsY, int[] newEnemyLocationsX, int[] newEnemyLocationsY,
        ObjectiveData[] newObjectives)
    {
        battleName = newName;
        tileset = newTileset;
        enemies = new Enemy[newEnemies.Length];
        newEnemies.CopyTo(enemies, 0);
        playerLocationsX = new int[newPlayerLocationsX.Length];
        playerLocationsY = new int[newPlayerLocationsY.Length];
        newEnemyLocationsX.CopyTo(enemyLocationsX, 0);
        newEnemyLocationsY.CopyTo(enemyLocationsY, 0);
        enemyLocationsX = new int[newEnemyLocationsX.Length];
        enemyLocationsY = new int[newEnemyLocationsY.Length];
        newEnemyLocationsX.CopyTo(enemyLocationsX, 0);
        newEnemyLocationsY.CopyTo(enemyLocationsY, 0);
        objectives = new ObjectiveData[newObjectives.Length];
        newObjectives.CopyTo(objectives, 0);
    }

    public string GetName()
    {
        return battleName;
    }
}
