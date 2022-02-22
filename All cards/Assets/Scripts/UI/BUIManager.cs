using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BUIManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject menuButton;
    public GameObject characterDisplay;
    public GameObject health;
    public GameObject energy;
    public GameObject overcharge;
    public SpriteRenderer charImage;
    public Button moveButton;
    public Button[] actionButtons;
    public Button[] confirmButtons;

    public GameObject enemyDisplay;
    public SpriteRenderer enemyImage;
    public GameObject enemyHealth;
    public GameObject enemyEnergy;

    // Forecast variables
    public GameObject forecastDisplay;
    public SpriteRenderer[] forePortraits;
    public GameObject[] foreBars;
    public Text[] currStats;
    public Text[] currDurations;
    public Text[] foreStats;
    public Text[] foreDurations;
    public Text[] chanceNums;

    public GameObject win;
    public GameObject loss;

    public bool locked = false;

    bool lockedMove = false;

    // Called by Tile.SelectTile
    public void DisplayCharacter(CharacterStats character)
    {
        if (menu.activeSelf)
            menu.SetActive(false);

        charImage.sprite = character.portrait;
        if (character.moved == true)
            moveButton.interactable = false;
        else
            moveButton.interactable = true;
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

        characterDisplay.SetActive(true);
    }

    // Used by ability/item
    public void DisplayForecast(CharacterStats currUnit, Ability currAbility)
    {
        if (currUnit != null)
        {
            forePortraits[0].sprite = currUnit.portrait;
            forePortraits[1].sprite = currUnit.portrait;

            SetStatBars(currUnit, foreBars[0], foreBars[1], foreBars[2]);
            SetStatBars(currUnit, foreBars[3], foreBars[4], foreBars[5], currUnit.GetOffsets(currAbility));

            SetForeStatNums(currStats, currUnit.GetStats());
            SetForeStatNums(foreStats, currUnit.GetOffsets(currAbility), currUnit.GetBaseStats(), currUnit.GetStats());
            SetForeDurationNums(currDurations, currUnit.GetDurations());
            SetForeDurationNums(foreDurations, currUnit.GetDurations(), currAbility.GetDurations());

            forecastDisplay.SetActive(true);
        }
    }
    // Used by attack
    public void DisplayForecast(CharacterStats currUnit, int strength, int precision)
    {
        if (currUnit != null)
        {
            forePortraits[0].sprite = currUnit.portrait;
            forePortraits[1].sprite = currUnit.portrait;

            int[] strengthArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // Calculate potency based on unit defense
            strengthArray[0] = -strength + currUnit.GetStats()[6] > 0 ? 0 : -strength + currUnit.GetStats()[6];
            strengthArray[10] = precision;

            SetStatBars(currUnit, foreBars[0], foreBars[1], foreBars[2]);
            SetStatBars(currUnit, foreBars[3], foreBars[4], foreBars[5], strengthArray);

            SetForeStatNums(currStats, currUnit.GetStats());
            SetForeStatNums(foreStats, strengthArray, currUnit.GetBaseStats(), currUnit.GetStats());
            SetForeDurationNums(currDurations, currUnit.GetDurations());
            SetForeDurationNums(foreDurations, currUnit.GetDurations());

            forecastDisplay.SetActive(true);
        }
    }
    public void HideForecast()
    {
        forecastDisplay.SetActive(false);
    }
    // Base stats are used for forecast, otherwise function is used to display current status
    // Curr stats are used for color coding purposes and for health and mana forecasting
    public void SetForeStatNums(Text[] currTexts, int[] numsToSet, int[] baseStats = null, int[] currStats = null)
    {
        // Color codes stat numbers based on positve or negative or neutral changes
        if (baseStats != null && currStats != null)
        {
            // Health and energy are not color coded
            numsToSet[0] += currStats[0];
            if (numsToSet[0] < 0)
                numsToSet[0] = 0;
            numsToSet[1] += currStats[1];
            if (numsToSet[1] < 0)
                numsToSet[1] = 0;
            for (int i = 2; i < currTexts.Length; i++)
            {
                numsToSet[i] += numsToSet[i] == 0 ? currStats[i] : baseStats[i];
                if (numsToSet[i] == currStats[i])
                    currTexts[i].color = UnityEngine.Color.grey;
                else if (numsToSet[i] > currStats[i])
                    currTexts[i].color = UnityEngine.Color.green;
                else
                    currTexts[i].color = UnityEngine.Color.red;
                if (numsToSet[i] < 0)
                    numsToSet[i] = 0;
            }
        }

        // Set forecast stat nums
        for (int i = 0; i < currTexts.Length; i++)
        {
            currTexts[i].text = numsToSet[i].ToString();
        }
        // Set forecast chance nums
        chanceNums[0].text = (Math.Round(100 * Math.Pow(CharacterStats.DEX_MULTIPLIER, numsToSet[7])) + (numsToSet[10] * 10)).ToString();
        chanceNums[1].text = (100 + (numsToSet[10] * 10)).ToString();
    }
    public void SetForeDurationNums(Text[] currTexts, int[] currDurations, int[] newDurations = null)
    {
        // Sets forecast durations if new effects are given, current durations otherwise
        // Set text to appropriate num if it exists, "" otherwise
        for (int i = 0; i < currTexts.Length; i++)
        {
            if (newDurations != null && newDurations[i] > 0)
                currTexts[i].text = newDurations[i].ToString();
            else if (currDurations[i] > 0)
                currTexts[i].text = currDurations[i].ToString();
            else
                currTexts[i].text = "";
        }
    }

    // Sets given stat bars
    void SetStatBars(CharacterStats currUnit, GameObject currHealthBar, GameObject currEnergyBar, GameObject currOverchargeBar, int[] offsets = null)
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


        if (unlockedButton != 3 && !moveButton.interactable && !toLock)
            lockedMove = true;
        else if (unlockedButton != 3 && lockedMove)
            lockedMove = false;
        else if (unlockedButton != 3)
            moveButton.interactable = toLock;
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
