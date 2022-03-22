using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generally manages the main menu
public class MMManager : MonoBehaviour
{
    public GameObject BattleUI;

    public GameObject frontPage;
    public BattleSetupUI customBattle;
    public CCManager characters;
    public MCManager charForm;
    public SCManager characterSaves;
    public GameObject abilityForm;
    public SAManager abilitySaves;
    public CIManager imageSelection;
    public MIManager itemForm;

    public void OpenFrontPage()
    {
        frontPage.SetActive(true);
        customBattle.gameObject.SetActive(false);
        characters.gameObject.SetActive(false);
    }

    public void OpenCustomBattle()
    {
        customBattle.gameObject.SetActive(true);
        customBattle.InitDisplay();
        frontPage.SetActive(false);
    }
    // Resets menu, opens battleUI and hides
    public void StartCombat()
    {
        customBattle.gameObject.SetActive(false);
        frontPage.SetActive(true);
        BattleUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OpenCharacters()
    {
        characters.gameObject.SetActive(true);
        characters.InitDisplay();
        frontPage.SetActive(false);
        charForm.gameObject.SetActive(false);
        abilityForm.SetActive(false);
        abilitySaves.gameObject.SetActive(false);
        itemForm.gameObject.SetActive(false);
    }

    // Primary initialization is done in MCManager
    public void DisplayCharForm()
    {
        charForm.gameObject.SetActive(true);
        charForm.RefreshAbilities();
        characters.gameObject.SetActive(false);
        abilityForm.SetActive(false);
        imageSelection.gameObject.SetActive(false);
    }
    public void DisplaySavedCharacterData(CharacterData data, int isEditingSaveIndex = -1)
    {
        charForm.gameObject.SetActive(false);
        characterSaves.gameObject.SetActive(true);
        characterSaves.InitDisplay(data, isEditingSaveIndex);
    }

    // Primary initialization is done in MAManager
    public void DisplayAbilityForm()
    {
        abilityForm.SetActive(true);
        charForm.gameObject.SetActive(false);
        abilitySaves.gameObject.SetActive(false);
    }
    public void DisplaySavedAbilityData(AbilityData data, bool isInChar, int isEditingSaveIndex = -1)
    {
        abilityForm.SetActive(false);
        characters.gameObject.SetActive(false);
        abilitySaves.gameObject.SetActive(true);
        abilitySaves.InitDisplay(data, isInChar, isEditingSaveIndex);
    }

    public void OpenCharImages(bool isPortrait)
    {
        charForm.gameObject.SetActive(false);
        imageSelection.gameObject.SetActive(true);
        imageSelection.InitDisplay(isPortrait);
    }

    public void OpenItemForm()
    {
        itemForm.gameObject.SetActive(true);
        characters.gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}