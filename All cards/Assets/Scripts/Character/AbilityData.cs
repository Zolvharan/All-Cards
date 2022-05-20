using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityData
{
    public string abilityName;
    public bool directed;
    public bool biased;
    public bool ally;
    public int[] potencies;
    public int[] durations;
    public int[] costPotencies;
    public int[] costDurations;
    public int[] globalStats;
    public int[] pushInts;

    public AbilityData(string newName, bool isDirected, bool isBiased, bool isAlly, int[] newPotencies, int[] newDurations, int[] newCostPotencies, int[] newCostDurations, int[] newGlobalStats, int[] newPushInts)
    {
        abilityName = newName;
        directed = isDirected;
        biased = isBiased;
        ally = isAlly;
        potencies = new int[10];
        newPotencies.CopyTo(potencies, 0);
        durations = new int[10];
        newDurations.CopyTo(durations, 0);
        costPotencies = new int[10];
        newCostPotencies.CopyTo(costPotencies, 0);
        costDurations = new int[10];
        newCostDurations.CopyTo(costDurations, 0);
        globalStats = new int[3];
        newGlobalStats.CopyTo(globalStats, 0);
        pushInts = new int[4];
        newPushInts.CopyTo(pushInts, 0);

        for (int i = 0; i < potencies.Length; i++)
        {
            // Replace durations with 0 if corresponding potency is 0
            if (potencies[i] == 0)
                durations[i] = 0;
            if (costPotencies[i] == 0)
                costDurations[i] = 0;

            // If biased, make all potencies positive
            if (potencies[i] < 0 && !directed && biased)
                potencies[i] *= -1;
        }
    }

    // Constructs ability
    public Ability ConstructAbility(bool isPlayer)
    {
        Ability newAbility = new Ability(abilityName, directed, biased, ally, potencies, durations, costPotencies, costDurations, globalStats, pushInts, isPlayer);
        return newAbility;
    }

    public string GetName()
    {
        return abilityName;
    }
}
