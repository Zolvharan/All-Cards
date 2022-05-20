using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterStats
{
    Sprite banner;
    int enemyLevel;
    EnemyControl enemyControl;

    HashSet<Tile> tilesInRange;
    CharacterStats closestPlayer;
    Tile closestTile;
    int currDistance;
    int tileDistance;

    // Initializes variables
    public void ConstructUnit(string newName, Ability[] newAbilities, Sprite newPortrait, Sprite newBattleSprite, bool isFlying, Dictionary<String, int> newBaseStats, Sprite newBanner, int newEnemyLevel, bool isPlayer)
    {
        // Enemy has no items
        ConstructCharacter(newName, newAbilities, newPortrait, newBattleSprite, isFlying, newBaseStats, null, isPlayer);

        banner = newBanner;
        enemyLevel = newEnemyLevel;
    }

    public void SetController(EnemyControl newControl)
    {
        enemyControl = newControl;
    }
    public Sprite GetBanner()
    {
        return banner;
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
