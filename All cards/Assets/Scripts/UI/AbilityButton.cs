using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public PlayerControl controller;

    public Text abilityName;
    public Text cost;
    CharacterStats attachedUnit;
    Tile currTile;
    int index;
    int range;
    int radius;
    bool isItems;

    public void SetButton(string newName, string newCost, CharacterStats newUnit, Tile newTile, int newIndex, bool items)
    {
        abilityName.text = newName;
        cost.text = newCost;
        attachedUnit = newUnit;
        currTile = newTile;
        index = newIndex;
        isItems = items;

        if (!items)
        {
            range = attachedUnit.abilities[index].range;
            radius = attachedUnit.abilities[index].radius;
        }
        else
        {
            range = attachedUnit.items[index].range;
            radius = attachedUnit.items[index].radius;
        }
    }

    public void UseAbility()
    {
        if (isItems || attachedUnit.HasMana(index))
            controller.Casting(index, range, radius, isItems);
    }
}
