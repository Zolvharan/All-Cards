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
        tilesInRange = currTile.FindTilesInRange(currTile, currStats["moveSpeed"]);
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
