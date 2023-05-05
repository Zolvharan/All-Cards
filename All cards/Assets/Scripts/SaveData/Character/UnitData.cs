using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public string unitName;
    public AbilityData[] abilities;
    public byte[] portrait;
    public byte[] battleSprite;
    public bool flying;
    public int[] stats;
    public byte[] banner;
    public int enemyLevel;
    // TODO: AIType?

    public UnitData(string newName, AbilityData[] newAbilities, byte[] newPortrait, byte[] newBattleSprite, int[] statNums, bool isFlying, int newEnemyLevel)
    {
        unitName = newName;
        abilities = new AbilityData[newAbilities.Length];
        newAbilities.CopyTo(abilities, 0);
        portrait = new byte[newPortrait.Length];
        newPortrait.CopyTo(portrait, 0);
        battleSprite = new byte[newBattleSprite.Length];
        newBattleSprite.CopyTo(battleSprite, 0);
        stats = new int[8];
        statNums.CopyTo(stats, 0);
        flying = isFlying;
        enemyLevel = newEnemyLevel;
    }

    // Takes basePrefab as input, and initializes unit
    public void ConstructUnit(Enemy basePrefab, bool isPlayer)
    {
        Dictionary<string, int> baseStats = new Dictionary<string, int>();
        baseStats["health"] = 20;
        baseStats["energy"] = 100;
        baseStats["moveSpeed"] = stats[0];
        baseStats["attackRange"] = stats[1];
        baseStats["strength"] = stats[2];
        baseStats["energyRegen"] = stats[3];
        baseStats["precision"] = stats[4];
        baseStats["dexterity"] = stats[5];
        baseStats["defense"] = stats[6];
        baseStats["resistance"] = stats[7];

        Sprite newPortrait = CharacterImageForm.ConstructImage(portrait);
        Sprite newBattleSprite = CharacterImageForm.ConstructImage(battleSprite);
        Sprite newBanner = CharacterImageForm.ConstructImage(banner);
        // TODO: init abilities
        Ability[] newAbilities = new Ability[abilities.Length];
        for (int i = 0; i < abilities.Length; i++)
        {
            newAbilities[i] = abilities[i].ConstructAbility(isPlayer);
        }

        basePrefab.ConstructUnit(unitName, newAbilities, newPortrait, newBattleSprite, flying, baseStats, newBanner, enemyLevel, isPlayer);
    }

    public void SetBanner(byte[] newBanner)
    {
        banner = new byte[newBanner.Length];
        newBanner.CopyTo(banner, 0);
    }
    public string GetName()
    {
        return unitName;
    }
    public int GetEnemyLevel()
    {
        return enemyLevel;
    }
}
