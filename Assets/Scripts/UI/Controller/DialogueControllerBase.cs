using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 공통 대화 컨트롤러의 베이스 클래스.
/// 
/// 설계 개요:
/// - 플레이어는 진행되어야 할 대사들을 Dictionary<string, bool>로 보유한다.
///   * key: DialogueId (ScriptableObject가 가진 식별자)
///   * value: 이미 진행(true) / 미진행(false)
/// - NPC는 자신의 대사 목록(Dialogue SO 리스트)을 우선순위로 정렬해 둔다.
/// - 대화 시작 시, NPC는 우선순위 높은 순서대로
///     (1) 그 대사가 플레이어의 "진행해야 할 대사 목록"에 존재하는지
///     (2) 아직 진행되지 않았는지
///   를 확인하고, 조건에 맞는 첫 대사를 선택한다. (없으면 기본 대사를 진행)
/// - 선택된 대사의 "대사 시퀀스"를 큐에 담아 한 덩어리씩 출력한다.
/// - 모든 시퀀스를 소진하면 EndDialogue()에서
///     (1) 플레이어 진행상황을 true로 갱신/저장
///     (2) 씬 맥락(교회/상점)에 맞는 메뉴 UI 복귀
///   를 수행한다.
/// 
/// 추상 메서드:
/// - DisplayEntry(DialogueText) / DisplayEntry(string)
///   : 실제 표시는 파생 클래스에서 구현 (예: 타이핑 효과, TMP 적용 등)
/// </summary>
public abstract class DialogueControllerBase : UIControllerBase
{
    [SerializeField] protected GameObject conversationUI; // 대화 패널
    [SerializeField] protected Button dialogueButton;     // 다음 대사로 넘김 버튼
    
    // 이 NPC가 보유한 대사 ScriptableObject 목록.
    [SerializeField] private List<Dialogue> dialogueList;

    // 현재 선택되어 진행 중인 대사
    private Dialogue curDialogue;

    // 화면에 표시할 대사 큐
    protected Queue<DialogueText> dialogueQueue = new Queue<DialogueText>();

    // DialogueId → Dialogue SO 매핑 캐시 (빠른 접근을 위해)
    protected Dictionary<string, Dialogue> dialogueMap = new Dictionary<string, Dialogue>();

    // 진행 중인 텍스트 트윈, 텍스트 스킵 제어용
    protected Tween DialogueTween;

    // 트윈 스킵 시 즉시 출력할 대사를 저장할 변수
    protected string prevText;

    /// <summary>
    /// NPC의 대사들을 우선순위 오름차순으로 정렬.
    /// </summary>
    protected override void SetUp()
    {
        dialogueList.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }

    /// <summary>
    /// 대화 UI 표시와 함께 대화 흐름 시작.
    /// </summary>
    public override void Show()
    {
        conversationUI.SetActive(true);
        StartConversation();
    }

    /// <summary>
    /// 대화 UI 숨김.
    /// </summary>
    public override void Hide()
    {
        conversationUI.SetActive(false);
    }

    /// <summary>
    /// 우선순위/진행상태를 고려하여 현재 진행할 대사를 정하고,
    /// 해당 대사의 시퀀스를 큐에 채운 뒤 첫 항목을 출력.
    /// </summary>
    private void StartConversation()
    {
        SetCurrentDialogue();
        DisplayNext();
    }

    /// <summary>
    /// 외부에서 특정 DialogueId로 시작할 때 사용. (ex.단일 대사 흐름)
    /// </summary>
    public void Monologue(string dialogueId)
    {
        conversationUI.SetActive(true);
        SetDialogue(dialogueId);
        DisplayNext();
    }

    /// <summary>
    /// "다음" 동작:
    /// - 트윈(타이핑 등)이 진행 중이면 즉시 스킵해서 완성 텍스트출력.
    /// - 아니면 큐에서 다음 DialogueText를 꺼내서 표시
    /// - 큐가 비었으면 대화 종료 처리
    /// </summary>
    public void DisplayNext()
    {
        // 진행 중인 텍스트 트윈이 있으면 스킵: 바로 완성 텍스트 출력
        if (DialogueTween != null && DialogueTween.IsActive() && prevText != null)
        {
            DialogueTween.Kill();
            DisplayEntry(prevText);
            return;
        }

        // 더 출력할 대사가 없는 경우 → 종료
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
        }
        // 다음 대사 출력
        else if (dialogueQueue.Count > 0)
        {
            var entry = dialogueQueue.Dequeue();
            DisplayEntry(entry);
        }
    }

    /// <summary>
    /// 파생 클래스에서 타이핑 효과등 실제 렌더링 구현.
    /// </summary>
    protected abstract void DisplayEntry(DialogueText entry);

    /// <summary>
    /// 파생 클래스에서 타이핑 효과등 실제 렌더링 구현.
    /// </summary>
    protected abstract void DisplayEntry(string entryText);

    /// <summary>
    /// 대화 종료 처리:
    /// 1) 씬에 맞는 메뉴 UI로 복귀
    /// 2) 플레이어 대화 진행상황을 true로 갱신 및 저장.
    /// </summary>
    private void EndDialogue()
    {
        string scene = SceneManager.GetActiveScene().name;

        // 대화가 끝나면 대화진행 전 메뉴 UI로 돌아간다.
        if (scene == GameConstants.Scene.CHURCH_SCENE)
        {
            UIManager.Instance().ShowController<ChurchMenuUIController>();
        }
        else if (scene == GameConstants.Scene.SHOP_SCENE)
        {
            UIManager.Instance().ShowController<ShopMenuUIController>();
        }

        // 대화 패널 숨김
        Hide();

        // 진행상황 갱신:
        // 플레이어는 "진행해야 할 대사 목록"을 Dictionary<string,bool>로 들고 있음.
        // 현재 대사의 DialogueId가 존재한다면 true로 바꾸어 완료 처리.
        if (PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(curDialogue.DialogueId, out bool _))
        {
            PlayerManager.Instance().LocalContext.Dialogues[curDialogue.DialogueId] = true;
            PlayerManager.Instance().OnContextChanged(); // 저장/세이브 트리거 등
        }
    }

    /// <summary>
    /// 주어진 DialogueId로 해당 대사의 시퀀스를 큐에 넣음.
    /// (초기 접근 시 dialogueMap이 비어 있으면 맵 초기화)
    /// </summary>
    private void SetDialogue(string dialogueId)
    {
        if (dialogueMap == null || dialogueMap.Count == 0)
        {
            SetDialogueMap();
        }

        dialogueQueue = new Queue<DialogueText>(dialogueMap[dialogueId].DialogueSequence);
    }

    /// <summary>
    /// 로직:
    /// 1) 우선순위 오름차순으로 순회.
    /// 2) 플레이어의 진행해야 할 대사 목록에 해당 DialogueId가 있고, 아직 진행되지 않았다면 그 대사를 선택
    /// 3) 모든 후보가 없으면 Priority==999인 기본 대사를 선택.
    /// </summary>
    private void SetCurrentDialogue()
    {
        if (dialogueList == null || dialogueList.Count == 0)
        {
            return; // 데이터가 없다면 아무 것도 하지 않음
        }

        foreach (Dialogue data in dialogueList)
        {
            curDialogue = data; // 일단 현재 후보로 잡아둔다

            // 플레이어 진행 목록에 존재하고, 아직 미진행이면 이 대사를 선택
            if (PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(data.DialogueId, out bool alreadySpoken) &&
                !alreadySpoken)
            {
                SetDialogue(data.DialogueId);
                
                if (data.DialogueId == DialogueKey.MerchantFinalBossClear.ToId())
                {
                    Debug.Log("마지막 대화, 이 대화 종료 후 -> 엔딩크레딧 재생");
                }

                break;
            }
            
            if (data.Priority == 999) // 기본 대사는 우선순위 999
            {
                SetDialogue(data.DialogueId);
                break;
            }
        }
    }

    /// <summary>
    /// DialogueId → Dialogue SO 매핑.
    /// (런타임 최초 접근 시 한 번 만들어 캐시)
    /// </summary>
    private void SetDialogueMap()
    {
        foreach (Dialogue d in dialogueList)
        {
            dialogueMap[d.DialogueId] = d;
        }
    }
}
