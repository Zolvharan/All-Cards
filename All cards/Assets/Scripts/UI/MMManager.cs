using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the main menu
public class MMManager : MonoBehaviour
{
    public GameObject frontPage;
    public GameObject customBattle;
    public GameObject characters;
    public GameObject charForm;
    public GameObject abilityForm;

    const int MAX_CAPACITY = 25;
    int currCapacity;
    public Text capacityText;
    public Toggle flyingToggle;
    const int FLYING_COST = 5;
    public Text[] statTextNums;
    int[] statNums;
    // The highest and lowest numbers that stats can be
    // order is: MoveSpeed, AttackRange, Strength, EnergyRegen, Precision, Dexterity, Defense, Resistance
    readonly int[] MIN_STAT_NUMS = { 0, 0, 0, 0, 0, 0, 0, 0 };
    readonly int[] MAX_STAT_NUMS = { 10, 10, 10, 5, 10, 5, 5, 5 };
    readonly int[] DEFAULT_STAT_NUMS = { 1, 1, 1, 1, 0, 0, 0, 0 };

    // Front page methods
    public void OpenFrontPage()
    {
        frontPage.SetActive(true);
        customBattle.SetActive(false);
        characters.SetActive(false);
    }
    public void OpenCustomBattle()
    {
        customBattle.SetActive(true);
        frontPage.SetActive(false);
    }
    public void OpenCharacters()
    {
        characters.SetActive(true);
        frontPage.SetActive(false);
        charForm.SetActive(false);
    }
    public void Exit()
    {
        Application.Quit();
    }

    // Char form methods
    public void OpenCreationForm(bool isEditing)
    {
        // TODO: Save bool variable
        if (isEditing)
        {
            // TODO
            return;
        }
        else
        {
            statNums = new int[8];
            DEFAULT_STAT_NUMS.CopyTo(statNums, 0);
            flyingToggle.isOn = false;
        }
        // Initialize stat nums and curr capacity
        currCapacity = 0;
        for (int i = 0; i < statNums.Length; i++)
        {
            statTextNums[i].text = statNums[i].ToString();
            currCapacity += statNums[i];
        }
        if (flyingToggle.isOn)
            currCapacity += 4;
        capacityText.text = currCapacity.ToString() + '/' + MAX_CAPACITY;

        charForm.SetActive(true);
        characters.SetActive(false);
    }
    // Used by -+ buttons
    public void IncrementStat(int statIndex)
    {
        if (statNums[statIndex] < MAX_STAT_NUMS[statIndex])
        {
            statNums[statIndex]++;
            statTextNums[statIndex].text = statNums[statIndex].ToString();
            capacityText.text = (++currCapacity).ToString() + '/' + MAX_CAPACITY;
        }
    }
    public void DecrementStat(int statIndex)
    {
        if (statNums[statIndex] > MIN_STAT_NUMS[statIndex])
        {
            statNums[statIndex]--;
            statTextNums[statIndex].text = statNums[statIndex].ToString();
            capacityText.text = (--currCapacity).ToString() + '/' + MAX_CAPACITY;
        }
    }
    // Used by flying toggle
    public void ToggleFlying(Toggle flying)
    {
        capacityText.text = (currCapacity += flying.isOn ? FLYING_COST : -FLYING_COST).ToString() + '/' + MAX_CAPACITY;
    }

    // Ability form methods
    public void OpenAbilityForm(bool isEditing)
    {
        // TODO: Save bool variable
        if (isEditing)
        {
            // TODO
            return;
        }
        else
        {

        }
        abilityForm.SetActive(true);
        charForm.SetActive(false);
    }
    public void ReOpenCreationForm(bool saveChanges)
    {
        if (saveChanges)
        {
            // TODO: save changes
        }
        charForm.SetActive(true);
        abilityForm.SetActive(false);
    }
    // TODO: Saves ability to file
    public void SaveAbility()
    {
        ReOpenCreationForm(true);
    }

    // Custom battle methods
    public void StartBattle()
    {

    }

}
