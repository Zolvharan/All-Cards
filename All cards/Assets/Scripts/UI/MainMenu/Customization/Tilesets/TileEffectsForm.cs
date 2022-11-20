using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileEffectsForm : MonoBehaviour
{
    public TileFormManager tileForm;
    public AbilityForm abilityForm;

    public Text[] statTextNums;
    public Button customizeButton;
    public Toggle abilityToggle;

    int[] preRawNums;
    int[] prePercentNums;

    int[] rawStatNums;
    int[] percentStatNums;
    AbilityData tileAbility;

    public Text rawPercentText;
    bool isRaw;
    const int PERCENTAGE_STEP = 10;

    protected readonly int[] MIN_STAT_NUMS = { -5, -5, -10, -10, -10, -5, -10, -5, -5, -5};
    protected readonly int[] MAX_STAT_NUMS = { 5, 5, 10, 10, 10, 5, 10, 5, 5, 5};
    const int MAX_PERCENTAGE = 40;
    const int MIN_PERCENTAGE = 0;

    public void InitForm(TileData currTile)
    {
        rawStatNums = new int[10];
        percentStatNums = new int[8];
        // Init percents to default
        for (int i = 0; i < percentStatNums.Length; i++)
        {
            percentStatNums[i] = PERCENTAGE_STEP;
        }
        if (currTile != null)
        {
            currTile.GetRawEffects().CopyTo(rawStatNums, 0);
            currTile.GetPercentageEffects().CopyTo(percentStatNums, 0);
        }
        abilityToggle.isOn = currTile == null ? false : currTile.HasAbility();
        tileAbility = currTile == null ? null : currTile.GetAbility();
        isRaw = true;
    }
    public void OpenTileEffects()
    {
        this.gameObject.SetActive(true);
        preRawNums = new int[10];
        rawStatNums.CopyTo(preRawNums, 0);
        prePercentNums = new int[8];
        percentStatNums.CopyTo(prePercentNums, 0);
        SetDisplayNums();
    }
    public void OpenTileForm(bool saveChanges)
    {
        // Revert
        if (!saveChanges)
        {
            rawStatNums = preRawNums;
            percentStatNums = prePercentNums;
        }

        tileForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    void SetDisplayNums()
    {
        for (int i = 0; i < statTextNums.Length; i++)
        {
            if (isRaw || i < 2 || i == 10)
                statTextNums[i].text = rawStatNums[i].ToString();
            else
                statTextNums[i].text = (percentStatNums[i - 2] * PERCENTAGE_STEP).ToString() + "%";
        }
    }

    public void OpenAbilityForm()
    {
        abilityForm.OpenAbilityFormInTile(tileAbility);
    }
    public void SetAbility(AbilityData newAbility)
    {
        tileAbility = newAbility;
    }
    public void AbilityToggle()
    {
        customizeButton.interactable = abilityToggle.isOn;
        if (abilityToggle.isOn)
            tileAbility = new AbilityData("", false, false, false, new int[10], new int[10], new int[10], new int[10], new int[3], new int[4]);
        else
            tileAbility = null;
    }

    public void RawPercentSwitch()
    {
        isRaw = !isRaw;
        rawPercentText.text = isRaw ? "Raw" : "Percentage";
        SetDisplayNums();
    }
    public int[] GetRawEffects()
    {
        return rawStatNums;
    }
    public int[] GetPercentageEffects()
    {
        int[] adjustedPercentages = new int[8];
        percentStatNums.CopyTo(adjustedPercentages, 0);
        for (int i = 0; i < 8; i++)
        {
            adjustedPercentages[i] = adjustedPercentages[i] / PERCENTAGE_STEP;
        }

        return percentStatNums;
    }
    public AbilityData GetAbility()
    {
        return tileAbility;
    }

    // Used by -+ buttons
    public void IncrementStat(int statIndex)
    {
        if (isRaw || statIndex < 2 || statIndex == 10)
        {
            if (rawStatNums[statIndex] < MAX_STAT_NUMS[statIndex])
            {
                rawStatNums[statIndex]++;
                statTextNums[statIndex].text = rawStatNums[statIndex].ToString();
            }
        }
        else
        {
            if (percentStatNums[statIndex - 2] < MAX_PERCENTAGE)
            {
                percentStatNums[statIndex - 2]++;
                statTextNums[statIndex].text = (percentStatNums[statIndex - 2] * PERCENTAGE_STEP).ToString() + "%";
            }
        }
    }
    public void DecrementStat(int statIndex)
    {
        if (isRaw || statIndex < 2 || statIndex == 10)
        {
            if (rawStatNums[statIndex] > MIN_STAT_NUMS[statIndex])
            {
                rawStatNums[statIndex]--;
                statTextNums[statIndex].text = rawStatNums[statIndex].ToString();
            }
        }
        else
        {
            if (percentStatNums[statIndex - 2] > MIN_PERCENTAGE)
            {
                percentStatNums[statIndex - 2]--;
                statTextNums[statIndex].text = (percentStatNums[statIndex - 2] * PERCENTAGE_STEP).ToString() + "%";
            }
        }
    }
}
