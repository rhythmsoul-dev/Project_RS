using System;
using UnityEngine;

/// <summary>
/// 악보 UI 컨트롤러
/// - 플레이어가 보유한 악보 아이템을 능력치 종류(공격/체력/체간)별 슬롯에 배치
/// - 같은 종류의 악보는 약간씩 x축으로 오프셋을 줘서 "겹쳐 보이게" 표현
///   → 한눈에 종류별 구역, 개수까지 파악 가능
/// </summary>
public class SheetMusicUIController : UIControllerBase
{
    [SerializeField] private GameObject sheetMusicUI;         // 전체 악보 UI
    [SerializeField] private GameObject sheetMusicItemPrefab; // 표시용 아이템 프리팹
    
    // 종류별 부모 슬롯(컨테이너). 같은 종류의 악보는 각 슬롯 아래에 겹쳐서 배치됨.
    [SerializeField] private RectTransform attackSheetSlot;   // 공격력 증가 악보 구역
    [SerializeField] private RectTransform healthSheetSlot;   // 체력 증가 악보 구역
    [SerializeField] private RectTransform balanceSheetSlot;  // 체간 증가 악보 구역

    // 같은 종류일 때 살짝 겹쳐 보이도록 하는 x축 오프셋
    private float offset = 10f;
    
    /// <summary>
    /// 플레이어가 가진 악보를 순회하며 종류별 슬롯에 생성/배치한다.
    /// </summary>
    protected override void SetUp()
    {
        // 플레이어의 보유 악보 리스트를 가져와 슬롯에 채우기
        foreach (SheetMusic sheetMusic in PlayerManager.Instance().LocalContext.SheetMusics)
        {
            GameObject sheetMusicSlot = null;
            RectTransform rt;
            Vector2 ap;

            //   악보 종류 판별:
            //   sheetMusic.Id % 3 결과로 분기 (1=공격, 2=체력, 0=밸런스)
            //   같은 구역에 생성하고, childCount를 이용해 x축으로 겹치게 배치
            switch (sheetMusic.Id % 3)
            {
                case 1 : // 공격력 증가 악보
                    sheetMusicSlot = Instantiate(sheetMusicItemPrefab, attackSheetSlot);
                    rt = sheetMusicSlot.GetComponent<RectTransform>();
                    ap = rt.anchoredPosition;
                    // 현재 슬롯 자식 수 - 1 만큼 오른쪽으로 밀어 겹치기
                    ap.x = offset * (attackSheetSlot.childCount - 1);
                    rt.anchoredPosition = ap;
                    break; 

                case 2 : // 체력 증가 악보
                    sheetMusicSlot = Instantiate(sheetMusicItemPrefab, healthSheetSlot);
                    rt = sheetMusicSlot.GetComponent<RectTransform>();
                    ap = rt.anchoredPosition;
                    ap.x = offset * (healthSheetSlot.childCount - 1);
                    rt.anchoredPosition = ap;
                    break;

                case 0 : // 체간 증가 악보
                    sheetMusicSlot = Instantiate(sheetMusicItemPrefab, balanceSheetSlot);
                    rt = sheetMusicSlot.GetComponent<RectTransform>();
                    ap = rt.anchoredPosition;
                    ap.x = offset * (balanceSheetSlot.childCount - 1);
                    rt.anchoredPosition = ap;
                    break;
            }

            // 아이콘 세팅
            SheetMusicSlot sheetMusicSlotScript = sheetMusicSlot?.GetComponent<SheetMusicSlot>();

            // 악보 데이터에서 아이콘 로드.
            sheetMusic.LoadIcon((icon) =>
            {
                sheetMusicSlotScript.SetIcon(icon);
            });
        }
    }

    /// <summary>
    /// 악보 UI 보이기
    /// </summary>
    public override void Show()
    {
        sheetMusicUI.SetActive(true);
    }

    /// <summary>
    /// 악보 UI 숨기기
    /// </summary>
    public override void Hide()
    {
        sheetMusicUI.SetActive(false);
    }
}
