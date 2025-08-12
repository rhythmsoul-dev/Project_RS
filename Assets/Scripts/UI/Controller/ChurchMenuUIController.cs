using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 교회 메뉴 UI 컨트롤러
/// UIManager를 통해 등록/참조되는 씬 전용 UI 중 하나.
/// 씬 진입 시 버튼 이벤트 연결, 씬 나갈 때 참조 해제되는 구조.
/// </summary>
public class ChurchMenuUIController : UIControllerBase
{
    [SerializeField] private GameObject churchMenuUI;  // 교회 메뉴 UI 전체 패널
    [SerializeField] private Button portalButton;      // 포탈 버튼
    [SerializeField] private Button conversationButton;// 대화 버튼
    [SerializeField] private Button exitButton;        // 나가기 버튼
    [SerializeField] private GameObject fog;           // 화면 안개효과

    /// <summary>
    /// UIManager가 컨트롤러 등록 시 호출.
    /// 버튼 클릭 이벤트를 초기화하고 새로 연결.
    /// </summary>
    protected override void SetUp()
    {
        // 포탈 버튼 → 포탈 UI 열기
        portalButton.onClick.RemoveAllListeners();
        portalButton.onClick.AddListener(OpenPortalUI);
        
        // 대화 버튼 → 대화 UI 열기
        conversationButton.onClick.RemoveAllListeners();
        conversationButton.onClick.AddListener(OpenConversationUI);
        
        // 나가기 버튼 → 마을 씬으로 이동
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Exit);
    }

    /// <summary>
    /// UI 표시
    /// </summary>
    public override void Show()
    {
        fog.SetActive(true);
        churchMenuUI.SetActive(true);
    }

    /// <summary>
    /// UI 숨기기
    /// </summary>
    public override void Hide()
    {
        fog.SetActive(false);
        churchMenuUI.SetActive(false);
    }

    /// <summary>
    /// 포탈 UI 열기.
    /// 특정 대화가 진행된 경우 → 포탈 UI 열기,
    /// 아니면 대화 UI로 대체.
    /// </summary>
    private void OpenPortalUI()
    {
        if (PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFirstMeet2.ToId(), out bool b) && b)
        {
            Hide();
            UIManager.Instance().ShowController<PortalUIController>();
        }
        else
        {
            OpenConversationUI();
        }
    }

    /// <summary>
    /// 대화 UI 열기.
    /// </summary>
    private void OpenConversationUI()
    {
        Hide();
        SetMessage();
        UIManager.Instance().ShowController<ConversationUIController>();
    }

    /// <summary>
    /// 나가기시 마을 씬으로 전환.
    /// </summary>
    private void Exit()
    {
        SceneLoader.Instance().LoadScene(GameConstants.Scene.VILLAGE_SCENE);
    }

    /// <summary>
    /// 수녀 대사 조건 확인 후 조건에 맞게 세팅.
    /// 대사 진행상황 저장.
    /// </summary>
    private void SetMessage()
    {
        if (PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFirstMeet2.ToId(), out bool b) && b)
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunQuestMessage.ToId()] = true;
        }
        else
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunQuestMessage.ToId()] = false;
        }
        PlayerManager.Instance().OnContextChanged();
    }
}
