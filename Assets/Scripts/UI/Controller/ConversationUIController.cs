using DG.Tweening;
using TMPro;
using UnityEngine;

public class ConversationUIController : DialogueControllerBase
{
    [SerializeField] private GameObject npcDialogue;
    [SerializeField] private TMP_Text npcText;
    
    protected override void DisplayEntry(DialogueText entry)
    {
        if (entry.speaker == "NPC")
        {
            prevText = entry.text;
            npcDialogue.SetActive(true);
            npcText.DOKill();
            npcText.SetText("");
            npcText.ForceMeshUpdate();
            DialogueTween = npcText.DOText(entry.text, 1).SetUpdate(true);
        }
    }
    protected override void DisplayEntry(string entryText)
    {
        npcText.SetText(entryText);
    }
}