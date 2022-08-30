using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BUIManager : MonoBehaviour
{
    public MainMenuManager mainMenuManager;
    public LevelGenerator level;

    public GameObject menu;
    public GameObject menuButton;
    public GameObject characterDisplay;
    public GameObject health;
    public GameObject energy;
    public GameObject overcharge;
    public SpriteRenderer charImage;
    public Button[] actionButtons;
    public Button[] confirmButtons;

    public GameObject enemyDisplay;
    public SpriteRenderer bannerImage;
    public SpriteRenderer enemyImage;
    public GameObject enemyHealth;
    public GameObject enemyEnergy;

    CharacterStats currCharacter;
    public GameObject statusDisplay;
    public SpriteRenderer statusCharImage;
    public GameObject statusHealth;
    public GameObject statusEnergy;
    public GameObject statusOvercharge;
    public Text[] statusNums;
    public Text[] statusTurnNums;

    public ForecastManager forecastDisplay;

    public GameObject win;
    public GameObject loss;

    public bool locked = false;

    // Update is called once per frame
    void Update()
    {
        // Close status whenever select button is pressed
        if (Input.GetKeyDown(Inputs.select) && statusDisplay.activeSelf)
        {
            CloseStatus();
        }
    }

    public void ReturnToMenu()
    {
        menu.SetActive(false);
        level.DestroyLevel();
        mainMenuManager.OpenFrontPage();
        this.gameObject.SetActive(false);
    }

    // Called by Tile.SelectTile
    public void DisplayCharacter(CharacterStats character)
    {
        if (menu.activeSelf)
            menu.SetActive(false);

        charImage.sprite = character.portrait;
        if (character.attacked == true)
        {
            foreach(Button button in actionButtons)
                button.interactable = false;
        }
        else
        {
            foreach(Button button in actionButtons)
                button.interactable = true;
        }

        SetStatBars(character, health, energy, overcharge);
        currCharacter = character;
        characterDisplay.SetActive(true);
    }

    // Ability/Item
    public void DisplayForecast(CharacterStats currUnit, Ability currAbility, CharacterStats castingCharacter)
    {
        forecastDisplay.DisplayForecast(currUnit, currAbility, castingCharacter);
    }
    // Attack
    public void DisplayForecast(CharacterStats currUnit, int strength, int precision)
    {
        forecastDisplay.DisplayForecast(currUnit, strength, precision);
    }
    public void HideForecast()
    {
        forecastDisplay.gameObject.SetActive(false);
    }

    // Sets given stat bars
    public void SetStatBars(CharacterStats currUnit, GameObject currHealthBar, GameObject currEnergyBar, GameObject currOverchargeBar, int[] offsets = null)
    {
        float charHealth = currUnit.GetHealth();
        float maxHealth = currUnit.GetMaxHealth();
        float charEnergy = currUnit.GetEnergy();
        float maxEnergy = currUnit.GetMaxEnergy();

        // When forecasting, get offsets for bars
        if (offsets != null)
        {
            charHealth += offsets[0];
            charEnergy += offsets[1];
        }

        // Keep bar from going negative
        if (charHealth / maxHealth < 0)
            currHealthBar.transform.localScale =  new Vector3(0, currHealthBar.transform.localScale.y, currHealthBar.transform.localScale.x);
        else
            currHealthBar.transform.localScale = new Vector3((charHealth / maxHealth <= 1 ? charHealth / maxHealth : 1), currHealthBar.transform.localScale.y, currHealthBar.transform.localScale.x);
        currOverchargeBar.transform.localScale = new Vector3((charHealth / maxHealth > 1 ? (charHealth - maxHealth) / maxHealth : 0), currHealthBar.transform.localScale.y, currHealthBar.transform.localScale.x);
        if (charEnergy / maxEnergy < 0)
            currHealthBar.transform.localScale = new Vector3(0, currHealthBar.transform.localScale.y, currHealthBar.transform.localScale.x);
        else
            currEnergyBar.transform.localScale = new Vector3(charEnergy / maxEnergy, currEnergyBar.transform.localScale.y, currEnergyBar.transform.localScale.x);
    }

    public void DisplayEnemy(CharacterStats enemy)
    {
        enemyDisplay.SetActive(true);
        currCharacter = enemy;
        bannerImage.sprite = enemy.GetComponent<Enemy>().GetBanner();
        enemyImage.sprite = enemy.portrait;
        float charHealth = enemy.GetHealth();
        float maxHealth = enemy.GetMaxHealth();
        enemyHealth.transform.localScale = new Vector3(charHealth / maxHealth, enemyHealth.transform.localScale.y, enemyHealth.transform.localScale.x);
        float charEnergy = enemy.GetEnergy();
        float maxEnergy = enemy.GetMaxEnergy();
        enemyEnergy.transform.localScale = new Vector3(charEnergy / maxEnergy, enemyEnergy.transform.localScale.y, enemyEnergy.transform.localScale.x);
    }
    // Used to temporarily enable and disable character display and confirm, cancel buttons
    public void ToggleCharDisplay(bool toActivate = false, bool activateCancel = false, bool activateConfirm = false)
    {
        characterDisplay.SetActive(toActivate);
        confirmButtons[0].gameObject.SetActive(activateConfirm);
        confirmButtons[1].gameObject.SetActive(activateCancel);
    }
    public void DisableEnemyDisplay()
    {
        enemyDisplay.SetActive(false);
    }

    public void DisplayStatus()
    {
        statusDisplay.SetActive(true);
        statusCharImage.sprite = currCharacter.portrait;
        SetStatBars(currCharacter, statusHealth, statusEnergy, statusOvercharge);
        int[] currStats = currCharacter.GetStats();
        int[] currTurns = currCharacter.GetDurations();
        for (int i = 0; i < statusNums.Length; i++)
        {
            statusNums[i].text = currStats[i] < 0 ? "" : currStats[i].ToString();
            statusTurnNums[i].text = currTurns[i] < 0 ? "" : currTurns[i].ToString();
        }
    }
    public void CloseStatus()
    {
        statusDisplay.SetActive(false);
    }

    // Used at end and start of turn
    public void DisplayMenu(bool toLock = false)
    {
        if (locked)
            return;

        if (characterDisplay.activeSelf)
            characterDisplay.SetActive(false);

        if (toLock && menu.activeSelf)
        {
            menu.SetActive(false);
            return;
        }

        menu.SetActive(!menu.activeSelf);
    }
    public void LockMenu(bool lockMenu)
    {
        locked = lockMenu;
    }

    // Toggles ability screen
    public void AbilityClick(CharacterStats unit, Tile currTile, bool items = false)
    {
        if (!items)
            actionButtons[1].gameObject.GetComponent<AbilityUI>().ToggleMenu(unit, currTile);
        else
            actionButtons[2].gameObject.GetComponent<AbilityUI>().ToggleMenu(unit, currTile, items);
    }
    // locks when ability menu is open
    public void LockAction(bool toLock, int unlockedButton)
    {
        // Locks and unlocks menu
        menuButton.SetActive(toLock);

        if (unlockedButton != 0)
            actionButtons[0].interactable = toLock;
        if (unlockedButton != 1)
            actionButtons[1].interactable = toLock;
        if (unlockedButton != 2)
            actionButtons[2].interactable = toLock;
    }
    public void UnlockMenu()
    {
        menuButton.SetActive(true);
    }
    public void LockSpecificAction(bool toLock, int lockedButton)
    {
        actionButtons[lockedButton].interactable = toLock;
    }

    public void Win()
    {
        win.SetActive(true);
    }
    public void Lose()
    {
        loss.SetActive(true);
    }
}
