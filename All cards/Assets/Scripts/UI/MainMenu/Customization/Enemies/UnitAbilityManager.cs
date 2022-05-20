using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages the ability creation and editing menus
public class UnitAbilityManager : MonoBehaviour
{
    public EnemyGenManager enemyGenManager;
    public UnitFormManager unitFormManager;

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
    public static readonly int[] MIN_STAT_NUMS = { -10, -10, -10, -5, -10, -5, -5, -5, -10, -10, 0, 0, 0, -8, -8, -8, -8 };
    public static readonly int[] MAX_STAT_NUMS = { 10, 10, 10, 5, 10, 5, 5, 5, 10, 10, 10, 10, 4, 8, 8, 8, 8 };
    public static readonly int[] MIN_DURATION_NUMS = { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 };
    public static int MAX_DURATION = 5;

    bool isInChar;
    bool editing;

    // Accesses ability in char form
    public void OpenAbilityFormInCharacter(bool isEditing)
    {
        isInChar = true;
        charComponents.SetActive(isInChar);

        if (!isEditing)
        {
            unitFormManager.gameObject.SetActive(false);
            InitAbilityForm(false);
        }
        // Cannot edit if no ability is present
        else if (!(unitFormManager.abilityList.options.Count == 0))
        {
            unitFormManager.gameObject.SetActive(false);
            InitAbilityForm(true, unitFormManager.currAbilities[unitFormManager.abilityList.value]);
        }

        baseStatTexts[0].text = "(" + unitFormManager.GetStrength() + ")";
        baseStatTexts[1].text = "(" + unitFormManager.GetAttackRange() + ")";
        baseStatTexts[2].text = "(" + unitFormManager.GetPrecision() + ")";
    }
    // Accesses ability directly
    public void OpenAbilityForm(bool isEditing, AbilityData currAbility = null)
    {
        isInChar = false;
        charComponents.SetActive(isInChar);

        if (!isEditing)
            InitAbilityForm(false);
        // Cannot edit if no ability is present
        else if (enemyGenManager.lists[2].options.Count != 0)
            InitAbilityForm(true, SaveData.GetAbilities()[enemyGenManager.lists[2].value]);
    }
    void InitAbilityForm(bool isEditing, AbilityData currAbility = null)
    {
        this.gameObject.SetActive(true);

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
    }

    public void ExitAbilityForm(bool saveChanges)
    {
        if (isInChar)
            ReOpenCreationForm(saveChanges);
        else
        {
            enemyGenManager.OpenEnemies();
            this.gameObject.SetActive(false);
        }
    }
    // Used to apply and return
    void ReOpenCreationForm(bool saveChanges)
    {
        if (saveChanges)
        {
            AbilityData newAbility = new AbilityData(nameField.text, directed, biased, ally, potencies, durations, costPotencies, costDurations, globalStats, pushInts);
            ReOpenForm(newAbility);
        }
        else
        {
            unitFormManager.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    // Used to add loaded ability data to character
    public void ReOpenFormWithLoad(AbilityData loadAbility)
    {
        // Load data
        ReOpenForm(loadAbility);
    }
    public void ReOpenForm(AbilityData newAbility)
    {
        if (editing)
            unitFormManager.currAbilities[unitFormManager.abilityList.value] = newAbility;
        else
        {
            unitFormManager.currAbilities.Add(newAbility);
            unitFormManager.AdjustCapacity(true);
        }
        unitFormManager.RefreshAbilities();

        unitFormManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    // Opens ability saving form
    public void SaveAbility()
    {
        AbilityData newAbility = new AbilityData(nameField.text, directed, biased, ally, potencies, durations, costPotencies, costDurations, globalStats, pushInts);

        // If editing from main ability list, init save location to ability being edited
        enemyGenManager.OpenSavedAbilityData(newAbility, isInChar, !isInChar && editing ? enemyGenManager.lists[1].value : -1);
    }
    // Opens ability saving form in load
    public void LoadAbility()
    {
        enemyGenManager.OpenSavedAbilityData(null, isInChar);
    }

    // TODO: Calculate energy cost
    public void CalculateEnergyCost()
    {
        // Radius will probably multiply cost

        // TODO: total energy cost will adjust per character
        // If a cost exceeds max stat, excess cost is ignored
        // range, precision, and probably strength will give some discount
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
    }
    // Increment stat and display
    public void IncrementStat(int statIndex)
    {
        // If push
        if (statIndex >= 13)
        {
            // Adds 2 to index if it is cost
            if (!potencyView)
                statIndex += 2;

            if (pushInts[statIndex - 13] < MAX_STAT_NUMS[statIndex])
                statTextNums[statIndex].text = (++pushInts[statIndex - 13]).ToString();
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
    }
    public void DecrementStat(int statIndex)
    {
        // If push
        if (statIndex >= 13)
        {
            // Adds 2 to index if it is cost
            if (!potencyView)
                statIndex += 2;

            if (pushInts[statIndex - 13] > MIN_STAT_NUMS[statIndex])
                statTextNums[statIndex].text = (--pushInts[statIndex - 13]).ToString();
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
    }
}