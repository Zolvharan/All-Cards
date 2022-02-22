using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Ability
{
    // Procedurally generated item data
    // Created from ability and effectively a limited use free ability

    public int numUses;

    public bool UseItem(HashSet<Tile> tiles, bool player)
    {
        UseAbility(tiles, player);
        numUses--;
        if (numUses == 0)
            return true;
        else
            return false;
    }
}
