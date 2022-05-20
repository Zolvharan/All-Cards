using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionFormManager : MonoBehaviour
{
    public EnemyGenManager enemyGenManager;
    public CIManager imageSelectionManager;

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
        bannerData = CIManager.GetTheEmpty();
        banner.sprite = CharacterData.ConstructImage(bannerData);

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
            banner.sprite = CharacterData.ConstructImage(bannerData);
        }
        else
        {
            nameField.text = "";
            currUnits = new List<UnitData>();
        }
        RefreshUnits();

        alertText.text = "";
    }

    public void OpenImages()
    {
        imageSelectionManager.gameObject.SetActive(true);
        imageSelectionManager.InitDisplay(BANNER_PATH);
        this.gameObject.SetActive(false);
    }
    // Sets portrait or battleSprite if saving, and closes image selection
    public void SetImage(bool saveChanges)
    {
        this.gameObject.SetActive(true);
        Sprite newImage = imageSelectionManager.GetCurrImage();
        byte[] newData = imageSelectionManager.GetCurrData();
        imageSelectionManager.ExitImages();

        if (saveChanges)
        {
            banner.sprite = newImage;
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
        // Turn ability list into array
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
