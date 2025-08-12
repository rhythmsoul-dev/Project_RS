using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AdventureScene : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private PortalUIController portalUI;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button memoButton;
    [SerializeField] private GuideUI guideUI;

    private void Start()
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraManager.Instance().MainCamera;
        canvas.sortingLayerName = "CombatUI";
        
        CombatSystem.Instance().Init();

        StartCoroutine(OnStart());
        
        portalUI.Init();
        UIManager.Instance().RegisterController(portalUI);
        
        settingButton.onClick.RemoveAllListeners();
        settingButton.onClick.AddListener(() => UIManager.Instance().ShowController<SettingsUIController>());
        
        memoButton.onClick.RemoveAllListeners();
        memoButton.onClick.AddListener(() => UIManager.Instance().ShowController<MemoUIController>());

        if (guideUI != null)
        {
            guideUI.Init();
            guideUI.transform.parent.gameObject.SetActive(false);
        }
    }

    private IEnumerator OnStart()
    {
        SoundManager.Instance().StopBGM();

        yield return null;
        
        if (SceneLoader.Instance().CurMapType == MapType.Graveyard)
        {
            SoundManager.Instance().Play(GameConstants.Sound.GRAVE_YARD_BGM);
        }
        if (SceneLoader.Instance().CurMapType == MapType.Dungeon)
        {
            SoundManager.Instance().Play(GameConstants.Sound.DUNGEON_BGM);
            if (SceneLoader.Instance().CurMapIndex == 1)
            {
                StartCoroutine(CheckClear());
            }
        }
    }

    private IEnumerator CheckClear()
    {
        yield return new WaitUntil(() => PlayerManager.Instance().LocalContext.KilledEnemies.ContainsKey(394));
        
        yield return new WaitUntil(() => PlayerManager.Instance().LocalContext.Dialogues.ContainsKey("PrincessTrigger"));

        if (!guideUI.IsAllGuideOver)
        {
            guideUI.transform.parent.gameObject.SetActive(true);
            guideUI.GuideOverAction = () => PortalManager.Instance().MoveToPortal();
            guideUI.StartCoroutine(guideUI.GuideCheck());
        }
        
        yield return null;
    }
}
