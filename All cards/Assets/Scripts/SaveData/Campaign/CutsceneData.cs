using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneData
{
    public byte[] backdropData;
    public CharacterStats[] participatingCharacters;
    public string[] dialogueLines;
    public int[] speakingCharacterIndexes;

    // TODO: Branching dialogue
    public int[] dialogueChoicePoints;
    public DialogueChoiceData[] dialogueChoices;

    public CutsceneData(byte[] newData, CharacterStats[] newCharacters, string[] newLines, int[] newIndexes, int[] newPoints, DialogueChoiceData[] newChoices)
    {
        backdropData = new byte[newData.Length];
        newData.CopyTo(backdropData, 0);
        participatingCharacters = new CharacterStats[newCharacters.Length];
        newCharacters.CopyTo(participatingCharacters, 0);
        dialogueLines = new string[newLines.Length];
        newLines.CopyTo(dialogueLines, 0);
        speakingCharacterIndexes = new int[newIndexes.Length];
        newIndexes.CopyTo(speakingCharacterIndexes, 0);
        dialogueChoicePoints = new int[newPoints.Length];
        newPoints.CopyTo(dialogueChoicePoints, 0);
        dialogueChoices = new DialogueChoiceData[newChoices.Length];
        newChoices.CopyTo(dialogueChoices, 0);
    }
}
