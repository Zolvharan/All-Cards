using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterStats
{
    EnemyControl enemyControl;
    HashSet<Tile> tilesInRange;
    CharacterStats closestPlayer;
    Tile closestTile;
    int currDistance;
    int tileDistance;

    public void SetController(EnemyControl newControl)
    {
        // TODO: TEMP CODE
        baseStats = new Dictionary<string, int>();
        baseStats["health"] = 20;
        baseStats["energy"] = 1;
        baseStats["moveSpeed"] = 4;
        baseStats["attackRange"] = 1;
        baseStats["strength"] = 5;
        baseStats["precision"] = 0;
        baseStats["defense"] = 0;
        baseStats["dexterity"] = 0;
        baseStats["resistance"] = 0;
        baseStats["energyRegen"] = 0;
        currStats = new Dictionary<string, int>(baseStats);
        statEffectsPotencies = new Dictionary<string, int>();
        statEffectsPotencies["health"] = 0;
        statEffectsPotencies["energy"] = 0;
        statEffectsPotencies["moveSpeed"] = 0;
        statEffectsPotencies["attackRange"] = 0;
        statEffectsPotencies["strength"] = 0;
        statEffectsPotencies["precision"] = 0;
        statEffectsPotencies["defense"] = 0;
        statEffectsPotencies["dexterity"] = 0;
        statEffectsPotencies["resistance"] = 0;
        statEffectsPotencies["energyRegen"] = 0;
        statEffectsDurations = new Dictionary<string, int>(statEffectsPotencies);
        flying = false;

        enemyControl = newControl;
    }
    override protected void Die()
    {
        dead = true;
        currTile.ClearUnit(false);
        enemyControl.ScanEnemies();
        this.gameObject.SetActive(false);
    }

    // Rudimentary AI behavior
    public void Move()
    {
        NewTurn();
        if (dead)
            return;
        tilesInRange = currTile.GetTiles(currTile, currStats["moveSpeed"], false);
        closestPlayer = null;
        foreach (CharacterStats unit in currTile.map.characters)
        {
            if (!unit.dead)
            {
                tileDistance = Math.Abs(unit.currTile.xPos - currTile.xPos) + Math.Abs(unit.currTile.yPos - currTile.yPos);
                if (closestPlayer == null || tileDistance < currDistance)
                {
                    closestPlayer = unit;
                    currDistance = tileDistance;
                }
            }
        }
        if (closestPlayer == null)
            return;
        currDistance = Math.Abs(closestPlayer.currTile.xPos - currTile.xPos) + Math.Abs(closestPlayer.currTile.yPos - currTile.yPos);
        foreach (Tile tile in tilesInRange)
        {
            tileDistance = Math.Abs(tile.xPos - closestPlayer.currTile.xPos) + Math.Abs(tile.yPos - closestPlayer.currTile.yPos);
            if (tileDistance < currDistance && !tile.occupied)
            {
                closestTile = tile;
                currDistance = tileDistance;
            }
        }
        if (currDistance <= currStats["attackRange"])
            closestPlayer.TakeDamage(currStats["strength"]);
        currTile.ClearUnit(false);
        closestTile.PlaceUnit(this);
    }
}
