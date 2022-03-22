using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipFormUI : MonoBehaviour
{
    public BattleSetupUI BSManager;

    public Dropdown allItemsList;
    public Dropdown currItemsList;
    public Button addItemButton;
    List<Item> currItems;
    int currItemsIndex;
    int MAX_ITEMS = 4;

    public void InitDisplay(int newItemsIndex)
    {
        currItems = BSManager.GetItems(newItemsIndex);
        currItemsIndex = newItemsIndex;

        List<string> itemNames = new List<string>();
        foreach (ItemData item in SaveData.GetItems())
        {
            itemNames.Add(item.GetName());
        }
        allItemsList.ClearOptions();
        allItemsList.AddOptions(itemNames);

        SetCurrItemsListNames();
    }

    public void AddItem()
    {
        currItems.Add(SaveData.GetItems()[allItemsList.value].ConstructItem());
        SetCurrItemsListNames();

        // Lock add if items have reached max
        if (currItems.Count >= MAX_ITEMS)
            addItemButton.interactable = false;
    }
    public void RemoveItem()
    {
        // Ignore if no items exist
        if (currItems.Count > 0)
        {
            currItems.RemoveAt(currItemsList.value);
            SetCurrItemsListNames();
        }

        // Unlock add if locked
        if (addItemButton.interactable == false)
            addItemButton.interactable = true;
    }
    void SetCurrItemsListNames()
    {
        List<string> currItemNames = new List<string>();
        for (int i = 0; i < currItems.Count; i++)
        {
            currItemNames.Add(currItems[i].abilityName);
        }
        currItemsList.ClearOptions();
        currItemsList.AddOptions(currItemNames);
    }

    public void Cancel()
    {
        BSManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void Apply()
    {
        BSManager.gameObject.SetActive(true);
        BSManager.SetItems(currItems, currItemsIndex);
        this.gameObject.SetActive(false);
    }
}
