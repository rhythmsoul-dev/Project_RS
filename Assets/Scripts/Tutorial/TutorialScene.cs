using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScene : MonoBehaviour
{
    [SerializeField] private GuideUI guideUI;
    [SerializeField] private LootData lootData;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button memoButton;
    
    private void Start()
    {
        TutorialSystem.Instance().Init();
        CombatSystem.Instance().Init();
        
        settingButton.onClick.RemoveAllListeners();
        settingButton.onClick.AddListener(() => UIManager.Instance().ShowController<SettingsUIController>());
        
        memoButton.onClick.RemoveAllListeners();
        memoButton.onClick.AddListener(() => UIManager.Instance().ShowController<MemoUIController>());
        
        SoundManager.Instance().Play(GameConstants.Sound.INTRO_BGM);
        
        guideUI.Init();
        if (!guideUI.IsAllGuideOver)
        {
            guideUI.StartCoroutine(guideUI.GuideCheck());
        }
        StartCoroutine(TutorialSceneStart());
    }
    
    private IEnumerator TutorialSceneStart()
    {
        yield return new WaitUntil(() => UIManager.Instance().HasController<TutorialUI>());

        yield return new WaitUntil(() => guideUI.IsAllGuideOver);

        SoundManager.Instance().SwitchBGM(GameConstants.Sound.GRAVE_YARD_BGM, 2f);

        yield return null;
        
        SoundManager.Instance().Play(GameConstants.Sound.GET_NOTE);
        Memo memo = new Memo(lootData.Memo);
        PlayerManager.Instance().LocalPlayer.AddMemo(memo);
        UIManager.Instance().GetController<LogUIController>().ShowLog(lootData);

        yield return new WaitForSeconds(0.2f);

        UIManager.Instance().GetController<TutorialUI>().TryTutorial(TutorialType.Exploration_Movement);
    }
}
