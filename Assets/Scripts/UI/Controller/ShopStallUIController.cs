using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 악보 상점 스톨 UI 컨트롤러
/// - 상점은 3종류(공격/체력/체간) 악보를 한 페이지에 묶어서 보여줌.
/// - 페이지는 악보의 "증가치 단계(1~3단계)"에 대응.
///   예) 2단계 페이지 버튼 클릭 → 2단계 공격/체력/밸런스 악보 3개가 한 화면에 진열.
/// - 특정 적 처치로 상위 단계 페이지 버튼이 해금됨.
/// - 구매 성공/실패 결과에 따라 팝업과 대사가 출력.
/// </summary>
public class ShopStallUIController : UIControllerBase
{
    [SerializeField] private GameObject shopStallUI;   // 상점 UI 패널
    
    // 배경
    [SerializeField] private Image backGround;
    [SerializeField] private GameObject bG;
    [SerializeField] private List<Sprite> backGroundImages;

    // 닫기 버튼
    [SerializeField] private Button exitButton;
    
    // 플레이어 골드 표시
    [SerializeField] private TMP_Text gold;

    // 페이지 전환 버튼
    [SerializeField] private List<Button> pageChangeButtons;
    
    // 한 페이지에 진열되는 UI 슬롯들
    [SerializeField] private List<Image> sheetMusicIcons;   // 아이콘
    [SerializeField] private List<TMP_Text> sheetMusicNames; // 이름
    [SerializeField] private List<TMP_Text> sheetMusicEffects; // 효과 설명
    [SerializeField] private List<Button> sheetMusicButtons; // 구매 버튼
    [SerializeField] private List<TMP_Text> sheetMusicPrice; // 가격
    [SerializeField] private List<GameObject> soldOut;      // 이미 소유시 '품절' 표시

    // 구매 확인/결과 팝업
    [SerializeField] private GameObject messagePanel; // 전체 팝업 패널
    [SerializeField] private GameObject checkMessage; // "구매하시겠습니까?" 확인 창
    [SerializeField] private Button checkYesButton;   // 예
    [SerializeField] private Button checkNoButton;    // 아니오
    [SerializeField] private GameObject resultMessage;// 결과 창
    [SerializeField] private TMP_Text resultText;     // 결과 문구 ("구매 성공!" / "구매 실패")
    [SerializeField] private Button confirmButton;    // 결과 확인 버튼

    // 구매 결과에 따른 팝업 대사를 잠깐 띄웠다가 닫기 위한 서브 버튼
    [SerializeField] private Button subButton;
    [SerializeField] private GameObject subButtonObject;

    private int currentPage;          // 현재 페이지
    private int selectedItem;         // 현재 구매 확인 중인 아이템
    private List<SheetMusic> shopItems; // 상점에 진열할 악보

    /// <summary>
    /// - 페이지 해금 조건(특정 적 처치 여부)에 따라 버튼 활성/리스너 부여
    /// - 상점 인벤토리 로드 후 슬롯 채우기 + 배경 적용
    /// </summary>
    protected override void SetUp()
    {
        currentPage = 0; // 기본은 1단계 페이지

        // 닫기
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Exit);

        // 구매 확인 팝업: 예/아니오
        checkYesButton.onClick.RemoveAllListeners();
        checkYesButton.onClick.AddListener(CheckYes);
        checkNoButton.onClick.RemoveAllListeners();
        checkNoButton.onClick.AddListener(CheckNo);

        // 구매 결과 팝업 확인 버튼
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmCliked);
        
        // 1단계 페이지 버튼(항상 열려있다고 가정)
        pageChangeButtons[0].onClick.RemoveAllListeners();
        pageChangeButtons[0].onClick.AddListener(() => OnPageChange(0));
        
        // 2단계 페이지 버튼: 특정 적 처치 시 해금 (첫 번째 보스)
        if (PlayerManager.Instance().LocalContext.KilledEnemies.ContainsKey(404))
        {
            pageChangeButtons[1].onClick.RemoveAllListeners();
            pageChangeButtons[1].onClick.AddListener(() => OnPageChange(1));
        }
        
        // 3단계 페이지 버튼: 더 강한 적 처치 시 해금 (두 번째 보스)
        if (PlayerManager.Instance().LocalContext.KilledEnemies.ContainsKey(408))
        {
            pageChangeButtons[2].onClick.RemoveAllListeners();
            pageChangeButtons[2].onClick.AddListener(() => OnPageChange(2));
        }
        
        // 상점 인벤토리 로드 → shopItems 구성 → 슬롯/배경 세팅
        ShopSystem.Instance().LoadShopInventory((inventory)=>
        {
            shopItems = new List<SheetMusic>();
            
            for (int i = 0; i < inventory.ItemsForSale.Count; i++)
            {
                shopItems.Add(new SheetMusic(inventory.ItemsForSale[i]));
            }
            
            SetShopStall(); // 현재 페이지에 맞춰 슬롯 채우기
            SetBackGround(); // 현재 페이지 배경 적용
        });
        
        // 대사 닫기용 보조 버튼 (구매 성공/실패 시 상점주인의 멘트를 띄웠다가 닫는 용도)
        subButton.onClick.RemoveAllListeners();
        subButton.onClick.AddListener(HideMonologue);
    }

    public override void Show()
    {
        shopStallUI.SetActive(true);
    }

    public override void Hide()
    {
        shopStallUI.SetActive(false);
    }

    /// <summary>
    /// 현재 페이지에 해당하는 배경 이미지로 교체
    /// </summary>
    private void SetBackGround()
    {
        backGround.sprite = backGroundImages[currentPage];
    }

    /// <summary>
    /// 현재 페이지의 3개 슬롯을 채운다.
    /// - 골드 표시 업데이트
    /// - pageSize = currentPage * 3
    /// - 각 슬롯에 아이콘/이름/효과/가격 바인딩
    /// - 이미 보유한 악보면 '품절' 표시 + 버튼 비활성
    /// </summary>
    private void SetShopStall()
    {
        gold.SetText(PlayerManager.Instance().LocalContext.Stats.Gold.ToString() + " G");
        
        int pageSize = currentPage * 3; // 0→0~2, 1→3~5, 2→6~8 ...

        for (int i = 0; i < 3; i++)
        {
            int idx = i;

            // 아이콘 로드
            shopItems[idx + pageSize].LoadIcon((icon) =>
            {
                sheetMusicIcons[idx].sprite = icon;
            });

            // 텍스트 바인딩
            sheetMusicNames[idx].SetText(shopItems[idx + pageSize].Name);
            sheetMusicEffects[idx].SetText(shopItems[idx + pageSize].Description);
            sheetMusicPrice[idx].SetText(shopItems[idx + pageSize].Price.ToString() + " G");
            
            // 기본 상태: 구매 가능
            soldOut[idx].SetActive(false);
            sheetMusicButtons[idx].onClick.RemoveAllListeners();
            sheetMusicButtons[idx].onClick.AddListener(() => PopUpCheckMessage(idx));

            // 이미 보유한 악보라면 '품절' 처리 및 버튼 비활성
            if (PlayerManager.Instance().LocalContext.SheetMusics.Any(sm => sm.Id == shopItems[idx + pageSize].Id))
            {
                soldOut[idx].SetActive(true);
                sheetMusicButtons[idx].onClick.RemoveAllListeners();
            }
        }
    }

    /// <summary>
    /// 페이지 변경.
    /// </summary>
    private void OnPageChange(int index)
    {
        currentPage = index;
        SetBackGround();
        SetShopStall();
    }
   
    /// <summary>
    /// 구매 확인 팝업 띄우기.
    /// - 확인 창 활성화 / 결과 창 비활성화
    /// </summary>
    private void PopUpCheckMessage(int index)
    {
        selectedItem = index + (currentPage * 3);
        messagePanel.SetActive(true);
        checkMessage.SetActive(true);
        resultMessage.SetActive(false);
    }

    /// <summary>
    /// [예] 클릭 시 구매 시도.
    /// TryBuyItem 반환값:
    /// 1 -> 성공, 2 -> 돈 부족, 3 -> 이미 소지
    /// - 성공/실패에 따라 결과 문구 + 상점주인 대사 호출
    /// </summary>
    private void CheckYes()
    {
        if (ShopSystem.Instance().TryBuyItem(shopItems[selectedItem]) == 1)
        {
            SoundManager.Instance().Play(GameConstants.Sound.BUY_ITEM);
            resultText.SetText("구매 성공!");
            UIManager.Instance().GetController<ConversationUIController>().Monologue(DialogueKey.MerchantPurchaseSuccess.ToId());
            subButtonObject.SetActive(true);
        }
        else if (ShopSystem.Instance().TryBuyItem(shopItems[selectedItem]) == 2)
        {
            resultText.SetText("구매 실패");
            UIManager.Instance().GetController<ConversationUIController>().Monologue(DialogueKey.MerchantPurchaseFail1.ToId());
            subButtonObject.SetActive(true);
        }
        else
        {
            resultText.SetText("구매 실패");
            UIManager.Instance().GetController<ConversationUIController>().Monologue(DialogueKey.MerchantPurchaseFail2.ToId());
            subButtonObject.SetActive(true);
        }

        checkMessage.SetActive(false);
        resultMessage.SetActive(true);
    }

    /// <summary>
    /// [아니오] 클릭 시 확인 창 닫기
    /// </summary>
    private void CheckNo()
    {
        messagePanel.SetActive(false);
        checkMessage.SetActive(false);
    }

    /// <summary>
    /// 결과 팝업 확인 버튼.
    /// - 팝업 닫고
    /// - 슬롯 상태 갱신(골드/품절 등 반영)
    /// </summary>
    private void OnConfirmCliked()
    {
        messagePanel.SetActive(false);
        resultMessage.SetActive(false);
        SetShopStall();
    }

    /// <summary>
    /// 상점 스톨 닫고 상점 메인 메뉴로 복귀
    /// </summary>
    private void Exit()
    {
        Hide();
        UIManager.Instance().ShowController<ShopMenuUIController>();
    }

    /// <summary>
    /// 구매 후 띄운 상점주인 멘트 닫기용 보조 버튼
    /// </summary>
    private void HideMonologue()
    {
        UIManager.Instance().GetController<ConversationUIController>().Hide();
        subButtonObject.SetActive(false);
    }
}
