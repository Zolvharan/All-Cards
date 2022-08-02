using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public AbilityData[] abilities;
    public byte[] portrait;
    public byte[] battleSprite;
    public bool flying;
    public int[] stats;

    public CharacterData(string newName, AbilityData[] newAbilities, byte[] newPortrait, byte[] newBattleSprite, int[] statNums, bool isFlying)
    {
        characterName = newName;
        abilities = new AbilityData[newAbilities.Length];
        newAbilities.CopyTo(abilities, 0);
        portrait = new byte[newPortrait.Length];
        newPortrait.CopyTo(portrait, 0);
        battleSprite = new byte[newBattleSprite.Length];
        newBattleSprite.CopyTo(battleSprite, 0);
        stats = new int[8];
        statNums.CopyTo(stats, 0);
        flying = isFlying;
    }

    // Takes basePrefab as input, and initializes character
    public void ConstructCharacter(CharacterStats basePrefab, List<Item> newItems, bool isPlayer)
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
        // TODO: init abilities
        Ability[] newAbilities = new Ability[abilities.Length];
        for (int i = 0; i < abilities.Length; i++)
        {
            newAbilities[i] = abilities[i].ConstructAbility(isPlayer);
        }

        basePrefab.ConstructCharacter(characterName, newAbilities, newPortrait, newBattleSprite, flying, baseStats, newItems, isPlayer);
    }

    public string GetName()
    {
        return characterName;
    }
}
