using System;
using System.Collections;
using UnityEngine;

public class ShopSceneUISetUp : MonoBehaviour
{
    [SerializeField] private ShopMenuUIController shopMenuUI;
    [SerializeField] private ShopStallUIController shopStallUI;
    [SerializeField] private ConversationUIController conversationUI;
    [SerializeField] private NPCDialogueUIController npcDialogueUI;
    
    private void Start()
    {
        RegisterUI(); 
        StartCoroutine(OnStart());
        StartCoroutine(ShopTutorialCheck());
        SetDialogue();
    }
    
    private IEnumerator OnStart()
    {
        SoundManager.Instance().StopBGM();
        yield return null;
        SoundManager.Instance().Play(GameConstants.Sound.SHOP_BGM);
    }
    
    private IEnumerator ShopTutorialCheck()
    {
        TutorialSystem.Instance().Init();
        
        if (!TutorialSystem.Instance().IsCompleted(TutorialType.General_ShopUI))
        {
            yield return new WaitUntil(() => UIManager.Instance().HasController<TutorialUI>());
            
            yield return new WaitUntil(() => PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFirstMeet.ToId(), out var b) && b);
            
            UIManager.Instance().GetController<TutorialUI>().TryTutorial(TutorialType.General_ShopUI);
        }
    }

    private void SetDialogue()
    {
        if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFirstMeet2.ToId(), out var _))
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.MerchantFirstMeet2.ToId()] = false;
        }
        PlayerManager.Instance().OnContextChanged();
    }
    private void RegisterUI()
    {
        UIManager.Instance().RegisterController(npcDialogueUI);
        UIManager.Instance().RegisterController(shopMenuUI);
        UIManager.Instance().RegisterController(shopStallUI);
        UIManager.Instance().RegisterController(conversationUI);
    }
}
