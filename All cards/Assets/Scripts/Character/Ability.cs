using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ability
{
    public string abilityName;
    // Set when character is initialized
    bool usedByPlayer;

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
    public Dictionary<string, int> potencies;
    // Determines effect durations (0 for instant, positive for buffs/debuffs and HOT/DOTs)
    public Dictionary<string, int> durations;
    // Cost effects
    public Dictionary<string, int> costPotencies;
    public Dictionary<string, int> costDurations;
    // Pushes units. First 2 ints are push x and push y, second 2 are cost (self) push x and push y
    public int[] pushInts;

    public Ability(string newName, bool isDirected, bool isBiased, bool isAlly, int[] newPotencies, int[] newDurations, int[] newCostPotencies, int[] newCostDurations, int[] newGlobals, int[] newPushInts, bool isPlayer)
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
        pushInts = new int[4];
        newPushInts.CopyTo(pushInts, 0);
        usedByPlayer = isPlayer;
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
        //double hit;
        double posHit;
        double negHit;
        double randHitModifier;
        double currHit;
        foreach (Tile tile in tiles)
        {
            if (tile.currUnit != null)
            {
                if (!directed && biased && tile.currUnit.player != player)
                    reverseEffect = -1;
                // ignore unit if ability is directed and it is not a target
                else if (directed && ((tile.currUnit.player == player) != ally))
                    continue;
                // determine hit, factor dexterity into equation if effect is negative
                randHitModifier = random.NextDouble();
                posHit = 1 + (precision / 10) + randHitModifier;
                negHit = (100 * Math.Pow(CharacterStats.DEX_MULTIPLIER, tile.currUnit.GetStats()[7]) + (precision * 10)) / 100 + randHitModifier;
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
                    //hit = potencies[target] > 0 ? 1 + (precision / 10) + random.NextDouble() : (100 * Math.Pow(CharacterStats.DEX_MULTIPLIER, tile.currUnit.GetStats()[7]) + (precision * 10)) / 100 + random.NextDouble();
                    if ((potencies[target] > 0 && posHit >= 1) || (potencies[target] < 0 && negHit >= 1))
                    {
                        currHit = potencies[target] > 0 ? posHit : negHit;
                        // Ignore uneffected stats (checks for tile variable in case character died)
                        tile.currUnit.AddStatEffect(target, durations[target], (int)Math.Floor(potencies[target] * reverseEffect * (currHit >= 2 ? 1.5f : 1)));    // Ternary tests crit
                    }
                }
                // Move unit if ability pushes. Will not push if no push is present, or if ability fails to hit enemy.
                // Always pushes ally
                if ((pushInts[0] != 0 || pushInts[1] != 0) && ((tile.currUnit.player == usedByPlayer) || negHit >= 1))
                {
                    // TODO
                }
                // TODO: Miss display (also hit display)
                if (negHit < 1)
                {
                    UnityEngine.Debug.Log("Miss");
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
        // Apply self push
        if (pushInts[1] != 0 || pushInts[2] != 0)
        {
            // TODO
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
    public Dictionary<string, int> GetCostPotencies()
    {
        return costPotencies;
    }
    public Dictionary<string, int> GetCostDurations()
    {
        return costDurations;
    }
    public bool IsPlayer()
    {
        return usedByPlayer;
    }
    public bool IsBiased()
    {
        return biased;
    }
}