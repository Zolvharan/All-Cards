using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ability
{
    public string abilityName;
    // Player created ability data

    // AOE targeting bools and ints
    // determines if ability hits everything or just one side of battle
    public bool directed;
    // determines if effect is different depending on allegiance of unit (irrelevant if directed)
    public bool biased;
    // determines if ability hits ally or enemy (irrelevant if not directed)
    public bool ally;

    // determines tile radius (0 is used for single target)
    public int radius;
    // determines distance over which ability can be used (0 for caster only)
    public int range;
    // determines ability accuracy
    public int precision;

    // Each element of a given index in each array all correspond to each other

    // Determines what the ability affects
    // TODO (can include position)
    public Dictionary<string, int> potencies;
    // Determines effect durations (0 for instant, positive for buffs/debuffs and HOT/DOTs)
    public Dictionary<string, int> durations;
    // Cost effects
    public Dictionary<string, int> costPotencies;
    public Dictionary<string, int> costDurations;

    public Ability(string newName, bool isDirected, bool isBiased, bool isAlly, int[] newPotencies, int[] newDurations, int[] newCostPotencies, int[] newCostDurations, int[] newGlobals)
    {
        abilityName = newName;
        directed = isDirected;
        biased = isBiased;
        ally = isAlly;
        potencies = new Dictionary<string, int>();
        InitStatDict(potencies, newPotencies);
        durations = new Dictionary<string, int>();
        InitStatDict(durations, newDurations);
        costPotencies = new Dictionary<string, int>();
        InitStatDict(costPotencies, newCostPotencies);
        costDurations = new Dictionary<string, int>();
        InitStatDict(costDurations, newCostDurations);
        radius = newGlobals[2];
        range = newGlobals[0];
        precision = newGlobals[1];
    }
    // copySource order could change in the future
    void InitStatDict(Dictionary<string, int> copyTarget, int[] copySource)
    {
        copyTarget["moveSpeed"] = copySource[0];
        copyTarget["attackRange"] = copySource[1];
        copyTarget["strength"] = copySource[2];
        copyTarget["energyRegen"] = copySource[3];
        copyTarget["precision"] = copySource[4];
        copyTarget["dexterity"] = copySource[5];
        copyTarget["defense"] = copySource[6];
        copyTarget["resistance"] = copySource[7];
        copyTarget["health"] = copySource[8];
        copyTarget["energy"] = copySource[9];
    }

    // player determines relative friend and foe, caster is used to apply cost
    public void UseAbility(HashSet<Tile> tiles, bool player, CharacterStats caster)
    {
        // Used for biased abilities
        int reverseEffect = 1;
        System.Random random = new System.Random();
        double hit;
        foreach (Tile tile in tiles)
        {
            if (tile.currUnit != null)
            {
                if (!directed && biased && tile.currUnit.player != player)
                    reverseEffect = -1;
                // ignore unit if ability is directed and it is not a target
                else if (directed && ((tile.currUnit.player == player) != ally))
                    continue;
                // affect stat of each unit in possible AOE across duration
                foreach (string target in CharacterStats.statTargets)
                {
                    // Break if unit dies
                    if (tile.currUnit == null)
                        break;
                    // Ignore uneffected stats
                    if (potencies[target] == 0)
                        continue;
                    // determine hit, factor dexterity into equation if effect is negative
                    hit = potencies[target] > 0 ? 1 + (precision / 10) + random.NextDouble() : (100 * Math.Pow(CharacterStats.DEX_MULTIPLIER, tile.currUnit.GetStats()[7]) + (precision * 10)) / 100 + random.NextDouble();
                    if (hit >= 1)
                    {
                        // Ignore uneffected stats (checks for tile variable in case character died)
                        tile.currUnit.AddStatEffect(target, durations[target], (int)Math.Floor(potencies[target] * reverseEffect * (hit >= 2 ? 1.5f : 1)));    // Ternary tests crit
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Miss");
                        // TODO: Miss display (also hit display)
                    }
                }
            }
            reverseEffect = 1;
        }
        // Apply cost, cost ignores evasion and mitigation
        foreach (string target in CharacterStats.statTargets)
        {
            // Ignore uneffected stats
            if (costPotencies[target] == 0)
                continue;
            caster.AddStatEffect(target, costDurations[target], -costPotencies[target], true);
        }
    }

    public Dictionary<string, int> GetPotencies()
    {
        return potencies;
    }
    public Dictionary<string, int> GetDurations()
    {
        return durations;
    }
}