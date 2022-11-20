using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveForm : MonoBehaviour
{
    public BattleForm battleForm;
    public Toggle victoryToggle;
    public Toggle defeatToggle;
    public Toggle eventToggle;
    public GameObject eventEdit;
    public Toggle materialToggle;
    public Dropdown materialList;
    public GameObject materialComponents;
    public Toggle battleToggle;
    public Dropdown battleList;
    public GameObject battleComponents;
    public Toggle characterToggle;
    public Dropdown characterList;
    public GameObject characterComponents;

    public Dropdown objectiveTypes;
    public GameObject[] typeComponents;

    public void InitForm(ObjectiveData currObjective)
    {
        if (currObjective != null)
        {

        }
        else
        {
            objectiveTypes.value = 0;
            victoryToggle.isOn = true;
            eventToggle.isOn = false;
            materialToggle.isOn = false;
            materialList.ClearOptions();
            battleToggle.isOn = false;
            battleList.ClearOptions();
            characterToggle.isOn = false;
            characterList.ClearOptions();
        }
    }

    public void ReturnToBattleForm(bool saveChanges)
    {
        if (saveChanges)
        {

        }

        battleForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void TypeUpdate()
    {
        foreach (GameObject group in typeComponents)
        {
            group.SetActive(false);
        }
        typeComponents[objectiveTypes.value].SetActive(true);
    }

    public void HasEvent()
    {
        eventEdit.SetActive(eventToggle.isOn);
    }
    public void UnlocksMaterials()
    {
        materialComponents.SetActive(materialToggle.isOn);
    }
    public void UnlocksBattles()
    {
        battleComponents.SetActive(battleToggle.isOn);
    }
    public void UnlocksCharacters()
    {
        characterComponents.SetActive(characterToggle.isOn);
    }
}
