using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;

// Character prefab data
public class OldCharacterStats : MonoBehaviour
{
    // Stat targets are const to keep accessing deterministic
    public static string[] statTargets = { "health", "energy", "moveSpeed", "attackRange", "strength", "defense", "dexterity", "precision", "resistance", "energyRegen" };

    public PlayerControl playerControl;

    public Ability[] abilities;
    public List<Item> items;

    // TODO: Following public variables should be private
    // and initialized with character creation and saved somewhere
    public Sprite portrait;
    // public Sprite fieldSprite
    public string characterName;
    // TODO: Following variables should not exist and are placeholders for initialization
    public int maxHealth; // Probably flat or close to, determines starting health
    public int maxEnergy; // Probably flat or close to, determines starting energy
    public int moveSpeed; // Affects the move speed of character
    public int attackRange; // Affects the attack range of all attacks
    public int energyRegen; // Determines the amount of energy regenerated every turn
    public int strength; // Affects the damage dealt to health by all health damaging abilites
    public int resistance; // counters and affects non-health targeting attacks
    public int precision; // counters dexterity and affects critical
    public int dexterity; // general counter, percent chance to dodge
    public int defense; // counters strength, flat damage barrier against all health damaging attacks
    public bool flying; // Allows character to travel with static tile move cost

    // Used to calculate accuracy
    public const float DEX_MULTIPLIER = 0.8f;

    Dictionary<string, int> baseStats;
    protected Dictionary<string, int> currStats;

    // For buffs/debuffs
    Dictionary<string, int> statEffectsPotencies;
    Dictionary<string, int> statEffectsDurations;

    public bool moved = false;
    public bool attacked = false;
    public bool dead = false;
    public bool player;

    public Tile currTile;

    // Initializes variables
    void Awake()
    {
        baseStats = new Dictionary<string, int>();
        currStats = new Dictionary<string, int>();
        statEffectsPotencies = new Dictionary<string, int>();
        statEffectsDurations = new Dictionary<string, int>();

        baseStats["health"] = maxHealth;
        baseStats["energy"] = maxEnergy;
        baseStats["moveSpeed"] = moveSpeed;
        baseStats["attackRange"] = attackRange;
        baseStats["strength"] = strength;
        baseStats["precision"] = precision;
        baseStats["defense"] = defense;
        baseStats["dexterity"] = dexterity;
        baseStats["resistance"] = resistance;
        baseStats["energyRegen"] = energyRegen;

        currStats["health"] = maxHealth;
        currStats["energy"] = maxEnergy;
        currStats["moveSpeed"] = moveSpeed;
        currStats["attackRange"] = attackRange;
        currStats["strength"] = strength;
        currStats["precision"] = precision;
        currStats["defense"] = defense;
        currStats["dexterity"] = dexterity;
        currStats["resistance"] = resistance;
        currStats["energyRegen"] = energyRegen;

        statEffectsPotencies["health"] = 0;
        statEffectsPotencies["energy"] = 0;
        statEffectsPotencies["moveSpeed"] = 0;
        statEffectsPotencies["attackRange"] = 0;
        statEffectsPotencies["strength"] = 0;
        statEffectsPotencies["precision"] = 0;
        statEffectsPotencies["defense"] = 0;
        statEffectsPotencies["dexterity"] = 0;
        statEffectsPotencies["resistance"] = 0;
        statEffectsPotencies["energyRegen"] = 0;

        statEffectsDurations["health"] = 0;
        statEffectsDurations["energy"] = 0;
        statEffectsDurations["moveSpeed"] = 0;
        statEffectsDurations["attackRange"] = 0;
        statEffectsDurations["strength"] = 0;
        statEffectsDurations["precision"] = 0;
        statEffectsDurations["defense"] = 0;
        statEffectsDurations["dexterity"] = 0;
        statEffectsDurations["resistance"] = 0;
        statEffectsDurations["energyRegen"] = 0;
    }

    public void NewTurn()
    {
        moved = false;
        attacked = false;

        currStats["energy"] += currStats["energyRegen"];
        if (currStats["energy"] > baseStats["energy"])
            currStats["energy"] = baseStats["energy"];

        Dictionary<string, int> temp = new Dictionary<string, int>();
        foreach (KeyValuePair<string, int> duration in statEffectsDurations)
        {
            temp.Add(duration.Key, duration.Value);
        }
        foreach (KeyValuePair<string, int> duration in statEffectsDurations)
        {
            // Health and energy are handled differently than other stats
            if ((duration.Key == "health" || duration.Key == "energy") && statEffectsDurations[duration.Key] > 0)
            {
                AdjustHealth(statEffectsPotencies[duration.Key], duration.Key == "health");
                if (currStats["health"] <= 0 && !dead)
                    Die();
            }
            else if (statEffectsDurations[duration.Key] == 1)
            {
                currStats[duration.Key] = baseStats[duration.Key];
            }
            temp[duration.Key]--;
        }
        foreach (KeyValuePair<string, int> duration in temp)
        {
            statEffectsDurations[duration.Key] = temp[duration.Key];
        }
    }

    public void Attack(CharacterStats unit)
    {
        // Determine attack hit
        System.Random random = new System.Random();
        double hit = (100 * Math.Pow(DEX_MULTIPLIER, unit.GetStats()[7]) + (currStats["precision"] * 10)) / 100 + random.NextDouble();
        // Crit
        if (hit >= 2)
            unit.TakeDamage((int)Math.Floor(strength * 1.5f));
        // Hit
        if (hit >= 1)
            unit.TakeDamage(strength);
        else
        {
            UnityEngine.Debug.Log("Miss");
            // TODO: Miss display (also hit display)
        }
        attacked = true;
    }
    // returns whether target is friend or foe
    public bool CanAttack(CharacterStats unit)
    {
        return player != unit.player;
    }
    // Used for forecasting
    public Ability GetAbility(int abilityIndex, bool isItems)
    {
        return isItems ? items[abilityIndex] : abilities[abilityIndex];
    }
    public void UseAbility(int abilityIndex, HashSet<Tile> tiles, bool isItems)
    {
        if (!isItems)
        {
            //abilities[abilityIndex].UseAbility(tiles, player, this);
        }
        else
        {
            // Use item and remove it if it is expended
            //if (items[abilityIndex].UseItem(tiles, player, this))
            //    items.RemoveAt(abilityIndex);
        }
        attacked = true;
    }
    public void TakeDamage(int attackPower)
    {
        attackPower -= currStats["defense"];
        if (attackPower > 0)
        {
            currStats["health"] -= attackPower;
            if (currStats["health"] <= 0)
                Die();
        }
    }
    virtual protected void Die()
    {
        dead = true;
        currTile.ClearUnit(false);
        playerControl.ScanTeam();
        this.gameObject.SetActive(false);
    }

    // bypass is used for when an ability cost is applied
    public void AddStatEffect(string statName, int duration, int potency, bool bypassDefAndRes = false)
    {
        // Adjust numbers for resistance
        if (statName != "health" && statName != "energy" && potency < 0 && !bypassDefAndRes)
        {
            potency += currStats["resistance"];
            // Penalty cannot be positive
            if (potency > 0)
                potency = 0;
        }

        if (statEffectsDurations.ContainsKey(statName))
        {
            // If stat is not health or energy, reset stat before setting it
            if (statName != "health" && statName != "energy")
               currStats[statName] = baseStats[statName];

            // overwrites existing effects
            statEffectsDurations[statName] = duration;
            statEffectsPotencies[statName] = potency;
        }
        else
        {
            statEffectsDurations.Add(statName, duration);
            statEffectsPotencies.Add(statName, potency);
        }

        if (statName == "health" || statName == "energy")
            AdjustHealth(potency, statName == "health", bypassDefAndRes);
        else
        {
            currStats[statName] += potency;
            // Stats cannot be brought below zero
            if (currStats[statName] < 0)
                currStats[statName] = 0;
        }
        if (currStats["health"] <= 0)
            Die();
    }
    // Health is handled differently from other stats (function handles energy too)
    private void AdjustHealth(int potency, bool health, bool bypassDefAndRes = false)
    {
        if (health)
        {
            // If damage is given, reduce by potency minus defense
            if (potency < 0)
            {
                if (!bypassDefAndRes)
                    potency += currStats["defense"];
                // Damage cannot be positive
                if (potency > 0)
                    potency = 0;
                currStats["health"] += potency;
            }

            else
                currStats["health"] += GetHealing(potency);
        }
        else
        {
            if (potency < 0)
            {
                if (!bypassDefAndRes)
                    potency += currStats["resistance"];
                // Damage cannot be positive
                if (potency > 0)
                    potency = 0;
            }

            currStats["energy"] += potency;

            // Keep energy within the upper limit and zero
            if (currStats["energy"] > baseStats["energy"])
                currStats["energy"] = baseStats["energy"];
            else if (currStats["energy"] < 0)
                currStats["energy"] = 0;
        }
    }
    private int GetHealing(int potency)
    {
        // Overcharge healing is half as effective
        if (currStats["health"] >= baseStats["health"])
            potency /= 2;
        else if (currStats["health"] + potency > baseStats["health"])
            potency -= (currStats["health"] + potency - baseStats["health"]) / 2;

        // Keep health within the upper limit
        if (currStats["health"] + potency > baseStats["health"] * 2)
            potency = -(currStats["health"] - baseStats["health"] * 2);

        return potency;
    }

    public bool HasMana(int abilityIndex)
    {
        return abilities[abilityIndex].costPotencies["energy"] <= currStats["energy"];
    }
    public int GetHealth()
    {
        return currStats["health"];
    }
    public int GetMaxHealth()
    {
        return baseStats["health"];
    }
    public int GetEnergy()
    {
        return currStats["energy"];
    }
    public int GetMaxEnergy()
    {
        return baseStats["energy"];
    }
    public int GetMoveSpeed()
    {
        return currStats["moveSpeed"];
    }
    public int GetAttackRange()
    {
        return currStats["attackRange"];
    }

    // Used for forecasting
    public int[] GetStats()
    {
        int[] stats = {currStats["health"], currStats["energy"], currStats["moveSpeed"], currStats["attackRange"], currStats["strength"],
            currStats["precision"], currStats["defense"], currStats["dexterity"], currStats["resistance"], currStats["energyRegen"], currStats["precision"] };
        return stats;
    }
    public int[] GetBaseStats()
    {
        int[] stats = {baseStats["health"], baseStats["energy"], baseStats["moveSpeed"], baseStats["attackRange"], baseStats["strength"],
            baseStats["precision"], baseStats["defense"], baseStats["dexterity"], baseStats["resistance"], baseStats["energyRegen"] };
        return stats;
    }
    public int[] GetDurations()
    {
        int[] durations = {statEffectsDurations["health"], statEffectsDurations["energy"], statEffectsDurations["moveSpeed"],
            statEffectsDurations["attackRange"], statEffectsDurations["strength"], statEffectsDurations["precision"], statEffectsDurations["defense"],
            statEffectsDurations["dexterity"], statEffectsDurations["resistance"], statEffectsDurations["energyRegen"] };
        return durations;
    }
    public int[] GetOffsets(Ability usedAbility)
    {
        // Stat effects are mitigated by defense and resistance when potency is negative
        Dictionary<string, int> potencies = usedAbility.GetPotencies();
        int[] offsets = new int[potencies.Count];

        // Ignore offsets if spell will not hit unit
        if (usedAbility.directed && player != usedAbility.ally)
        {
            for (int i = 0; i < 10; i++)
                offsets[i] = 0;
        }
        else
        {
            // Health is affected by defense
            if (potencies["health"] != 0)
            {
                if (potencies["health"] < 0)
                {
                    offsets[0] = potencies["health"] + currStats["defense"];
                    // Keep negative effects from becoming positive;
                    if (offsets[0] > 0)
                        offsets[0] = 0;
                }
                else
                    offsets[0] = GetHealing(potencies["health"]);
            }
            else
                offsets[0] = 0;

            // The rest are resistance
            for (int i = 1; i < offsets.Length; i++)
            {
                if (potencies[statTargets[i]] < 0)
                {
                    offsets[i] = potencies[statTargets[i]] + currStats["resistance"];
                    // Keep negative effects from becoming positive;
                    if (offsets[i] > 0)
                        offsets[i] = 0;
                }
                else
                    offsets[i] = potencies[statTargets[i]];
            }
        }

        // Pass in the current precision for forecasting purposes
        offsets[10] = usedAbility.precision;

        return offsets;
    }
}