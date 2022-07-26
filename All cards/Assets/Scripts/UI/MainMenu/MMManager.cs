using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generally manages the main menu
public class MMManager : MonoBehaviour
{
    public GameObject BattleUI;

    public GameObject frontPage;
    public BattleSetupUI customBattle;
    public CCManager characters;
    public EnemyGenManager enemies;
    public GameObject tilesets;

    public void OpenFrontPage()
    {
        frontPage.SetActive(true);
        customBattle.gameObject.SetActive(false);
        characters.gameObject.SetActive(false);
        enemies.gameObject.SetActive(false);
        tilesets.SetActive(false);
    }

    public void OpenCustomBattle()
    {
        customBattle.gameObject.SetActive(true);
        customBattle.InitDisplay();
        frontPage.SetActive(false);
    }
    // Resets menu, opens battleUI and hides
    public void StartCombat()
    {
        customBattle.gameObject.SetActive(false);
        frontPage.SetActive(false);
        BattleUI.SetActive(true);
    }

    public void OpenCharacters()
    {
        characters.gameObject.SetActive(true);
        characters.InitDisplay();
        frontPage.SetActive(false);
    }
    public void OpenEnemies()
    {
        enemies.gameObject.SetActive(true);
        enemies.InitDisplay();
        frontPage.SetActive(false);
    }
    public void OpenTilesets()
    {
        tilesets.gameObject.SetActive(true);
        frontPage.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}