using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharPicker : MonoBehaviour
{
    // Used by dropdown boxes to select party members
    static CharacterStats[] selectedParty = new CharacterStats[6];
    // Determines characters position in party
    public int pickerID;
    
    public void SetCharacter(CharacterStats character)
    {
        selectedParty[pickerID] = character;
    }
}
