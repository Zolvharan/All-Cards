using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionFormManager : CreationForm
{
    public EnemyGenManager enemyGenManager;
    public CharacterImageForm imageSelectionManager;

    public InputField nameField;
    public Dropdown unitList;
    public List<UnitData> currUnits;
    public Image banner;
    public byte[] bannerData;
    const string BANNER_PATH = ".\\SavedData\\Banners";

    public Text alertText;
    bool removingUnit;

    bool editing;

    public void InitDisplay(bool isEditing)
    {
        bannerData = CharacterImageForm.GetTheEmpty();
        banner.sprite = CharacterImageForm.ConstructImage(bannerData);

        editing = isEditing;
        if (isEditing)
        {
            // Get faction selected
            FactionData currFaction = SaveData.GetFactions()[enemyGenManager.lists[0].value];
            nameField.text = currFaction.GetName();
            currUnits = new List<UnitData>();
            // Set unit dropdown
            for (int i = 0; i < currFaction.units.Length; i++)
            {
                currUnits.Add(currFaction.units[i]);
            }
            bannerData = currFaction.GetBanner();
            banner.sprite = CharacterImageForm.ConstructImage(bannerData);
        }
        else
        {
            nameField.text = "";
            currUnits = new List<UnitData>();
        }
        RefreshUnits();

        alertText.text = "";
    }
    public void ExitFactionForm()
    {
        enemyGenManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OpenImages()
    {
        imageSelectionManager.gameObject.SetActive(true);
        imageSelectionManager.InitDisplay(BANNER_PATH, this);
        this.gameObject.SetActive(false);
    }
    // Sets portrait or battleSprite if saving, and closes image selection
    public override void SetImage(bool saveChanges, Sprite newSprite, byte[] newData)
    {
        this.gameObject.SetActive(true);

        if (saveChanges)
        {
            banner.sprite = newSprite;
            bannerData = new byte[newData.Length];
            newData.CopyTo(bannerData, 0);
        }
    }
    public byte[] GetBanner()
    {
        return bannerData;
    }

    public void OpenSaves()
    {
        enemyGenManager.OpenSavedFactionData(BuildFaction(), editing ? enemyGenManager.lists[0].value : -1);
    }
    // Takes form data and builds faction
    public FactionData BuildFaction()
    {
        // Turn unit list into array
        foreach (UnitData unit in currUnits)
        {
            unit.SetBanner(bannerData);
        }
        UnitData[] unitsToSave = new UnitData[currUnits.Count];
        currUnits.CopyTo(unitsToSave);
        FactionData newFaction = new FactionData(nameField.text, bannerData, unitsToSave);

        return newFaction;
    }

    // Sets unit list
    public void RefreshUnits()
    {
        unitList.ClearOptions();
        List<string> currUnitNames = new List<string>();
        for (int i = 0; i < currUnits.Count; i++)
        {
            currUnitNames.Add(currUnits[i].GetName());
        }
        unitList.AddOptions(currUnitNames);
    }

    public void RemoveUnit()
    {
        // Ignore if no units exist
        if (currUnits.Count > 0)
        {
            // User has to press button twice to remove
            if (removingUnit == false)
            {
                removingUnit = true;
                alertText.text = "Remove " + currUnits[unitList.value].GetName() + "?";
            }
            else
            {
                removingUnit = false;
                alertText.text = "";

                currUnits.RemoveAt(unitList.value);
                List<string> currUnitNames = new List<string>();
                for (int i = 0; i < currUnits.Count; i++)
                {
                    currUnitNames.Add(currUnits[i].GetName());
                }
                unitList.ClearOptions();
                unitList.AddOptions(currUnitNames);
            }
        }
    }
}
