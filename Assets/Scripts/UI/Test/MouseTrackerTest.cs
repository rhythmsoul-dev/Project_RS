using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 마우스 위치를 추적하여 UI 요소를 마우스 옆에 띄우는 스크립트.
/// - UI 요소에 마우스를 올리면(하이라이트 상태) 마우스 근처에 설명 텍스트 표시
/// - 텍스트가 마우스를 따라다니도록 위치 업데이트
/// </summary>
public class MouseTrackerTest : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;         // UI가 속한 캔버스의 RectTransform
    [SerializeField] private RectTransform targetTextTransform;// 마우스를 따라다닐 UI(Text)의 RectTransform
    [SerializeField] private TMP_Text targetText;               // 표시할 텍스트
    [SerializeField] private CustomButton church;               // 교회 UI 버튼
    [SerializeField] private CustomButton shop;                 // 상점 UI 버튼

    private Vector2 mousePos; // 캔버스 기준 마우스 좌표

    void Update()
    {
        // [Church 버튼] 하이라이트 상태일 때
        if (church.IsHighlighted())
        {
            // 스크린 좌표 → 캔버스 로컬 좌표 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out mousePos);
            // 마우스 위치에서 오프셋(오른쪽 100, 아래 50) 적용
            targetTextTransform.anchoredPosition = mousePos + new Vector2(100f, -50f);
            // 텍스트 변경
            targetText.SetText("Church");
        }
        // [Shop 버튼] 하이라이트 상태일 때
        else if (shop.IsHighlighted())
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out mousePos);
            targetTextTransform.anchoredPosition = mousePos + new Vector2(100f, -50f);
            targetText.SetText("Shop");
        }
        // 버튼 둘 다 하이라이트가 아닐 때
        else
        {
            // 선택된 UI 해제
            EventSystem.current.SetSelectedGameObject(null);
            // 텍스트 숨김
            targetText.SetText("");
        }
    }
}