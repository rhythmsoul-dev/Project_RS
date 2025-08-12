using DG.Tweening;
using TMPro;
using UnityEngine;

public class NPCDialogueUIController : DialogueControllerBase
{
    [SerializeField] private TMP_Text npcText;
    
    protected override void SetUp()
    {
        base.SetUp();
        
        Show();
    }
    protected override void DisplayEntry(DialogueText entry)
    {
        if (entry.speaker == "NPC")
        {
            prevText = entry.text;
            conversationUI.SetActive(true);
            npcText.DOKill();
            npcText.SetText("");
            DialogueTween = npcText.DOText(entry.text, 1).SetUpdate(true);
        }
    }
    protected override void DisplayEntry(string entryText)
    {
        npcText.SetText(entryText);
    }
}
