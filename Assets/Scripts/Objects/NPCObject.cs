using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NPCObject : InteractableObject
{
    private string triggerName = "PrincessTrigger";
    protected override void Init()
    {
        ObjectType = ObjectType.NPC;
        if (PlayerManager.Instance().LocalContext.Dialogues.ContainsKey(triggerName))
        {
            gameObject.SetActive(false);
        }
    }

    public override void OnInteract()
    {
        PlayerManager.Instance().LocalContext.Dialogues[triggerName] = true;
        gameObject.SetActive(false);
    }
}
