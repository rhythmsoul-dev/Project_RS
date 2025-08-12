using System.Collections;
using UnityEngine;

public class ChurchSceneUISetup : MonoBehaviour
{
    [SerializeField] private ChurchMenuUIController churchMenuUI;
    [SerializeField] private PortalUIController portalUI;
    [SerializeField] private ConversationUIController conversationUI;
    [SerializeField] private NPCDialogueUIController npcDialogueUI;

    private void Start()
    {
        RegisterUI();
        StartCoroutine(OnStart());
        StartCoroutine(ChurchTutorialCheck());
        SetDialogue();
    }

    private IEnumerator OnStart()
    {
        SoundManager.Instance().StopBGM();
        yield return null;
        SoundManager.Instance().Play(GameConstants.Sound.CHURCH_BGM);
    }

    private IEnumerator ChurchTutorialCheck()
    {
        TutorialSystem.Instance().Init();
        
        if (!TutorialSystem.Instance().IsCompleted(TutorialType.General_Dialogue))
        {
            yield return new WaitUntil(() => UIManager.Instance().HasController<TutorialUI>());
            
            yield return new WaitUntil(() => PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.NunFirstMeet.ToId(), out var b) && b);
            
            UIManager.Instance().GetController<TutorialUI>().TryTutorial(TutorialType.General_Dialogue);
        }
    }

    private void SetDialogue()
    {
        if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.NunAfterMerchant.ToId(), out var _) &&// NunAfterMerchant 대화를 진행하지 않았고
            PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFirstMeet.ToId(), out var b) && b) // MerchantFirstMeet을 진행했다면 
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunAfterMerchant.ToId()] = false;
        }
        
        if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.NunFirstMeet2.ToId(), out var _))
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunFirstMeet2.ToId()] = false;
        }
        PlayerManager.Instance().OnContextChanged();
    }
    private void RegisterUI()
    {
        UIManager.Instance().RegisterController(churchMenuUI);     
        portalUI.Init();
        UIManager.Instance().RegisterController(portalUI);
        UIManager.Instance().RegisterController(conversationUI);
        UIManager.Instance().RegisterController(npcDialogueUI);
    }
}
