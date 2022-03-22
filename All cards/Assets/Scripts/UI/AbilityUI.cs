using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public Transform UI;
    public GameObject[] buttons;

    bool menuOpen;

    // Start is called before the first frame update
    void Start()
    {
        menuOpen = false;
    }

    public void ToggleMenu(CharacterStats unit, Tile currTile, bool items = false)
    {
        if (!menuOpen)
        {
            OpenMenu(unit, currTile, items);
            menuOpen = true;
        }
        else
        {
            CloseMenu();
            menuOpen = false;
        }
    }
    // Takes an array of unit abilities and displays menu
    void OpenMenu(CharacterStats unit, Tile currTile, bool items = false)
    {
        if (!items)
        {
            for (int i = 0; i < unit.abilities.Length; i++)
            {
                // change button name to ability name and gives button references
                buttons[i].GetComponent<AbilityButton>().SetButton(unit.abilities[i].abilityName, unit.abilities[i].costPotencies["energy"].ToString() + " energy", unit, currTile, i, false);
                buttons[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < unit.items.Count; i++)
            {
                buttons[i].GetComponent<AbilityButton>().SetButton(unit.items[i].abilityName, unit.items[i].numUses.ToString() + " uses", unit, currTile, i, true);
                buttons[i].SetActive(true);
            }
        }
    }
    // Closes menu
    void CloseMenu()
    {
        foreach (GameObject button in buttons)
            button.SetActive(false);
    }
}
