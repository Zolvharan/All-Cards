using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChoiceData
{
    public string[] choices;

    public DialogueChoiceData(string[] newChoices)
    {
        choices = new string[newChoices.Length];
        newChoices.CopyTo(choices, 0);
    }
}
