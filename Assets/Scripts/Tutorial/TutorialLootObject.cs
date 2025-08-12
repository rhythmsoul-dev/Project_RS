using System;
using System.Collections;
using UnityEngine;

public class TutorialLootObject : LootObject
{
    [SerializeField] private PlayerDetector outLineDetector;
    [SerializeField] private PlayerDetector tutorialDetector;

    protected override void Init()
    {
        base.Init();
        outLineDetector.Detected.Changed = OutLineDetect;
        tutorialDetector.Detected.Changed = InteractionTutorial;
    }

    private void OutLineDetect(bool detect)
    {
        if (Interacted)
        {
            return;
        }
        
        if (detect)
        {
            ShowOutline();
        }
        else
        {
            HideOutline();
        }
    }

    public override void OnInteract()
    {
        if (Interacted)
        {
            AlreadyInteracted();
            return;
        }

        Interacted = true;
        
        SoundManager.Instance().Play(soundName);
        
        Player localPlayer = PlayerManager.Instance().LocalPlayer;
        
        localPlayer.Stats.RefillPotion();
        
        PlayerManager.Instance().LocalContext.LootDataIDs.Add(lootData.LootID);
        
        UIManager.Instance().GetController<LogUIController>().ShowLog(lootData);
        
        

        lootAnimator?.SetTrigger("Interact");
    }

    private void InteractionTutorial(bool detect)
    {
        if (detect)
        {
            UIManager.Instance().GetController<TutorialUI>().TryTutorial(TutorialType.Exploration_Interaction);
        }
    }
}
