using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    // Dialogue
    public CharacterData[] participatingCharacters;
    public string[] dialogueLines;
    public int[] speakingCharacterIndexes;

    // Add new units to field
    public CharacterData[] addedCharacters;
    public int[] characterXPositions;
    public int[] characterYPositions;

    // Change tiles
    public TileData[] changedTiles;
    public int[] tileXPositions;
    public int[] tileYPositions;

    public EventData()
    {
        participatingCharacters = null;
        dialogueLines = null;
        speakingCharacterIndexes = null;
        addedCharacters = null;
        characterXPositions = null;
        characterYPositions = null;
        changedTiles = null;
        tileXPositions = null;
        tileYPositions = null;
    }

    public void AddDialogue(CharacterData[] newCharacters, string[] newLines, int[] newIndexes)
    {
        participatingCharacters = new CharacterData[newCharacters.Length];
        newCharacters.CopyTo(participatingCharacters, 0);
        dialogueLines = new string[newLines.Length];
        newLines.CopyTo(dialogueLines, 0);
        speakingCharacterIndexes = new int[newIndexes.Length];
        newIndexes.CopyTo(speakingCharacterIndexes, 0);
    }
    public void AddCharacters(CharacterData[] newCharacters, int[] newXPositions, int[] newYPositions)
    {
        addedCharacters = new CharacterData[newCharacters.Length];
        newCharacters.CopyTo(addedCharacters, 0);
        characterXPositions = new int[newXPositions.Length];
        newXPositions.CopyTo(characterXPositions, 0);
        characterYPositions = new int[newYPositions.Length];
        newYPositions.CopyTo(characterYPositions, 0);
    }
    public void AddTiles(TileData[] newTiles, int[] newXPositions, int[] newYPositions)
    {
        changedTiles = new TileData[newTiles.Length];
        newTiles.CopyTo(changedTiles, 0);
        tileXPositions = new int[newXPositions.Length];
        newXPositions.CopyTo(tileXPositions, 0);
        tileYPositions = new int[newYPositions.Length];
        newYPositions.CopyTo(tileYPositions, 0);
    }
}
