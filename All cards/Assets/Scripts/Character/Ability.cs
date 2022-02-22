using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ability : MonoBehaviour
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

    public int energyCost;

    // Each element of a given index in each array all correspond to each other
    // Determines what the ability affects (can include position)
    public int[] potencies;
    // Determines effect durations (0 for instant, positive for buffs/debuffs and HOT/DOTs)
    public int[] durations;
    // Stat targets are const to keep accessing deterministic
    static string[] statTargets = { "health", "energy", "moveSpeed", "attackRange", "strength", "defense", "dexterity", "precision", "resistance", "energyRegen" };

    public void UseAbility(HashSet<Tile> tiles, bool player)
    {
        // Used for biased abilities
        int reverseEffect = 1;
        int i;
        System.Random random = new System.Random();
        double hit;
        foreach (Tile tile in tiles)
        {
            if (tile.currUnit != null)
            {
                if (!directed && biased && tile.currUnit.player != player)
                    reverseEffect = -1;
                // ignore unit if ability is directed and it is not a target
                if (directed && ((tile.currUnit.player == player) != ally))
                    continue;
                // affect stat of each unit in possible AOE across duration
                for (i = 0; i < statTargets.Length; i++)
                {
                    // Break if unit dies
                    if (tile.currUnit == null)
                        break;
                    // Ignore uneffected stats
                    if (potencies[i] == 0)
                        continue;
                    // determine hit, factor dexterity into equation if effect is negative
                    hit = potencies[i] > 0 ? 1 + (precision / 10) + random.NextDouble() : (100 * Math.Pow(CharacterStats.DEX_MULTIPLIER, tile.currUnit.GetStats()[7]) + (precision * 10)) / 100 + random.NextDouble();
                    if (hit >= 1)
                    {
                        // Ignore uneffected stats (checks for tile variable in case character died)
                        tile.currUnit.AddStatEffect(statTargets[i], durations[i], (int)Math.Floor(potencies[i] * reverseEffect * (hit >= 2 ? 1.5f : 1)));    // Ternary tests crit
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
    }

    public int[] GetPotencies()
    {
        return potencies;
    }
    public int[] GetDurations()
    {
        return durations;
    }
}