using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public GameObject mainCamera;
    public List<Enemy> enemies;
    public PlayerControl player;
    public BUIManager UI;
    public ActionDisplay actionDisplay;
    public GameObject enemyTurnDisplay;

    Enemy currEnemy;

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
        StartCoroutine(IterateEnemies());
    }
    IEnumerator IterateEnemies()
    {
        player.enabled = false;
        enemyTurnDisplay.SetActive(true);
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.dead)
            {
                currEnemy = enemy;
                yield return enemy.ExecuteTurn();
            }
        }
        player.enabled = true;
        enemyTurnDisplay.SetActive(false);
        player.StartTurn();
    }

    public ActionDisplay GetActionDisplay()
    {
        return actionDisplay;
    }
    public void SetCamera()
    {
        mainCamera.transform.position = new Vector3(currEnemy.transform.position.x, currEnemy.transform.position.y, mainCamera.transform.position.z);
    }
}
