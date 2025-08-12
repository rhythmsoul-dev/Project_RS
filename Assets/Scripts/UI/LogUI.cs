using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogUI : MonoBehaviour
{
    [Header("기본값 설정")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text logText;
    
    private string goldLabel = "G";
    private string amountLabel = "개";
    private string getLabel = "획득";
    private StringBuilder stringBuilder = new StringBuilder();
    
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private float logSizeY;
    private int movingCount;
    private Coroutine moveUpCoroutine;
    public bool Activated { get; private set; }

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
        logSizeY = rectTransform.sizeDelta.y;
        
        canvasGroup = GetComponent<CanvasGroup>();
        iconImage = transform.FindComponent<Image>("Icon");
        logText = GetComponentInChildren<TMP_Text>();
    }

    public void Setup(LootData data, Sprite icon, float duration)
    {
        iconImage.sprite = icon;
        
        switch (data.Type)
        {
            case LootType.Gold:
                logText.text = stringBuilder.Clear().Append(data.Gold).Append(goldLabel).Append(" ").Append(getLabel).ToString();
                break;
            case LootType.Memo:
                string memoFirstLine = data.Memo.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];
                logText.text = stringBuilder.Clear().Append(memoFirstLine).Append("\n").Append(getLabel).ToString();
                break;
            case LootType.Potion:
                logText.text = stringBuilder.Clear().Append(data.PotionAmount).Append(amountLabel).Append(" ").Append(getLabel).ToString();
                break;
            case LootType.Sheet:
            default:
                Debug.LogError("해당 유형의 획득 로그 로직 없음");
                break;
        }
        
        StartCoroutine(FadeInOut(duration, true));
    }

    public IEnumerator FadeInOut(float duration, bool isFadeIn)
    {
        if (isFadeIn)
        {
            gameObject.SetActive(true);
        }

        float time = 0f;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0f : 1f, isFadeIn ? 1f : 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = isFadeIn ? 1f : 0f;
        Activated = isFadeIn;

        if (!isFadeIn)
        {
            gameObject.SetActive(false);
            rectTransform.localPosition = originalPosition;
            movingCount = 0;
        }
    }

    public void MoveUp(float speed)
    {
        movingCount++;
        
        if (moveUpCoroutine != null)
        {
            StopCoroutine(moveUpCoroutine);
        }
        
        moveUpCoroutine = StartCoroutine(MovingUp(speed));
    }

    private IEnumerator MovingUp(float speed)
    {
        targetPosition = originalPosition + Vector3.up * (logSizeY * movingCount);
        
        while (Vector3.Distance(rectTransform.localPosition, targetPosition) > 0.1f)
        {
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, targetPosition, Time.deltaTime * speed);
            yield return null;
        }
        
        rectTransform.localPosition = targetPosition;
    }
}
