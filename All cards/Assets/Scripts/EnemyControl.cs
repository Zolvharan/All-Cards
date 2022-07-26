using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public List<Enemy> enemies;
    public PlayerControl player;
    public BUIManager UI;
    public ActionDisplay actionDisplay;

    public void SetEnemies(List<Enemy> newEnemies)
    {
        enemies = newEnemies;
        foreach (Enemy enemy in enemies)
        {
            enemy.SetController(this);
        }
    }

    // Detemines win condition
    public void ScanEnemies()
    {
        bool allDead = true;
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.dead)
                allDead = false;
        }
        if (allDead && !player.playerLost)
            UI.Win();
    }

    public void StartTurn()
    {
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.dead)
                enemy.Move();
        }
        player.StartTurn();
    }

    public ActionDisplay GetActionDisplay()
    {
        return actionDisplay;
    }
}
