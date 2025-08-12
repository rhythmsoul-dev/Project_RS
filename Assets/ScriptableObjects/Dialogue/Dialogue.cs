using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string DialogueId;
    public int Priority;
    public List<DialogueText> DialogueSequence = new List<DialogueText>();
}
