using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionDisplay : MonoBehaviour
{
    public PlayerControl playerControl;
    public Camera mainCamera;
    public GameObject mainDisplay;
    public GameObject menu;
    public Text actionName;
    public SpriteRenderer characterSprite;
    public Text[] statNums;
    public Text[] durationNums;
    public Text[] missTexts;
    public Image[] statBars;

    const float OPENING_DELAY = 1;
    const float MISS_DELAY = 0.5f;
    const float STAT_TICK_DELAY = 0.15f;
    const float CLOSING_DELAY = 2;
    const float CHARACTER_SWITCH_DELAY = 2;

    IEnumerator displayCoroutine;

    static HashSet<CharacterStats> deadUnits;

    public void Start()
    {
        deadUnits = new HashSet<CharacterStats>();
    }

    public IEnumerator StartAttackDisplay()
    {
        this.gameObject.SetActive(true);
        yield return StartCoroutine(displayCoroutine);
    }

    public void SetAttackDisplay(CharacterStats effectedUnit, int damage, bool isCrit)
    {
        mainCamera.transform.position = new Vector3(effectedUnit.gameObject.transform.position.x, effectedUnit.gameObject.transform.position.y, transform.position.z);
        actionName.text = "Attack";
        InitCharacterDisplay(effectedUnit, effectedUnit.GetStats(), effectedUnit.GetDurations());

        displayCoroutine = DisplayAttack(damage, (float)effectedUnit.GetMaxHealth(), (float)effectedUnit.GetHealth(), isCrit);
    }
    public IEnumerator DisplayAttack(int damage, float maxHealth, float currHealth, bool isCrit)
    {
        // Disable playerControl while display is going
        playerControl.enabled = false;
        mainDisplay.SetActive(true);
        menu.SetActive(false);

        yield return new WaitForSeconds(OPENING_DELAY);
        // Miss
        if (damage == -1)
        {
            missTexts[0].gameObject.SetActive(true);
            missTexts[0].text = "Miss";
            yield return new WaitForSeconds(MISS_DELAY);

        }
        else if (damage == 0)
        {
            missTexts[0].gameObject.SetActive(true);
            missTexts[0].text = "No effect";
            yield return new WaitForSeconds(MISS_DELAY);
        }
        // Hit
        else
        {
            if (isCrit)
            {
                missTexts[0].gameObject.SetActive(true);
                missTexts[0].text = "Critical";
            }
            for (int i = 0; i < damage && statNums[0].text != "0"; i++)
            {
                // Animate health bars and stat num
                currHealth--;
                statBars[0].transform.localScale = new Vector3((currHealth / maxHealth <= 1 ? currHealth / maxHealth : 1), statBars[0].transform.localScale.y, statBars[0].transform.localScale.x);
                statBars[1].transform.localScale = new Vector3((currHealth / maxHealth > 1 ? (currHealth - maxHealth) / maxHealth : 0), statBars[1].transform.localScale.y, statBars[1].transform.localScale.x);
                statNums[0].text = currHealth.ToString();
                yield return new WaitForSeconds(STAT_TICK_DELAY);
            }
        }

        yield return new WaitForSeconds(CLOSING_DELAY);
        ExitDisplay();
    }

    public void SetAbilityDisplay(List<CharacterStats> effectedUnits, List<int[]> preStats, List<int[]> preDurations, List<int[]> potencies, int[] durations, List<bool[]> isCrits, CharacterStats castingUnit, int[] costPotencies, int[] costDurations, string abilityName)
    {
        actionName.text = abilityName;

        displayCoroutine = DisplayAbility(effectedUnits, preStats, preDurations, potencies, durations, isCrits, castingUnit, costPotencies, costDurations, abilityName);
    }
    public IEnumerator DisplayAbility(List<CharacterStats> effectedUnits, List<int[]> preStats, List<int[]> preDurations, List<int[]> potencies, int[] durations, List<bool[]> isCrits, CharacterStats castingUnit, int[] costPotencies, int[] costDurations, string abilityName)
    {
        // Disable playerControl while display is going
        playerControl.enabled = false;
        mainDisplay.SetActive(true);
        menu.SetActive(false);

        float currStatValue;
        int j;
        int k;

        InitCharacterDisplay(effectedUnits[0], preStats[0], preDurations[0]);
        yield return new WaitForSeconds(OPENING_DELAY);
        for (int i = 0; i < effectedUnits.Count; i++)
        {
            InitCharacterDisplay(effectedUnits[i], preStats[i], preDurations[i]);
            mainCamera.transform.position = new Vector3(effectedUnits[i].gameObject.transform.position.x, effectedUnits[i].gameObject.transform.position.y, transform.position.z);
            for (j = 0; j < potencies[i].Length; j++)
            {
                currStatValue = preStats[i][j];
                // Miss
                if (potencies[i][j] == 65535)
                {
                    missTexts[j].gameObject.SetActive(true);
                    missTexts[j].text = "Miss";
                    yield return new WaitForSeconds(MISS_DELAY);
                }
                else if (potencies[i][j] == 0)
                {
                    missTexts[j].gameObject.SetActive(true);
                    missTexts[j].text = "No effect";
                    yield return new WaitForSeconds(MISS_DELAY);
                }
                // Hit
                else if (potencies[i][j] != -65535)
                {
                    if (isCrits[i][j])
                    {
                        missTexts[j].gameObject.SetActive(true);
                        missTexts[j].text = "Critical";
                    }
                    durationNums[j].text = durations[j] == 0 ? "" : durations[j].ToString();
                    for (k = 0; k < System.Math.Abs(potencies[i][j]) && statNums[j].text != "0"; k++)
                    {
                        // Animate health bars and stat num
                        currStatValue += potencies[i][j] > 0 ? 1 : -1;
                        // Change stat bars if health or energy
                        if (j == 0)
                        {
                            statBars[0].transform.localScale = new Vector3((currStatValue / effectedUnits[i].GetMaxHealth() <= 1 ? (float)currStatValue / (float)effectedUnits[i].GetMaxHealth() : 1),
                                statBars[0].transform.localScale.y, statBars[0].transform.localScale.x);
                            statBars[1].transform.localScale = new Vector3(((float)currStatValue / (float)effectedUnits[i].GetMaxHealth() > 1 ? ((float)currStatValue - (float)effectedUnits[i].GetMaxHealth()) / (float)effectedUnits[i].GetMaxHealth() : 0),
                                statBars[1].transform.localScale.y, statBars[1].transform.localScale.x);
                        }
                        else if (j == 1)
                            statBars[2].transform.localScale = new Vector3((currStatValue / effectedUnits[i].GetMaxEnergy()), statBars[2].transform.localScale.y, statBars[2].transform.localScale.x);

                        statNums[j].text = currStatValue.ToString();
                        yield return new WaitForSeconds(STAT_TICK_DELAY);
                    }
                }
            }

            yield return new WaitForSeconds(CHARACTER_SWITCH_DELAY);
        }
        InitCharacterDisplay(castingUnit, castingUnit.GetStats(), castingUnit.GetDurations());
        mainCamera.transform.position = new Vector3(castingUnit.gameObject.transform.position.x, castingUnit.gameObject.transform.position.y, transform.position.z);
        // Cost display
        for (j = 0; j < costPotencies.Length; j++)
        {
            currStatValue = castingUnit.GetStats()[j];
            // Hit
            if (costDurations[j] != 0)
                durationNums[j].text = costDurations[j].ToString();
            for (k = 0; k < System.Math.Abs(costPotencies[j]) && statNums[j].text != "0"; k++)
            {
                // Animate health bars and stat num
                currStatValue += costPotencies[j] < 0 ? 1 : -1;
                // Change stat bars if health or energy
                if (j == 0)
                {
                    statBars[0].transform.localScale = new Vector3((currStatValue / castingUnit.GetMaxHealth() <= 1 ? (float)currStatValue / (float)castingUnit.GetMaxHealth() : 1),
                        statBars[0].transform.localScale.y, statBars[0].transform.localScale.x);
                    statBars[1].transform.localScale = new Vector3(((float)currStatValue / (float)castingUnit.GetMaxHealth() > 1 ? ((float)currStatValue - (float)castingUnit.GetMaxHealth()) / (float)castingUnit.GetMaxHealth() : 0),
                        statBars[1].transform.localScale.y, statBars[1].transform.localScale.x);
                }
                else if (j == 1)
                    statBars[2].transform.localScale = new Vector3((currStatValue / castingUnit.GetMaxEnergy()), statBars[2].transform.localScale.y, statBars[2].transform.localScale.x);

                statNums[j].text = currStatValue.ToString();
                yield return new WaitForSeconds(STAT_TICK_DELAY);
            }
        }

        yield return new WaitForSeconds(CLOSING_DELAY);
        ExitDisplay();
    }

    void InitCharacterDisplay(CharacterStats effectedUnit, int[] stats, int[] durations)
    {
        characterSprite.sprite = effectedUnit.portrait;

        statBars[0].transform.localScale = new Vector3(((float)stats[0] / effectedUnit.GetMaxHealth() <= 1 ? (float)stats[0] / (float)effectedUnit.GetMaxHealth() : 1), statBars[0].transform.localScale.y, statBars[0].transform.localScale.x);
        statBars[1].transform.localScale = new Vector3(((float)stats[0] / (float)effectedUnit.GetMaxHealth() > 1 ? (stats[0] - (float)effectedUnit.GetMaxHealth()) / (float)effectedUnit.GetMaxHealth() : 0),
            statBars[1].transform.localScale.y, statBars[1].transform.localScale.x);
        statBars[2].transform.localScale = new Vector3((effectedUnit.GetEnergy() / effectedUnit.GetMaxEnergy()), statBars[2].transform.localScale.y, statBars[2].transform.localScale.x);
        for (int i = 0; i < stats.Length - 1; i++)
        {
            statNums[i].text = stats[i].ToString();
            // Duration is printed empty if 0 or below
            durationNums[i].text = durations[i] > 0 ? durations[i].ToString() : "";
        }
    }

    public void ExitDisplay()
    {
        // Display deaths
        foreach (CharacterStats unit in deadUnits)
        {
            unit.Die();
        }
        deadUnits.Clear();

        // Close misses
        foreach (Text text in missTexts)
        {
            text.gameObject.SetActive(false);
        }
        // Restore control, if players turn
        if (playerControl.IsPlayerTurn())
            playerControl.enabled = true;
        mainDisplay.SetActive(false);
        menu.SetActive(true);
        // If animation is canceled
        // TODO: currently broken
        if (displayCoroutine != null)
            displayCoroutine = null;
    }
    public static void AddDeath(CharacterStats newUnit)
    {
        deadUnits.Add(newUnit);
    }

    // TODO: Add turn start animation
}
