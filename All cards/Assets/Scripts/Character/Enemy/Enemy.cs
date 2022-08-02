using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterStats
{
    Sprite banner;
    int enemyLevel;
    EnemyControl enemyControl;

    System.Random random;

    const int VARIABLE_PURSUIT_DISTANCE = 3;
    const float MOVE_DELAY = 1;
    const float INJURED_RATIO = 0.7f;

    // Initializes variables
    public void ConstructUnit(string newName, Ability[] newAbilities, Sprite newPortrait, Sprite newBattleSprite, bool isFlying, Dictionary<String, int> newBaseStats, Sprite newBanner, int newEnemyLevel, bool isPlayer)
    {
        // Enemy has no items
        ConstructCharacter(newName, newAbilities, newPortrait, newBattleSprite, isFlying, newBaseStats, null, isPlayer);

        banner = newBanner;
        enemyLevel = newEnemyLevel;

        random = new System.Random();
    }

    public void SetController(EnemyControl newControl)
    {
        enemyControl = newControl;
    }
    public Sprite GetBanner()
    {
        return banner;
    }

    override public void Die()
    {
        dead = true;
        currTile.ClearUnit(false);
        enemyControl.ScanEnemies();
        this.gameObject.SetActive(false);
    }

    // Now slightly less rudimentary AI behavior
    public IEnumerator ExecuteTurn()
    {
        NewTurn();
        if (!dead)
        {
            // if has ability and has energy
            List<Ability> healOptions = new List<Ability>();
            List<Ability> supportOptions = new List<Ability>();
            List<Ability> attackOptions = new List<Ability>();
            List<Ability> regenOptions = new List<Ability>();
            foreach (Ability ability in abilities)
            {
                if (ability.GetEnergyCost() <= GetEnergy())
                {
                    if (ability.IsHeal())
                        healOptions.Add(ability);
                    else if (ability.IsSupport())
                        supportOptions.Add(ability);
                    else if (ability.IsAttack())
                        attackOptions.Add(ability);
                    else if (ability.IsEnergyRegen())
                        regenOptions.Add(ability);
                }
            }
            List<CharacterStats> injuredEnemies = new List<CharacterStats>();
            foreach (Enemy enemy in currTile.map.enemies)
            {
                if (enemy.IsInjured() && TargetInMoveRange(false, healOptions))
                    injuredEnemies.Add(enemy);
            }

            CharacterStats currTarget;
            bool needsToMove;
            Ability selectedAttack;
            if (HasEnergy())
            {
                // if can heal && ally in danger
                if (healOptions.Count > 0 && injuredEnemies.Count > 0)
                {
                    currTarget = injuredEnemies[random.Next(0, injuredEnemies.Count)];
                    selectedAttack = healOptions[random.Next(0, healOptions.Count)];
                    if (MarkedTargetInRange(currTarget, selectedAttack))
                    {
                        yield return StartCoroutine(Move(currTarget));
                        yield return UseEnemyAbility(selectedAttack, false);
                    }
                    else
                    {
                        yield return UseEnemyAbility(selectedAttack, false);
                        Retreat();
                    }
                }
                // if target in range
                else if (TargetInMoveRange(true, attackOptions))
                {
                    selectedAttack = null;
                    // if energy not full and standard attack in range, use standard attack if RNG says so, otherwise use ability
                    // Unless there are no attack options
                    if (attackOptions.Count != 0 || !(currStats["energy"] != baseStats["energy"] && TargetInMoveRange() && random.Next(0, attackOptions.Count + 1) == attackOptions.Count))
                        selectedAttack = attackOptions[random.Next(0, attackOptions.Count)];

                    List<Ability> singleItem;
                    if (selectedAttack != null)
                    {
                        singleItem = new List<Ability>();
                        singleItem.Add(selectedAttack);
                        needsToMove = !TargetInRange(true, singleItem);
                    }
                    // Standard attack
                    else
                        needsToMove = !TargetInRange();

                    if (needsToMove)
                        yield return StartCoroutine(Move());
                    if (selectedAttack == null)
                        yield return StandardAttack();
                    else
                        yield return UseEnemyAbility(selectedAttack, true);
                    if (!needsToMove)
                        Retreat();
                }
                // if has energy for support
                else if (supportOptions.Count > 0)
                {
                    yield return UseEnemyAbility(supportOptions[random.Next(0, supportOptions.Count)], false);
                    yield return StartCoroutine(Move());
                }
                // if can regenerate energy
                else if (regenOptions.Count > 0)
                {
                    yield return StartCoroutine(Move());
                    yield return UseEnemyAbility(regenOptions[random.Next(0, regenOptions.Count)], false);
                }
            }
            // if target in range
            else if (TargetInMoveRange())
            {
                if (TargetInRange())
                {
                    yield return StandardAttack();
                    Retreat();
                }
                else
                {
                    yield return StartCoroutine(Move());
                    yield return StandardAttack();
                }
            }
            // if can regenerate energy
            else if (regenOptions.Count > 0)
            {
                yield return StartCoroutine(Move());
                yield return UseEnemyAbility(regenOptions[random.Next(0, regenOptions.Count)], false);
            }
            // Move
            else
            {
                yield return StartCoroutine(Move());
            }
        }
    }

    // Move towards random player if null, random player is player within a certain distance of closest player
    // If not null, move towards target
    IEnumerator Move(CharacterStats target = null)
    {
        HashSet<Tile> tilesInRange = currTile.GetTiles(currTile, currStats["moveSpeed"], false);
        Tile closestTile;
        int currDistance = -1;
        int tileDistance;

        if (target == null)
        {
            // Get closest unit
            foreach (CharacterStats unit in currTile.map.characters)
            {
                if (!unit.dead)
                {
                    tileDistance = Math.Abs(unit.currTile.xPos - currTile.xPos) + Math.Abs(unit.currTile.yPos - currTile.yPos);
                    if (target == null || tileDistance < currDistance)
                    {
                        target = unit;
                        currDistance = tileDistance;
                    }
                }
            }
            // TODO: VARIABLE_PURSUIT_DISTANCE
        }
        if (target != null)
        {
            currDistance = GetDistance(target.currTile);
            closestTile = this.currTile;
            foreach (Tile tile in tilesInRange)
            {
                tileDistance = Math.Abs(tile.xPos - target.currTile.xPos) + Math.Abs(tile.yPos - target.currTile.yPos);
                if (tileDistance < currDistance && !tile.occupied)
                {
                    closestTile = tile;
                    currDistance = tileDistance;
                }
            }
            enemyControl.SetCamera();
            yield return new WaitForSeconds(MOVE_DELAY);
            currTile.ClearUnit(false);
            closestTile.PlaceUnit(this);
            enemyControl.SetCamera();
            yield return new WaitForSeconds(MOVE_DELAY);
        }
    }
    // Filler move
    void Retreat()
    {
        // TODO
    }

    IEnumerator StandardAttack()
    {
        List<CharacterStats> targetsInRange = GetTargetsInRange(GetAttackRange(), true);
        Attack(targetsInRange[random.Next(0, targetsInRange.Count)], enemyControl.GetActionDisplay());
        yield return enemyControl.GetActionDisplay().StartAttackDisplay();
    }
    IEnumerator UseEnemyAbility(Ability ability, bool targetFoe)
    {
        List<CharacterStats> targets = GetTargetsInRange(ability.range, targetFoe);
        CharacterStats target = targets[random.Next(0, targets.Count)];

        HashSet<Tile> targetedTiles = target.currTile.CollectTiles(ability.radius);
        ability.UseAbility(targetedTiles, player, this, enemyControl.GetActionDisplay());
        yield return enemyControl.GetActionDisplay().StartAttackDisplay();
    }
    bool HasEnergy()
    {
        bool hasEnergy = false;
        foreach (Ability ability in abilities)
        {
            if (ability.GetEnergyCost() <= GetEnergy())
                hasEnergy = true;
        }

        return hasEnergy;
    }

    // ifMove is for detecting range with moving
    bool TargetInMoveRange(bool targetPlayer = true, List<Ability> potentialAbilities = null)
    {
        HashSet<Tile> tileSet = currTile.GetTiles(currTile, GetMoveSpeed(), flying);
        foreach (Tile tile in tileSet)
        {
            if (!tile.occupied && TargetInRange(targetPlayer, potentialAbilities, tile))
                return true;
        }
        return false;
    }
    // Tile used by TargetInMoveRange
    bool TargetInRange(bool targetPlayer = true, List<Ability> potentialAbilities = null, Tile tile = null)
    {
        int longestRange = GetAttackRange();
        if (potentialAbilities != null)
        {
            foreach (Ability ability in potentialAbilities)
            {
                if (longestRange < ability.range)
                    longestRange = ability.range;
            }
        }
        return GetTargetsInRange(longestRange, targetPlayer, tile).Count > 0;
    }
    bool MarkedTargetInRange(CharacterStats targetPlayer, Ability selectedAbility)
    {
        // If ability is null, use attack
        int range = selectedAbility == null ? GetAttackRange() : selectedAbility.range;
        return GetDistance(targetPlayer.currTile) <= range;
    }
    List<CharacterStats> GetTargetsInRange(int range, bool targetPlayer, Tile tile = null)
    {
        List<CharacterStats> allTargets;
        if (targetPlayer)
            allTargets = currTile.map.characters;
        else
        {
            allTargets = new List<CharacterStats>();
            foreach (Enemy enemy in currTile.map.enemies)
            {
                allTargets.Add(enemy);
            }
        }

        List<CharacterStats> targetsInRange = new List<CharacterStats>();
        foreach (CharacterStats character in allTargets)
        {
            if (GetDistance(character.currTile, tile) <= range)
                targetsInRange.Add(character);
        }
        return targetsInRange;
    }

    int GetDistance(Tile destinationTile, Tile sourceTile = null)
    {
        if (sourceTile == null)
            sourceTile = currTile;
        return Math.Abs(destinationTile.xPos - sourceTile.xPos) + Math.Abs(destinationTile.yPos - sourceTile.yPos);
    }

    bool IsInjured()
    {
        return GetHealth() < GetMaxHealth() * INJURED_RATIO;
    }
}
