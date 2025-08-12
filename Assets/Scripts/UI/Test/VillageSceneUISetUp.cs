using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VillageSceneUISetUp : MonoBehaviour
{
    [SerializeField] private CustomButton church;
    [SerializeField] private CustomButton shop;
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button openMemoButton;
    [SerializeField] private GuideUI guideUI;
    
    private void Awake()
    {
        church.onClick.RemoveAllListeners();
        church.onClick.AddListener(GoChurch);
        
        shop.onClick.RemoveAllListeners();
        
        if (PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.NunFirstMeet2.ToId(), out var a) && a)
        {
            if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.NunFinalBossClear.ToId(), out var b) || b)
            {
                shop.onClick.AddListener(GoShop);
            }
            else
            {
                Debug.Log("최종보스 처치 후 교회에 방문하지 않았음.");    
            }
        }
        else
        {
            Debug.Log("튜토리얼 후 교회에 방문하지 않았음.");
        }
        
        openSettingsButton.onClick.RemoveAllListeners();
        openSettingsButton.onClick.AddListener(() => UIManager.Instance().ShowController<SettingsUIController>());

        openMemoButton.onClick.RemoveAllListeners();
        openMemoButton.onClick.AddListener(() => UIManager.Instance().ShowController<MemoUIController>());
    }

    private void Start()
    {
        if (!PlayerManager.Instance().LocalContext.Dialogues.ContainsKey("Credit") && 
            PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFinalBossClear.ToId(), out var a) && a)
        {
            guideUI.GuideOverAction = () => SceneLoader.Instance().LoadScene(GameConstants.Scene.CREDIT_SCENE);
        }
        
        StartCoroutine(OnStart());
        
        PlayerManager.Instance().EnterVillage();

        StartCoroutine(TutorialInit());
        
        guideUI.Init();
        if (!guideUI.IsAllGuideOver)
        {
            guideUI.StartCoroutine(guideUI.GuideCheck());
        }
    }
    
    private IEnumerator OnStart()
    {
        SoundManager.Instance().StopBGM();
        yield return null;
        SoundManager.Instance().Play(GameConstants.Sound.VILLAGE_BGM);
    }

    private IEnumerator TutorialInit()
    {
        TutorialSystem.Instance().Init();
        
        if (!TutorialSystem.Instance().IsCompleted(TutorialType.General_Town))
        {
            yield return new WaitUntil(() => UIManager.Instance().HasController<TutorialUI>());
            UIManager.Instance().GetController<TutorialUI>().TryTutorial(TutorialType.General_Town);
        }
        
        yield break;
    }

    private void GoChurch()
    {
        SceneLoader.Instance().LoadScene(GameConstants.Scene.CHURCH_SCENE);
        if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.NunFirstMeet.ToId(), out _))
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunFirstMeet.ToId()] = false;
        }
        PlayerManager.Instance().OnContextChanged();
    }

    private void GoShop()
    {
        SceneLoader.Instance().LoadScene(GameConstants.Scene.SHOP_SCENE);
        if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFirstMeet.ToId(), out _))
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.MerchantFirstMeet.ToId()] = false;
        }
        
        PlayerManager.Instance().OnContextChanged();
    }
}


