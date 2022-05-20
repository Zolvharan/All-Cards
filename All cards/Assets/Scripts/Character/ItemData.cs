using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : AbilityData
{
    public int numUses;

    public ItemData(string newName, bool isDirected, bool isBiased, bool isAlly, int[] newPotencies, int[] newDurations, int[] newCostPotencies, int[] newCostDurations, int[] newGlobalStats, int[] newPushInts, int newNumUses)
        : base(newName, isDirected, isBiased, isAlly, newPotencies, newDurations, newCostPotencies, newCostDurations, newGlobalStats, newPushInts)
    {
        numUses = newNumUses;
    }

    // Constructs ability
    public Item ConstructItem(bool isPlayer)
    {
        Item newItem = new Item(abilityName, directed, biased, ally, potencies, durations, costPotencies, costDurations, globalStats, pushInts, numUses, isPlayer);
        return newItem;
    }
}
