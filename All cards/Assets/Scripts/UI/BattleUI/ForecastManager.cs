using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForecastManager : MonoBehaviour
{
    public BUIManager battleUIManager;

    public SpriteRenderer[] forePortraits;
    public GameObject[] foreBars;
    public Text[] currStats;
    public Text[] currDurations;
    public Text[] foreStats;
    public Text[] foreDurations;
    public Text[] currCostStats;
    public Text[] currCostDurations;
    public Text[] foreCostStats;
    public Text[] foreCostDurations;
    public Text[] chanceNums;

    // Used by ability/item
    public void DisplayForecast(CharacterStats currUnit, Ability currAbility, CharacterStats castingCharacter)
    {
        if (currUnit != null)
        {
            this.gameObject.SetActive(true);

            // Effect
            forePortraits[0].sprite = currUnit.portrait;
            forePortraits[1].sprite = currUnit.portrait;
            battleUIManager.SetStatBars(currUnit, foreBars[0], foreBars[1], foreBars[2]);
            battleUIManager.SetStatBars(currUnit, foreBars[3], foreBars[4], foreBars[5], currUnit.GetOffsets(currAbility, false));

            // Cost
            forePortraits[2].sprite = castingCharacter.portrait;
            forePortraits[3].sprite = castingCharacter.portrait;
            battleUIManager.SetStatBars(castingCharacter, foreBars[6], foreBars[7], foreBars[8]);
            battleUIManager.SetStatBars(castingCharacter, foreBars[9], foreBars[10], foreBars[11], castingCharacter.GetOffsets(currAbility, true));

            // Curr stats
            SetForeStatNums(currStats, currUnit.GetStats());
            SetForeDurationNums(currDurations, currUnit.GetDurations());
            // Forecasted stats
            SetForeStatNums(foreStats, currUnit.GetOffsets(currAbility, false), currUnit.GetBaseStats(), currUnit.GetStats());
            SetForeDurationNums(foreDurations, currUnit.GetDurations(), currAbility.GetDurations());
            // Curr cost stats
            SetForeStatNums(currCostStats, castingCharacter.GetStats());
            SetForeDurationNums(currCostDurations, castingCharacter.GetDurations());
            // Forcasted cost stats
            SetForeStatNums(foreCostStats, castingCharacter.GetOffsets(currAbility, true), castingCharacter.GetBaseStats(), castingCharacter.GetStats());
            SetForeDurationNums(foreCostDurations, castingCharacter.GetDurations(), currAbility.GetCostDurations());
        }
    }
    // Used by attack
    public void DisplayForecast(CharacterStats currUnit, int strength, int precision)
    {
        if (currUnit != null)
        {
            this.gameObject.SetActive(true);

            forePortraits[0].sprite = currUnit.portrait;
            forePortraits[1].sprite = currUnit.portrait;

            int[] strengthArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // Calculate potency based on unit defense
            strengthArray[0] = -strength + currUnit.GetStats()[6] > 0 ? 0 : -strength + currUnit.GetStats()[6];
            strengthArray[10] = precision;

            battleUIManager.SetStatBars(currUnit, foreBars[0], foreBars[1], foreBars[2]);
            battleUIManager.SetStatBars(currUnit, foreBars[3], foreBars[4], foreBars[5], strengthArray);

            SetForeStatNums(currStats, currUnit.GetStats());
            SetForeStatNums(foreStats, strengthArray, currUnit.GetBaseStats(), currUnit.GetStats());
            SetForeDurationNums(currDurations, currUnit.GetDurations());
            SetForeDurationNums(foreDurations, currUnit.GetDurations());
        }
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
        chanceNums[0].text = (System.Math.Round(100 * System.Math.Pow(CharacterStats.DEX_MULTIPLIER, numsToSet[7])) + (numsToSet[10] * 10)).ToString() + "%";
        chanceNums[1].text = (100 + (numsToSet[10] * 10)).ToString() + "%";
    }
    public void SetForeDurationNums(Text[] currTexts, int[] currDurations, Dictionary<string, int> newDurations = null)
    {
        // Sets forecast durations if new effects are given, current durations otherwise
        // Set text to appropriate num if it exists, "" otherwise
        for (int i = 0; i < currTexts.Length; i++)
        {
            if (newDurations != null && newDurations[CharacterStats.statTargets[i]] > 0)
                currTexts[i].text = newDurations[CharacterStats.statTargets[i]].ToString();
            else if (currDurations[i] > 0)
                currTexts[i].text = currDurations[i].ToString();
            else
                currTexts[i].text = "";
        }
    }
}
