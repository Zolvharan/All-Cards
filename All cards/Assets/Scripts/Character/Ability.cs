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

    const float PRECISION_MODIFIER = 0.1f;
    const float DEXTERITY_MODIFIER = 0.1f;
    const float CRIT_MULTIPLIER = 1.5f;

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
    public void UseAbility(HashSet<Tile> tiles, Tile aimedPoint, bool player, CharacterStats caster, ActionDisplay actionDisplay)
    {
        // Data collected for animation display
        List<CharacterStats> effectedUnits = new List<CharacterStats>();
        List<int[]> preStats = new List<int[]>();
        List<int[]> preDurations = new List<int[]>();
        List<int[]> effectivePotencies = new List<int[]>();
        List<bool[]> isCrits = new List<bool[]>();
        List<Vector3> prePositions = new List<Vector3>();
        List<Tile> toPush = new List<Tile>();

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
                // Init display data
                effectedUnits.Add(tile.currUnit);
                preStats.Add(tile.currUnit.GetStats());
                preDurations.Add(tile.currUnit.GetDurations());
                effectivePotencies.Add(new int[10]);
                isCrits.Add(new bool[10]);

                if (!directed && biased && tile.currUnit.player != player)
                    reverseEffect = -1;
                // ignore unit if ability is directed and it is not a target
                else if (directed && ((tile.currUnit.player == player) != ally))
                    continue;
                // determine hit, factor dexterity into equation if effect is negative
                randHitModifier = random.NextDouble();
                posHit = CharacterStats.GetPositiveHit(precision, randHitModifier);
                negHit = CharacterStats.GetNegativeHit(precision, tile.currUnit.GetStats()[7], randHitModifier);

                string target;
                // affect stat of each unit in possible AOE across duration
                for (int i = 0; i < CharacterStats.statTargets.Length; i++)
                {
                    target = CharacterStats.statTargets[i];
                    // Break if unit dies
                    if (tile.currUnit == null)
                        break;
                    // Ignore uneffected stats
                    if (potencies[target] == 0)
                    {
                        effectivePotencies[effectivePotencies.Count - 1][i] = -65535;
                        isCrits[isCrits.Count - 1][i] = false;
                        continue;
                    }
                    if ((potencies[target] > 0 && posHit >= 1) || (potencies[target] < 0 && negHit >= 1))
                    {
                        currHit = potencies[target] > 0 ? posHit : negHit;
                        // Collect display data
                        effectivePotencies[effectivePotencies.Count - 1][i] = effectedUnits[effectedUnits.Count - 1].GetEffectedPotency(target, (int)Math.Floor(potencies[target] * reverseEffect * (currHit >= 2 ? 1.5f : 1)));
                        isCrits[isCrits.Count - 1][i] = currHit >= 2;
                        // Ignore uneffected stats (checks for tile variable in case character died)
                        tile.currUnit.AddStatEffect(target, durations[target], (int)Math.Floor(potencies[target] * reverseEffect * (currHit >= 2 ? CRIT_MULTIPLIER : 1)));    // Ternary tests crit
                    }
                    else if (potencies[target] != 0)
                    {
                        // miss
                        effectivePotencies[effectivePotencies.Count - 1][i] = 65535;
                        isCrits[isCrits.Count - 1][i] = false;
                    }
                }
                // Move unit if ability pushes. Will not push if no push is present, or if ability fails to hit enemy.
                // Always pushes ally
                if (pushInts[0] != 0 || pushInts[1] != 0)
                {
                    // prePositions is empty if not a push ability
                    prePositions.Add(tile.currUnit.transform.position);
                    if ((tile.currUnit.player == usedByPlayer) || negHit >= 1)
                    {
                        toPush.Add(tile);
                    }
                }
                if (negHit < 1)
                {
                    UnityEngine.Debug.Log("Miss");
                }
            }
            reverseEffect = 1;
        }
        // Apply pushes
        int greatestDistance;
        int currDistance;
        Tile currTile;
        Tile nextTile;
        while (toPush.Count > 0)
        {
            greatestDistance = 0;
            currTile = toPush[0];
            foreach (Tile tile in toPush)
            {
                // TODO: Distance must adjust to direction
                currDistance = Math.Abs(Math.Abs(caster.currTile.xPos) - Math.Abs(tile.xPos)) + Math.Abs(Math.Abs(caster.currTile.yPos) - Math.Abs(tile.yPos));
                if (currDistance > greatestDistance)
                {
                    greatestDistance = currDistance;
                    currTile = tile;
                }
            }
            nextTile = CalculateNewPosition(currTile, caster.currTile, aimedPoint, new int[] { pushInts[0], pushInts[1] });
            nextTile.MoveUnit(currTile.currUnit);
            toPush.Remove(currTile);
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
        if (pushInts[2] != 0 || pushInts[3] != 0)
        {
            nextTile = CalculateNewPosition(caster.currTile, caster.currTile, caster.currTile, new int[] { pushInts[2], pushInts[3] });
            nextTile.MoveUnit(caster);
        }

        SetUpAbilityDisplay(effectedUnits, preStats, preDurations, prePositions, effectivePotencies, isCrits, caster, actionDisplay);
    }

    void SetUpAbilityDisplay(List<CharacterStats> effectedUnits, List<int[]> preStats, List<int[]> preDurations, List<Vector3> prePositions,
        List<int[]> effectivePotencies, List<bool[]> isCrits, CharacterStats caster, ActionDisplay actionDisplay)
    {
        int[] reDurations = new int[10];
        int[] reCostPotencies = new int[10];
        int[] reCostDurations = new int[10];
        for (int i = 0; i < 10; i++)
        {
            reDurations[i] = durations[CharacterStats.statTargets[i]];
            reCostPotencies[i] = costPotencies[CharacterStats.statTargets[i]];
            reCostDurations[i] = costDurations[CharacterStats.statTargets[i]];
        }

        actionDisplay.SetAbilityDisplay(effectedUnits, preStats, preDurations, prePositions, effectivePotencies, reDurations, isCrits, caster, reCostPotencies, reCostDurations, abilityName, precision);
    }

    // Take current position, and force, and direction from source and end tiles
    Tile CalculateNewPosition(Tile startTile, Tile sourceTile, Tile endTile, int[] force)
    {
        int greaterForce = force[0] > force[1] ? force[0] : force[1];
        int currXPos = startTile.xPos;
        int currYPos = startTile.yPos;
        float nextXPos = currXPos;
        float nextYPos = currYPos;

        // Calculate direction weight
        float[] direction = new float[] { endTile.xPos - sourceTile.xPos, endTile.yPos - sourceTile.yPos };
        // If cast on self, aim true
        if (direction[0] == 0 && direction[1] == 0)
            direction[1] = 1;

        float xPosTick = (force[0] * direction[1]) + (force[1] * direction[0]);
        float yPosTick = (force[1] * direction[1]) - (force[0] * direction[0]);
        if (Math.Abs(xPosTick) > Math.Abs(yPosTick))
        {
            yPosTick /= Math.Abs(xPosTick);
            xPosTick /= Math.Abs(xPosTick);
        }
        else
        {
            xPosTick /= Math.Abs(yPosTick);
            yPosTick /= Math.Abs(yPosTick);
        }

        for (int i = 0; i < greaterForce; i++)
        {
            nextXPos += xPosTick;
            nextYPos += yPosTick;
            if (!startTile.map.IsWithinBounds((int)Math.Round(nextXPos), (int)Math.Round(nextYPos)) || !startTile.map.tiles[(int)Math.Round(nextXPos)][(int)Math.Round(nextYPos)].IsPassable())
                break;
            currXPos = (int)Math.Round(nextXPos);
            currYPos = (int)Math.Round(nextYPos);
        }

        return startTile.map.tiles[currXPos][currYPos];
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

    // Enemy AI functions
    public int GetEnergyCost()
    {
        return costPotencies["energy"];
    }
    public bool IsHeal()
    {
        return potencies["health"] < 0 || (biased && potencies["health"] != 0);
    }
    public bool IsSupport()
    {
        bool isSupport = false;
        foreach (KeyValuePair<string, int> pair in potencies)
        {
            if ((pair.Value > 0 || (biased && pair.Value != 0)) && pair.Key != "health")
            {
                isSupport = true;
                break;
            }
        }
        return isSupport;
    }
    public bool IsAttack()
    {
        bool isAttack = false;
        foreach (KeyValuePair<string, int> pair in potencies)
        {
            if (pair.Value > 0)
            {
                isAttack = true;
                break;
            }
        }
        return isAttack;
    }
    public bool IsEnergyRegen()
    {
        return costPotencies["energy"] < 0;
    }
}