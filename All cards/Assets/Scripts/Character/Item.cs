using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Ability
{
    // Procedurally generated item data
    // Created from ability and effectively a limited use free ability

    public int numUses;

    public Item(string newName, bool isDirected, bool isBiased, bool isAlly, int[] newPotencies, int[] newDurations, int[] newCostPotencies, int[] newCostDurations, int[] newGlobals, int[] newPushInts, int newNumUses, bool isPlayer)
        : base(newName, isDirected, isBiased, isAlly, newPotencies, newDurations, newCostPotencies, newCostDurations, newGlobals, newPushInts, isPlayer)
    {
        /*
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
        radius = newGlobals[2];
        range = newGlobals[0];
        precision = newGlobals[1];
        */
        numUses = newNumUses;
    }

    public bool UseItem(HashSet<Tile> tiles, Tile aimedPoint, bool player, CharacterStats caster, ActionDisplay actionDisplay)
    {
        UseAbility(tiles, aimedPoint, player, caster, actionDisplay);
        numUses--;
        if (numUses == 0)
            return true;
        else
            return false;
    }
}
