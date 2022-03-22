using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Item form manager
public class MIManager : MonoBehaviour
{
    public MMManager mainManager;
    public CCManager ccManager;
    public SIManager saveForm;

    public GameObject optionsForm;
    public Toggle ranged;
    public Toggle precise;
    public Toggle AOE;
    public Toggle minus;
    public Toggle complex;
    public Toggle costly;
    public Toggle offensive;
    public Toggle biasedToggle;
    public Slider magnitude;

    public GameObject createComponents;
    public Text nameText;
    public InputField nameField;

    // View text, PDButton text, ECButton text
    public Text[] viewTexts;
    public GameObject[] statElements;
    public GameObject energyElements;
    bool potencyView;
    bool effectView;

    public Toggle directed;
    public Toggle biasedAlly;
    public Text biasedAllyText;
    int[] potencies;
    int[] durations;
    int[] costPotencies;
    int[] costDurations;
    // Precision, range, and radius
    int[] globalStats;
    public Text[] statTextNums;
    int numUses;
    public Text numUsesText;

    float BASE_ALLOCATION_POINTS = 4;
    float POINT_VARIANCE = 2;
    float MAGNITUDE_EFFECT = 0.35f;
    float NEGATIVE_FALLOFF = 0.8f;
    float BIASED_FALLOFF = 0.8f;
    int MAX_STATS_EFFECTED = 2;

    public void OpenItemForm(bool isViewing)
    {
        createComponents.SetActive(!isViewing);
        globalStats = new int[3];

        if (isViewing)
        {
            // Cannot view if no items exist
            if (ccManager.lists[2].options.Count == 0)
                return;

            ItemData currItem = SaveData.GetItems()[ccManager.lists[2].value];
            nameText.text = currItem.abilityName;
            currItem.potencies.CopyTo(potencies, 0);
            currItem.durations.CopyTo(durations, 0);
            currItem.costPotencies.CopyTo(costPotencies, 0);
            currItem.costDurations.CopyTo(costDurations, 0);
            currItem.globalStats.CopyTo(globalStats, 0);

            directed.isOn = currItem.directed;
            biasedAlly.isOn = directed ? currItem.ally : currItem.biased;
            biasedAllyText.text = directed ? "Ally" : "Biased";

            nameField.text = currItem.GetName();
        }
        else
        {
            nameText.text = "Item";
            nameField.text = "";
            RandomizeItem();
        }

        optionsForm.SetActive(false);
        potencyView = true;
        effectView = true;
        viewTexts[0].text = "Effect Potency";
        viewTexts[1].text = "Duration";
        viewTexts[2].text = "Cost";
        SetViewNums();

        mainManager.OpenItemForm();
    }
    public void OpenRandomOptions()
    {
        optionsForm.SetActive(!optionsForm.activeSelf);
    }

    public void SaveItem()
    {
        ItemData newItem = new ItemData(nameField.text, directed.isOn, biasedAlly.isOn, biasedAlly.isOn, potencies, durations, costPotencies, costDurations, globalStats, numUses);

        saveForm.gameObject.SetActive(true);
        saveForm.InitDisplay(newItem);
        this.gameObject.SetActive(false);
    }

    public void RandomizeItem()
    {
        // Empty potencies and durations
        potencies = new int[10];
        durations = new int[10];
        costPotencies = new int[10];
        costDurations = new int[10];

        numUses = (int)magnitude.maxValue - (int)magnitude.value + 1;
        numUsesText.text = numUses.ToString() + (numUses != 1 ? " uses" : " use");

        // Directed is enabled if biased is selected, otherwise directed is random
        if (biasedToggle.isOn)
        {
            directed.isOn = false;
            biasedAlly.isOn = true;
        }
        else
        {
            directed.isOn = Random.Range(0, 2) == 0 ? false : true;
            if (directed.isOn)
                biasedAlly.isOn = !offensive.isOn;
            else
                biasedAlly.isOn = false;
        }
        biasedAllyText.text = directed.isOn ? "Ally" : "Biased";
        // Random point variance
        float variance = Random.Range(-POINT_VARIANCE, POINT_VARIANCE);
        // Total points to allocate, based on complex toggle, costly toggle, AOE toggle, biased toggle and magnitude
        float allocationPoints = (BASE_ALLOCATION_POINTS + variance + (costly.isOn ? BASE_ALLOCATION_POINTS / 2 : 0) + (minus.isOn ? BASE_ALLOCATION_POINTS / 2 : 1) - (AOE.isOn ? BASE_ALLOCATION_POINTS / 2 : 1))
            * (magnitude.value * MAGNITUDE_EFFECT + 1) * (biasedToggle.isOn ? BIASED_FALLOFF : 1);
        // Negative points. Points are set to 0 if minus is not enabled
        float negativePoints = minus.isOn ? (BASE_ALLOCATION_POINTS + variance) * ((magnitude.value * MAGNITUDE_EFFECT) + 1) * NEGATIVE_FALLOFF : 0;

        List<int> newStatTargets = new List<int>();
        int currStatTarget;
        // at least 50/50 chance of health being affected
        if (Random.Range(0, 2) == 0)
            newStatTargets.Add(8);
        // Collect random stats to affect. Complex adds additional affected stats
        for (int i = 0; i < Random.Range((1 - newStatTargets.Count) + (complex.isOn ? MAX_STATS_EFFECTED : 0), MAX_STATS_EFFECTED * (complex.isOn ? 3 : 1)); i++)
        {
            currStatTarget = Random.Range(0, CharacterStats.statTargets.Length);
            if (!newStatTargets.Contains(currStatTarget))
                newStatTargets.Add(currStatTarget);
            else
                continue;
        }
        // Negative stats can be up to half of max
        List<int> newMinusStatTargets = new List<int>();
        if (minus.isOn)
        {
            for (int i = 0; i < Random.Range(1 * (complex.isOn ? MAX_STATS_EFFECTED / 2 : 1), MAX_STATS_EFFECTED / 2 * (complex.isOn ? 2 : 1)); i++)
            {
                currStatTarget = Random.Range(0, CharacterStats.statTargets.Length);
                if (!newStatTargets.Contains(currStatTarget) && !newMinusStatTargets.Contains(currStatTarget))
                    newMinusStatTargets.Add(currStatTarget);
                else
                    continue;
            }
        }
        // Cost stats can be up to half of max
        List<int> newCostStatTargets = new List<int>();
        if (costly.isOn)
        {
            for (int i = 0; i < Random.Range(1 * (complex.isOn ? MAX_STATS_EFFECTED / 2 : 1), MAX_STATS_EFFECTED / 2 * (complex.isOn ? 2 : 1)); i++)
            {
                currStatTarget = Random.Range(0, CharacterStats.statTargets.Length);
                if (!newCostStatTargets.Contains(currStatTarget))
                    newCostStatTargets.Add(currStatTarget);
                else
                    continue;
            }
        }

        // First allocate global stats
        int maxGlobal = ((int)((float)allocationPoints * 0.4f) < MAManager.MAX_STAT_NUMS[10]) ? (int)((float)allocationPoints * 0.4f) : MAManager.MAX_STAT_NUMS[10];
        maxGlobal = maxGlobal < 2 ? 2 : maxGlobal;
        // If ranged, random points are allocated to it up to a max of 40% allocated points, otherwise range is 0 or 1
        globalStats[0] = ranged.isOn ? Random.Range(2, maxGlobal) : Random.Range(0, 2);
        allocationPoints -= globalStats[0];
        // If precise,  random points are allocated to it up to a max of 40% allocated points, otherwise 0
        globalStats[1] = precise.isOn ? Random.Range(2, maxGlobal) : 0;
        allocationPoints -= globalStats[1];
        // If not AOE, radius = 0. Else radius is random from 1 to max
        globalStats[2] = AOE.isOn ? Random.Range(1, MAManager.MAX_STAT_NUMS[12]) : 0;

        // Set costs
        float currCostPoints = allocationPoints / Random.Range(2, 4);
        if (costly.isOn)
        {
            while (currCostPoints > 0 && newCostStatTargets.Count > 0)
            {
                costPotencies[newCostStatTargets[Random.Range(0, newCostStatTargets.Count)]]--;
                currCostPoints--;
            }
        }
        // Set effects
        while (allocationPoints > 0 && newStatTargets.Count > 0)
        {
            potencies[newStatTargets[Random.Range(0, newStatTargets.Count)]]++;
            allocationPoints--;
        }
        while (negativePoints > 0 && newMinusStatTargets.Count > 0)
        {
            potencies[newMinusStatTargets[Random.Range(0, newMinusStatTargets.Count)]]--;
            negativePoints--;
        }

        for (int i = 0; i < potencies.Length; i++)
        {
            // Set effect to max if effect exceeds max
            if (potencies[i] > MAManager.MAX_STAT_NUMS[i])
                potencies[i] = MAManager.MAX_STAT_NUMS[i];
            // Reverse potencies if offensive
            if (offensive.isOn)
                potencies[i] *= -1;
        }
        // Set durations for effects
        for (int i = 0; i < potencies.Length; i++)
        {
            // Health
            if (i == 8)
            {
                // 75% chance ignore duration
                if (Random.Range(0, 4) == 3)
                {
                    durations[i] = Random.Range(1, MAManager.MAX_DURATION + 1);
                    costDurations[i] = Random.Range(1, MAManager.MAX_DURATION + 1);
                }
            }
            // Energy cannot effect duration
            else if (i != 9)
            {
                durations[i] = Random.Range(1, MAManager.MAX_DURATION + 1);
                costDurations[i] = Random.Range(1, MAManager.MAX_DURATION + 1);
            }
        }

        SetViewNums();
    }

    // Some option toggle are mutually exclusive
    public void SetOffensiveOrMinus()
    {
        biasedToggle.isOn = false;
    }
    public void SetBiased()
    {
        offensive.isOn = false;
        minus.isOn = false;
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
            // or if the effect or cost equivalent potency is 0 (such fields are made inactive)
            statElements[i].SetActive(((!effectView && costPotencies[i] != 0) || (effectView && potencies[i] != 0)) || potencyView);
        }

        // Cannot edit energy in duration
        energyElements.SetActive(potencyView ? true : false);

        statTextNums[10].text = globalStats[0].ToString();
        statTextNums[11].text = globalStats[1].ToString();
        statTextNums[12].text = globalStats[2].ToString();
    }
}
