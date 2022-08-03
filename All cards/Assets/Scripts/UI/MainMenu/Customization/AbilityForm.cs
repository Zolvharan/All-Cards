using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// Manages the ability creation and editing menus
public class AbilityForm : CreationForm
{
    GameObject frontPage;
    CharacterForm characterForm;
    public SaveLoadForm saveLoadForm;

    public Toggle directedToggle;
    public Toggle biasedToggle;
    public Toggle allyToggle;

    // Components that are only active when accessed from charform
    public GameObject charComponents;
    // Strength, attack range, precision
    public Text[] baseStatTexts;

    // View text, PDButton text, ECButton text
    public Text[] viewTexts;
    public GameObject[] statElements;
    public GameObject energyElements;
    public GameObject pushElements;

    public InputField nameField;
    // See Ability for descriptions
    bool directed;
    bool biased;
    bool ally;

    bool potencyView;
    bool effectView;
    int[] potencies;
    int[] durations;
    int[] costPotencies;
    int[] costDurations;
    // Precision, range, and radius
    int[] globalStats;
    // Pushes units
    int[] pushInts;
    public Text[] statTextNums;
    // order is: charForm order, health, energy, range, precision, radius
    public static readonly int[] MIN_STAT_NUMS = { -10, -10, -10, -5, -10, -5, -5, -5, -10, -10, 0, 0, 0};
    public static readonly int[] MAX_STAT_NUMS = { 10, 10, 10, 5, 10, 5, 5, 5, 10, 10, 10, 10, 4};
    public static readonly int[] MIN_DURATION_NUMS = { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 };
    public static int MAX_DURATION = 5;
    public static int MAX_PUSH = 8;

    bool isInChar;
    bool editing;
    Dropdown currList;

    public Text energyBaseTextNum;
    public Text energyAdjustedTextNum;
    int currBaseEnergy;
    int currStrength;
    int currRange;
    int currPrecision;

    const float BASE_ENERGY_COST = -1f;
    const float BASE_STAT_MODIFIER = 0.5f;
    const float PUSH_STAT_MODIFIER = 0.5f;
    const float COST_ENERGY_MODIFIER = 0.5f;
    const float DURATION_COST_MODIFIER = 1.5f;
    const float CURR_STRENGTH_MODIFIER = 0.5f;

    const float RANGE_COST_MODIFIER = 0.1f;
    const float PRECISION_COST_MODIFIER = 0.1f;
    const float RADIUS_COST_MODIFIER = 1f;

    const float DIRECTED_COST_MODIFIER = 1.2f;
    const float BIASED_COST_MODIFIER = 2f;

    const float NEGATIVE_RANGE_DISCOUNT = 0.1f;
    const float NEGATIVE_PRECISION_DISCOUNT = 0.1f;

    public void OpenAbilityForm(bool isEditing, Dropdown newList, AbilityData currAbility, GameObject newFrontPage, bool inChar, CharacterForm newCharacterForm = null, int[] currStats = null)
    {
        frontPage = newFrontPage;
        characterForm = newCharacterForm;
        currList = newList;
        isInChar = inChar;
        charComponents.SetActive(isInChar);

        if (!isEditing)
            InitAbilityForm(false);
        // Cannot edit if no ability is present
        else if (currList.options.Count != 0)
            InitAbilityForm(true, currAbility);

        if (currStats != null)
        {
            currStrength = currStats[0];
            currRange = currStats[1];
            currPrecision = currStats[2];
            baseStatTexts[0].text = "(" + currStrength + ")";
            baseStatTexts[1].text = "(" + currRange + ")";
            baseStatTexts[2].text = "(" + currPrecision + ")";
        }
        else
        {
            baseStatTexts[0].text = "";
            baseStatTexts[1].text = "";
            baseStatTexts[2].text = "";
        }

        this.gameObject.SetActive(true);
    }
    void InitAbilityForm(bool isEditing, AbilityData currAbility = null)
    {
        editing = isEditing;
        potencies = new int[10];
        durations = new int[10];
        costPotencies = new int[10];
        costDurations = new int[10];
        globalStats = new int[3];
        pushInts = new int[4];

        if (isEditing)
        {
            // Set ability nums
            currAbility.potencies.CopyTo(potencies, 0);
            currAbility.durations.CopyTo(durations, 0);
            currAbility.costPotencies.CopyTo(costPotencies, 0);
            currAbility.costDurations.CopyTo(costDurations, 0);
            currAbility.globalStats.CopyTo(globalStats, 0);
            currAbility.pushInts.CopyTo(pushInts, 0);
            nameField.text = currAbility.abilityName;

            directedToggle.isOn = currAbility.directed;
            biasedToggle.isOn = currAbility.biased;
            allyToggle.isOn = currAbility.ally;
        }
        else
        {
            // Set nums to base
            MIN_DURATION_NUMS.CopyTo(durations, 0);
            MIN_DURATION_NUMS.CopyTo(costDurations, 0);
            nameField.text = "";

            directedToggle.isOn = false;
            biasedToggle.isOn = false;
            allyToggle.isOn = false;
        }
        potencyView = true;
        effectView = true;
        viewTexts[0].text = "Effect Potency";
        viewTexts[1].text = "Duration";
        viewTexts[2].text = "Cost";
        SetViewNums();

        SetEnergyCost();
    }

    public void ExitAbilityForm(bool saveChanges)
    {
        if (isInChar)
            ReOpenCreationForm(saveChanges);
        else
        {
            frontPage.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    // Used to apply and return
    void ReOpenCreationForm(bool saveChanges)
    {
        if (saveChanges)
        {
            costPotencies[9] = CalculateEnergyCost(true);
            AbilityData newAbility = new AbilityData(nameField.text, directed, biased, ally, potencies, durations, costPotencies, costDurations, globalStats, pushInts);
            ReOpenForm(newAbility);
        }
        else
        {
            characterForm.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    public void ReOpenForm(AbilityData newAbility)
    {
        if (editing)
            characterForm.currAbilities[characterForm.abilityList.value] = newAbility;
        else
        {
            characterForm.currAbilities.Add(newAbility);
            characterForm.AdjustCapacity(true);
        }

        characterForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void SaveItem()
    {
        costPotencies[9] = CalculateEnergyCost(true);
        AbilityData newAbility = new AbilityData(nameField.text, directed, biased, ally, potencies, durations, costPotencies, costDurations, globalStats, pushInts);
        SaveData.SaveAbility(newAbility, !isInChar && editing ? saveLoadForm.GetSaveValue() - 1 : -1);
        if (isInChar)
            this.gameObject.SetActive(true);
        else
            frontPage.SetActive(true);
    }
    public override void LoadItem()
    {
        ReOpenForm(SaveData.GetAbilities()[saveLoadForm.GetLoadValue()]);
    }
    // Opens ability saving form
    public void OpenSaveForm()
    {
        List<string> names = new List<string>();
        foreach (AbilityData ability in SaveData.GetAbilities())
        {
            names.Add(ability.GetName());
        }

        saveLoadForm.gameObject.SetActive(true);
        // If editing from main ability list, init save location to ability being edited
        saveLoadForm.InitDisplay(this, names, false, !isInChar && editing ? currList.value : -1);
        this.gameObject.SetActive(false);
    }
    // Opens ability saving form in load
    public void OpenLoadForm()
    {
        List<string> names = new List<string>();
        foreach (AbilityData ability in SaveData.GetAbilities())
        {
            names.Add(ability.GetName());
        }

        saveLoadForm.gameObject.SetActive(true);
        saveLoadForm.InitDisplay(this, names, true, !isInChar && editing ? currList.value : -1);
        this.gameObject.SetActive(false);
    }

    // Adjusted calcuates current stat modifiers
    public int CalculateEnergyCost(bool adjusted)
    {
        float cost = BASE_ENERGY_COST;
        for (int i = 0; i < potencies.Length - 1; i++)
        {
            if (!adjusted || i != 2)
                cost += (Math.Abs(potencies[i]) * (durations[i] > 0 ? (durations[i] * DURATION_COST_MODIFIER) : 1)) * BASE_STAT_MODIFIER;
            // effect strength
            else
                cost += (Math.Abs(potencies[i] - (currStrength * CURR_STRENGTH_MODIFIER)) * (durations[i] > 0 ? (durations[i] * DURATION_COST_MODIFIER) : 1)) * BASE_STAT_MODIFIER;
            cost -= (costPotencies[i] * (costDurations[i] > 0 ? (costDurations[i] * DURATION_COST_MODIFIER) : 1)) * COST_ENERGY_MODIFIER * BASE_STAT_MODIFIER;
        }
        for (int i = 0; i < pushInts.Length; i++)
        {
            cost += Math.Abs(pushInts[i]) * PUSH_STAT_MODIFIER;
        }
        // Don't multiply energy restore
        if (cost >= 0)
        {
            if (globalStats[0] > 0)
            {
                // Multiply discount if stat is negative
                if (adjusted)
                    cost *= (globalStats[0] - currRange) * (RANGE_COST_MODIFIER + 1) * globalStats[0] - currRange < 0 ? -(NEGATIVE_RANGE_DISCOUNT + 1) : 1;
                else
                    cost *= globalStats[0] * (RANGE_COST_MODIFIER + 1);
            }
            if (globalStats[1] > 0)
            {
                if (adjusted)
                    cost *= (globalStats[1] - currPrecision) * (PRECISION_COST_MODIFIER + 1) * globalStats[1] - currPrecision < 0 ? -(NEGATIVE_PRECISION_DISCOUNT + 1) : 1;
                else
                    cost *= globalStats[1] * (PRECISION_COST_MODIFIER + 1);
            }
            if (globalStats[2] > 0)
                cost *= globalStats[2] * (RADIUS_COST_MODIFIER + 1);
            if (directed)
                cost *= DIRECTED_COST_MODIFIER;
            else if (biased)
                cost *= BIASED_COST_MODIFIER;
        }

        //return (int)Math.Round(cost);
        return 0;
    }
    public void SetEnergyCost()
    {
        currBaseEnergy = CalculateEnergyCost(false);
        energyBaseTextNum.text = currBaseEnergy.ToString() + " Energy";
        if (isInChar)
            energyAdjustedTextNum.text = "(" + CalculateEnergyCost(true).ToString() + ")";
    }

    // Edits targeting and toggle based on toggle parameter
    public void TargetingToggle(Toggle toggle)
    {
        if (toggle == biasedToggle)
        {
            biased = toggle.isOn;
        }
        else if (toggle == allyToggle)
        {
            ally = toggle.isOn;
        }
        else
        {
            directed = toggle.isOn;
            if (directed)
            {
                allyToggle.interactable = true;
                biasedToggle.interactable = false;
            }
            else
            {
                biasedToggle.interactable = true;
                allyToggle.interactable = false;
            }
        }
        SetEnergyCost();
    }
    // Increment stat and display
    public void IncrementStat(int statIndex)
    {
        // If push
        if (statIndex >= 13)
        {
            // Adds 2 to index if it is cost
            if (!effectView)
            {
                if (pushInts[statIndex - 11] < MAX_PUSH)
                    statTextNums[statIndex].text = (++pushInts[statIndex - 11]).ToString();
            }
            else
            {
                if (pushInts[statIndex - 13] < MAX_PUSH)
                    statTextNums[statIndex].text = (++pushInts[statIndex - 13]).ToString();
            }
        }
        // If precision, range, or radius, and less than max
        else if (statIndex >= 10 && globalStats[statIndex - 10] < MAX_STAT_NUMS[statIndex])
            statTextNums[statIndex].text = (++globalStats[statIndex - 10]).ToString();
        else if (statIndex < 10)
        {
            // Effect potency
            if (potencyView && effectView && potencies[statIndex] < MAX_STAT_NUMS[statIndex])
                statTextNums[statIndex].text = (++potencies[statIndex]).ToString();
            // Effect duration
            if (!potencyView && effectView && durations[statIndex] < MAX_DURATION)
                statTextNums[statIndex].text = (++durations[statIndex]).ToString();
            // Cost potency
            if (potencyView && !effectView && costPotencies[statIndex] < MAX_STAT_NUMS[statIndex])
                statTextNums[statIndex].text = (++costPotencies[statIndex]).ToString();
            // Cost duration
            if (!potencyView && !effectView && costDurations[statIndex] < MAX_DURATION)
                statTextNums[statIndex].text = (++costDurations[statIndex]).ToString();
        }
        SetEnergyCost();
    }
    public void DecrementStat(int statIndex)
    {
        // If push
        if (statIndex >= 13)
        {
            // Adds 2 to index if it is cost
            if (!effectView)
            {
                if (pushInts[statIndex - 11] > -MAX_PUSH)
                    statTextNums[statIndex].text = (--pushInts[statIndex - 11]).ToString();
            }
            else
            {
                if (pushInts[statIndex - 11] > -MAX_PUSH)
                    statTextNums[statIndex].text = (--pushInts[statIndex - 11]).ToString();
            }
        }
        // If precision, range, or radius, and more than min
        else if (statIndex >= 10 && globalStats[statIndex - 10] > MIN_STAT_NUMS[statIndex])
            statTextNums[statIndex].text = (--globalStats[statIndex - 10]).ToString();
        else if (statIndex < 10)
        {
            // Effect potency
            if (potencyView && effectView && potencies[statIndex] > MIN_STAT_NUMS[statIndex])
                statTextNums[statIndex].text = (--potencies[statIndex]).ToString();
            // Effect duration
            if (!potencyView && effectView && durations[statIndex] > MIN_DURATION_NUMS[statIndex])
                statTextNums[statIndex].text = (--durations[statIndex]).ToString();
            // Cost potency
            if (potencyView && !effectView && costPotencies[statIndex] > MIN_STAT_NUMS[statIndex])
                statTextNums[statIndex].text = (--costPotencies[statIndex]).ToString();
            // Cost duration
            if (!potencyView && !effectView && costDurations[statIndex] > MIN_DURATION_NUMS[statIndex])
                statTextNums[statIndex].text = (--costDurations[statIndex]).ToString();
        }
        SetEnergyCost();
    }

    // Sets view to or from potency
    public void SetPotency()
    {
        // Set view texts
        potencyView = !potencyView;
        viewTexts[0].text = effectView ? "Effect" : "Cost";
        if (potencyView)
        {
            viewTexts[0].text += " Potency";
            viewTexts[1].text = "Duration";
        }
        else
        {
            viewTexts[0].text += " Duration";
            viewTexts[1].text = "Potency";
        }

        SetViewNums();
    }
    // Sets view to or from effect
    public void SetEffect()
    {
        // Set view texts
        effectView = !effectView;
        if (effectView)
        {
            viewTexts[0].text = "Effect";
            viewTexts[2].text = "Cost";
        }
        else
        {
            viewTexts[0].text = "Cost";
            viewTexts[2].text = "Effect";
        }
        viewTexts[0].text += potencyView ? " Potency" : " Duration";

        SetViewNums();
    }
    // Sets the stat nums to the current view
    void SetViewNums()
    {
        // Effect potency
        if (potencyView && effectView)
        {
            for (int i = 0; i < 10; i++)
            {
                statTextNums[i].text = potencies[i].ToString();
            }
        }
        // Effect duration
        if (!potencyView && effectView)
        {
            for (int i = 0; i < 10; i++)
            {
                statTextNums[i].text = durations[i].ToString();
            }
        }
        // Cost potency
        if (potencyView && !effectView)
        {
            for (int i = 0; i < 10; i++)
            {
                statTextNums[i].text = costPotencies[i].ToString();
            }
        }
        // Cost duration
        if (!potencyView && !effectView)
        {
            for (int i = 0; i < 10; i++)
            {
                statTextNums[i].text = costDurations[i].ToString();
            }
        }

        // Duration inaccessable when potency is 0
        for (int i = 0; i < potencies.Length; i++)
        {
            // bool checks if it is set to potency (all fields are made active)
            // if not it checks if the effect or cost equivalent potency is 0 (such fields are made inactive)
            statElements[i].SetActive(((!effectView && costPotencies[i] != 0) || (effectView && potencies[i] != 0)) || potencyView);
        }

        // Cannot edit energy in cost or duration
        energyElements.SetActive(effectView && potencyView);
        // Cannot edit push in duration
        pushElements.SetActive(potencyView);

        statTextNums[10].text = globalStats[0].ToString();
        statTextNums[11].text = globalStats[1].ToString();
        statTextNums[12].text = globalStats[2].ToString();

        if (effectView)
        {
            statTextNums[13].text = pushInts[0].ToString();
            statTextNums[14].text = pushInts[1].ToString();
        }
        else
        {
            statTextNums[13].text = pushInts[2].ToString();
            statTextNums[14].text = pushInts[3].ToString();
        }
    }
}