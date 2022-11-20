using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignComponents : MonoBehaviour
{
    public CampaignForm campaignForm;

    public Dropdown allBattles;
    public Dropdown allCharacters;
    public Dropdown allEnemyFactions;
    public Dropdown startingBattlesList;
    public Dropdown charactersList;
    public Dropdown enemyFactionsList;

    List<BattleData> currStartingBattles;
    List<CharacterData> currCharacters;
    List<FactionData> currFactions;

    public void InitForm()
    {
        currStartingBattles = new List<BattleData>();
        currCharacters = new List<CharacterData>();
        currFactions = new List<FactionData>();

        startingBattlesList.ClearOptions();
        charactersList.ClearOptions();
        enemyFactionsList.ClearOptions();

        List<string> newOptions = new List<string>();
        newOptions.Clear();
        allCharacters.ClearOptions();
        foreach (CharacterData characterData in SaveData.GetCharacters())
        {
            newOptions.Add(characterData.characterName);
        }
        allCharacters.AddOptions(newOptions);
        allCharacters.value = 0;

        newOptions.Clear();
        allEnemyFactions.ClearOptions();
        foreach (FactionData factionData in SaveData.GetFactions())
        {
            newOptions.Add(factionData.factionName);
        }
        allEnemyFactions.AddOptions(newOptions);
        allEnemyFactions.value = 0;
    }
    public void OpenForm()
    {
        List<string> newOptions = new List<string>();
        allBattles.ClearOptions();
        foreach (BattleData battleData in campaignForm.GetBattles())
        {
            newOptions.Add(battleData.battleName);
        }
        allBattles.AddOptions(newOptions);
        allBattles.value = 0;
    }

    public void ExitForm()
    {
        campaignForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void AddComponent(int dropdownIndex)
    {
        switch (dropdownIndex)
        {
            case 0:
                if (campaignForm.GetBattles().Count > 0)
                    currStartingBattles.Add(campaignForm.GetBattles()[allBattles.value]);
                RefreshBattles();
                break;
            case 1:
                if (SaveData.GetCharacters().Count > 0)
                    currCharacters.Add(SaveData.GetCharacters()[allCharacters.value]);
                RefreshCharacters();
                break;
            case 2:
                if (SaveData.GetFactions().Count > 0)
                    currFactions.Add(SaveData.GetFactions()[allEnemyFactions.value]);
                RefreshFactions();
                break;
        }
    }
    public void RemoveComponent(int dropdownIndex)
    {
        switch (dropdownIndex)
        {
            case 0:
                if (startingBattlesList.options.Count > 0)
                    currStartingBattles.RemoveAt(startingBattlesList.value);
                RefreshBattles();
                break;
            case 1:
                if (charactersList.options.Count > 0)
                    currCharacters.RemoveAt(charactersList.value);
                RefreshCharacters();
                break;
            case 2:
                if (charactersList.options.Count > 0)
                    currFactions.RemoveAt(enemyFactionsList.value);
                RefreshFactions();
                break;
        }
    }

    public void RefreshBattles()
    {
        List<string> names = new List<string>();
        foreach (BattleData battle in currStartingBattles)
        {
            names.Add(battle.GetName());
        }
        startingBattlesList.ClearOptions();
        startingBattlesList.AddOptions(names);
    }
    public void RefreshCharacters()
    {
        List<string> names = new List<string>();
        foreach (CharacterData character in currCharacters)
        {
            names.Add(character.GetName());
        }
        charactersList.ClearOptions();
        charactersList.AddOptions(names);
    }
    public void RefreshFactions()
    {
        List<string> names = new List<string>();
        foreach (FactionData faction in currFactions)
        {
            names.Add(faction.GetName());
        }
        enemyFactionsList.ClearOptions();
        enemyFactionsList.AddOptions(names);
    }

    public List<BattleData> GetStartingBattles()
    {
        return currStartingBattles;
    }
    public List<CharacterData> GetCharacters()
    {
        return currCharacters;
    }
    public List<FactionData> GetEnemyFactions()
    {
        return currFactions;
    }
    public List<Dropdown.OptionData> GetFactionOptions()
    {
        return enemyFactionsList.options;
    }
}
