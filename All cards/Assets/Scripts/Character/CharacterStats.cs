using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Character prefab data
public class CharacterStats : MonoBehaviour
{
    // Stat targets are const to keep accessing deterministic
    public static string[] statTargets = { "health", "energy", "moveSpeed", "attackRange", "strength", "energyRegen", "precision", "dexterity", "defense", "resistance"};

    public PlayerControl playerControl;

    public Ability[] abilities;
    public List<Item> items;

    public string characterName;
    public Sprite portrait;
    public bool flying; // Allows character to travel with static tile move cost

    // Used to calculate accuracy
    public const float DEXTERITY_MULTIPLIER = 0.8f;
    public const float PRECISION_MULTIPLIER = 10f;

    protected Dictionary<string, int> baseStats;
    protected Dictionary<string, int> currStats;

    // For buffs/debuffs
    protected Dictionary<string, int> statEffectsPotencies;
    protected Dictionary<string, int> statEffectsDurations;

    public bool moved = false;
    public bool attacked = false;
    public bool dead = false;
    public bool player;

    public Tile currTile;

    const float CRIT_MULTIPLIER = 1.5f;
    const float OVERHEAL_FALLOFF = 0.5f;

    // Initializes variables
    public void ConstructCharacter(string newName, Ability[] newAbilities, Sprite newPortrait, Sprite newBattleSprite, bool isFlying, Dictionary<String, int> newBaseStats, List<Item> newItems, bool isPlayer)
    {
        player = isPlayer;

        characterName = newName;
        abilities = newAbilities;
        items = newItems;
        portrait = newPortrait;
        this.GetComponent<SpriteRenderer>().sprite = newBattleSprite;
        flying = isFlying;

        baseStats = new Dictionary<string, int>(newBaseStats);
        currStats = new Dictionary<string, int>(baseStats);
        // Energy always starts empty
        currStats["energy"] = 0;

        statEffectsPotencies = new Dictionary<string, int>();

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

        statEffectsDurations = new Dictionary<string, int>(statEffectsPotencies);
    }
    public void AddItems(List<Item> newItems)
    {
        items.AddRange(newItems);
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

    public void Attack(CharacterStats unit, ActionDisplay actionDisplay)
    {
        // Determine attack hit
        System.Random random = new System.Random();
        double hit = GetNegativeHit(currStats["precision"], unit.GetStats()[7], random.NextDouble());

        // Calculate damage
        int damage = currStats["strength"];

        // Crit
        if (hit >= 2)
            damage = (int)Math.Floor((float)damage * CRIT_MULTIPLIER);
        // Miss
        else if (hit < 1)
            damage = -1;

        actionDisplay.SetAttackDisplay(unit, unit.AdjustDamage(damage), hit >= 2, currStats["precision"]);
        unit.TakeDamage(damage);

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
    public void UseAbility(int abilityIndex, HashSet<Tile> tiles, Tile aimedPoint, bool isItems, ActionDisplay actionDisplay)
    {
        if (!isItems)
        {
            abilities[abilityIndex].UseAbility(tiles, aimedPoint, player, this, actionDisplay);
        }
        else
        {
            // Use item and remove it if it is expended
            if (items[abilityIndex].UseItem(tiles, aimedPoint, player, this, actionDisplay))
                items.RemoveAt(abilityIndex);
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
                ActionDisplay.AddDeath(this);
        }
    }
    public int AdjustDamage(int currDamage)
    {
        if (currDamage == -1)
            return currDamage;
        currDamage -= currStats["defense"];
        return currDamage < 0 ? 0 : currDamage;
    }
    virtual public void Die()
    {
        dead = true;
        currTile.ClearUnit(false);
        playerControl.ScanTeam();
        this.gameObject.SetActive(false);
    }

    // For animation display
    public int GetEffectedPotency(string statName, int potency)
    {
        // Adjust numbers for resistance
        if (statName != "health" && statName != "energy" && potency < 0)
        {
            potency += currStats["resistance"];
            // Penalty cannot be positive
            if (potency > 0)
                potency = 0;
        }
        else if (statName == "health")
        {
            // Damage
            if (potency < 0)
            {
                potency += currStats["defense"];
                if (potency > 0)
                    potency = 0;
            }
            // Healing
            else if (potency > 0 && currStats["health"] + potency > baseStats["health"])
                potency = GetHealing(potency);
        }
        return potency;
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
            ActionDisplay.AddDeath(this);
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

    public static double GetPositiveHitChance(int precision)
    {
        return Math.Round(100 + (precision * PRECISION_MULTIPLIER));
    }
    public static double GetNegativeHitChance(int precision, int dexterity)
    {
        return Math.Round(100 * Math.Pow(DEXTERITY_MULTIPLIER, dexterity) + (precision * PRECISION_MULTIPLIER));
    }
    public static double GetPositiveHit(int precision, double randHitModifier)
    {
        return 1 + (precision * 0.01f * PRECISION_MULTIPLIER) + randHitModifier;
    }
    public static double GetNegativeHit(int precision, int dexterity, double randHitModifier)
    {
        return (100 * Math.Pow(DEXTERITY_MULTIPLIER, dexterity) + (precision * PRECISION_MULTIPLIER)) / 100 + randHitModifier;
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
    public bool GetFlying()
    {
        return flying;
    }

    // Used for forecasting
    public int[] GetStats()
    {
        int[] stats = new int[statTargets.Length + 1];
        for (int i = 0; i < stats.Length - 1; i++)
        {
            stats[i] = currStats[statTargets[i]];
        }
        // Pass in precision at the end as well
        stats[stats.Length - 1] = currStats["precision"];
        return stats;
    }
    public int[] GetBaseStats()
    {
        int[] stats = new int[statTargets.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = baseStats[statTargets[i]];
        }
        return stats;
    }
    public int[] GetDurations()
    {
        int[] durations = new int[statTargets.Length];
        for (int i = 0; i < durations.Length; i++)
        {
            durations[i] = statEffectsDurations[statTargets[i]];
        }
        return durations;
    }

    public int[] GetOffsets(Ability usedAbility, bool isCost)
    {
        if (isCost)
            return CollectCostOffsets(usedAbility);
        else
            return CollectOffsets(usedAbility);
    }
    // Used for ability effects
    public int[] CollectOffsets(Ability usedAbility)
    {
        // Stat effects are mitigated by defense and resistance when potency is negative
        Dictionary<string, int> potencies = usedAbility.GetPotencies();
        int[] offsets = new int[statTargets.Length + 1];

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
            for (int i = 1; i < statTargets.Length; i++)
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

        // Flip potencies if biased and target is enemy
        if (usedAbility.IsBiased() && usedAbility.IsPlayer() != player)
        {
            for (int i = 0; i < statTargets.Length; i++)
            {
                offsets[i] *= -1;
            }
        }

        // Pass in the current precision for forecasting purposes
        offsets[10] = usedAbility.precision;

        return offsets;
    }
    // Used for ability cost
    public int[] CollectCostOffsets(Ability usedAbility)
    {
        // Stat effects are mitigated by defense and resistance when potency is negative
        Dictionary<string, int> costPotencies = usedAbility.GetCostPotencies();
        int[] offsets = new int[statTargets.Length + 1];

        // Cost ignores defense
        offsets[0] = costPotencies["health"] > 0 ? offsets[0] = -costPotencies["health"] : offsets[0] = GetHealing(-costPotencies["health"]);

        // Cost ignores resistance
        for (int i = 1; i < statTargets.Length; i++)
        {
            offsets[i] = -costPotencies[statTargets[i]];
        }

        // Cost ignores precision
        offsets[10] = 0;

        return offsets;
    }
}