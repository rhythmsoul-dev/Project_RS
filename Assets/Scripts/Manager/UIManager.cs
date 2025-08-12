using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using JetBrains.Annotations;
using System;
using DG.Tweening;

//public enum UIType
//{
//    Gold,
//    SheetMusic,
//    ChurchMenu,
//    Portal,
//    Bank,
//    Conversation,
//    ShopMenu,
//    Shop,
//    Settings,
//    Defeat,
//    Potion
//}

public class UIManager : Singleton<UIManager>
{
    public bool IsInterrupted { get; private set; }

    [SerializeField] private Toast toastPrefab;
    private List<Toast> toasts = new List<Toast>();

    private Dictionary<Type, UIControllerBase> controllers = new Dictionary<Type, UIControllerBase>();

    //public GoldUIController GoldUI { get; private set; }
    //public SheetMusicUIController SheetMusicUI { get; private set; }
    //public ChurchMenuUIController ChurchMenuUI { get; private set; }
    //public PortalUIController PortalUI { get; private set; }
    //public BankUIController BankUI { get; private set; }
    //public ConversationUIController ConversationUI { get; private set; }
    //public ShopMenuUIController ShopMenuUI { get; private set; }

    //public ShopUIController ShopUI { get; private set; }

    //public DefeatUIController DefeatUI { get; private set; }

    //public FadeUIController FadeUI { get; private set; }
    ////public FadeInOutUI FadeInOutUI { get; private set; }
    //public SettingsUIController SettingsUI { get; private set; }

    //public PotionUIController PotionUI { get; private set; }

    //public void Show(UIType type)
    //{
    //    switch (type)
    //    {
    //        case UIType.Gold:
    //            GoldUI.Nullable()?.Show();
    //            break;
    //        case UIType.SheetMusic:
    //            SheetMusicUI.Nullable()?.Show();
    //            break;
    //        case UIType.ChurchMenu:
    //            ChurchMenuUI.Nullable()?.Show();
    //            break;
    //        case UIType.Portal:
    //            PortalUI.Nullable()?.Show();
    //            break;
    //        case UIType.Bank:
    //            BankUI.Nullable()?.Show();
    //            break;
    //        case UIType.Conversation:
    //            ConversationUI.Nullable()?.Show();
    //            break;
    //        case UIType.ShopMenu:
    //            ShopMenuUI.Nullable()?.Show();
    //            break;
    //        case UIType.Shop:
    //            ShopUI.Nullable()?.Show();
    //            break;
    //        case UIType.Settings:
    //            SettingsUI.Nullable()?.Show();
    //            break;
    //        case UIType.Defeat:
    //            DefeatUI.Nullable()?.Show();
    //            break;
    //        default:
    //            Debug.LogWarning($"UIType 없음: {type}");
    //            break;
    //    }
    //}

    //public void Hide(UIType type)
    //{
    //    switch (type)
    //    {
    //        case UIType.Gold:
    //            GoldUI.Nullable()?.Hide();
    //            break;
    //        case UIType.SheetMusic:
    //            SheetMusicUI.Nullable()?.Hide();
    //            break;
    //        case UIType.ChurchMenu:
    //            ChurchMenuUI.Nullable()?.Hide();
    //            break;
    //        case UIType.Portal:
    //            PortalUI.Nullable()?.Hide();
    //            break;
    //        case UIType.Bank:
    //            BankUI.Nullable()?.Hide();
    //            break;
    //        case UIType.Conversation:
    //            ConversationUI.Nullable()?.Hide();
    //            break;
    //        case UIType.ShopMenu:
    //            ShopMenuUI.Nullable()?.Hide();
    //            break;
    //        case UIType.Shop:
    //            ShopUI.Nullable()?.Hide();
    //            break;
    //        case UIType.Settings:
    //            SettingsUI.Nullable()?.Hide();
    //            break;
    //        case UIType.Potion:
    //            PotionUI.Nullable()?.Hide();
    //            break;
    //        default:
    //            Debug.LogWarning($"UIType 없음: {type}");
    //            break;
    //    }
    //}

    public void ShowController<T>() where T : UIControllerBase
    {
        PlayerManager.Instance().SwitchPlayerInputState(PlayerInputState.Menu_State);
        controllers[typeof(T)].Show();
    }

    public void HideController<T>() where T : UIControllerBase
    {
        PlayerManager.Instance().BackToPreviousState();
        controllers[typeof(T)].Hide();
    }

    public bool HasController<T>() where T : UIControllerBase
    {
        return controllers.ContainsKey(typeof(T));
    }

    public T GetController<T>() where T : UIControllerBase
    {
        return controllers[typeof(T)] as T;
    }

    public void RegisterController<T>(T controller) where T : UIControllerBase
    {
        controllers[typeof(T)] = controller;
    }

    //public void RegisterController(GoldUIController controller)
    //{
    //    GoldUI = controller;
    //}

    //public void RegisterController(PotionUIController controller)
    //{
    //    PotionUI = controller;
    //}
    //public void RegisterController(SheetMusicUIController controller)
    //{
    //    SheetMusicUI = controller;
    //}

    //public void RegisterController(ChurchMenuUIController controller)
    //{
    //    ChurchMenuUI = controller;
    //}

    //public void RegisterController(PortalUIController controller)
    //{
    //    PortalUI = controller;
    //}

    //public void RegisterController(BankUIController controller)
    //{
    //    BankUI = controller;
    //}

    //public void RegisterController(ConversationUIController controller)
    //{
    //    ConversationUI = controller;
    //}

    //public void RegisterController(ShopMenuUIController controller)
    //{
    //    ShopMenuUI = controller;
    //}
    //public void RegisterController(ShopUIController controller)
    //{
    //    ShopUI = controller;
    //}

    //public void RegisterController(DefeatUIController defeatUI)
    //{
    //    DefeatUI = defeatUI;
    //}

    //public void RegisterController(FadeInOutUI fadeInOutUI)
    //{
    //    FadeInOutUI = fadeInOutUI;
    //}

    //public void RegisterController(SettingsUIController controller)
    //{
    //    SettingsUI = controller;
    //}

    //public void BattleFadeInOut(float duration)
    //{
    //    FadeUIController fadeUI = GetController<FadeUIController>();
    //    if (fadeUI != null)
    //    {
    //        fadeUI.Show();
    //        StartCoroutine(DelayFadeOut(duration));
    //    }
    //}

    //private IEnumerator DelayFadeOut(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    GetController<FadeUIController>().Hide();
    //}

    public void FadeIn(CanvasGroup canvasGroup, float time, string animation = "none")
    {
        StartCoroutine(ProcessFadeIn(canvasGroup, time, animation));
    }

    private IEnumerator ProcessFadeIn(CanvasGroup canvasGroup, float time, string animation = "none")
    {
        if (canvasGroup == null || IsInterrupted)
        {
            yield break;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(true);

        Image image = canvasGroup.GetComponent<Image>();
        RectTransform rectTransform = canvasGroup.GetComponent<RectTransform>();

        bool isScaleAnim = false;
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = Vector3.one;

        bool isPositionAnim = false;
        Vector3 initialPosition = Vector3.one;
        Vector3 targetPosition = Vector3.one;

        if (animation != "none")
        {
            string[] anim = animation.Split(" ");

            isScaleAnim = anim[0] == "bigger" || anim[0] == "smaller";
            if (isScaleAnim)
            {
                initialScale = Vector3.one * float.Parse(anim[1]);
                targetScale = Vector3.one * float.Parse(anim[2]);
            }

            isPositionAnim = anim[0] == "up" || anim[0] == "down";
            if (isPositionAnim)
            {
                if (anim[0] == "up")
                {
                    initialPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y - 50f,
                        rectTransform.localPosition.z);
                }
                else if (anim[0] == "down")
                {
                    initialPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + 50f,
                        rectTransform.localPosition.z);
                }

                targetPosition = rectTransform.localPosition;
                rectTransform.localPosition = initialPosition;
            }
        }

        float percent = 0f;
        while (percent < 1f)
        {
            if (IsInterrupted)
            {
                break;
            }

            percent += Time.unscaledDeltaTime / time;

            if (isScaleAnim)
            {
                image.rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, percent);
            }

            if (isPositionAnim)
            {
                rectTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, percent);
            }

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, percent);

            yield return null;
        }

        if (IsInterrupted)
        {
            yield break;
        }

        if (isScaleAnim)
        {
            image.rectTransform.localScale = targetScale;
        }

        if (isPositionAnim)
        {
            rectTransform.localPosition = targetPosition;
        }

        canvasGroup.alpha = 1f;

        yield break;
    }

    public void FadeOut(CanvasGroup canvasGroup, float time, string animation = "none")
    {
        StartCoroutine(ProcessFadeOut(canvasGroup, time, animation));
    }

    private IEnumerator ProcessFadeOut(CanvasGroup canvasGroup, float time, string animation = "none")
    {
        if (canvasGroup == null || IsInterrupted)
        {
            yield break;
        }

        Image image = canvasGroup.GetComponent<Image>();
        RectTransform rectTransform = canvasGroup.GetComponent<RectTransform>();

        bool isScaleAnim = false;
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = Vector3.one;

        bool isPositionAnim = false;
        Vector3 initialPosition = Vector3.one;
        Vector3 targetPosition = Vector3.one;

        if (animation != "none")
        {
            string[] anim = animation.Split(" ");

            isScaleAnim = anim[0] == "bigger" || anim[0] == "smaller";
            if (isScaleAnim)
            {
                initialScale = Vector3.one * float.Parse(anim[1]);
                targetScale = Vector3.one * float.Parse(anim[2]);
            }

            isPositionAnim = anim[0] == "up" || anim[0] == "down";
            if (isPositionAnim)
            {
                if (anim[0] == "up")
                {
                    targetPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + 50f,
                        rectTransform.localPosition.z);
                }
                else if (anim[0] == "down")
                {
                    targetPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y - 50f,
                        rectTransform.localPosition.z);
                }

                initialPosition = rectTransform.localPosition;
            }
        }

        float percent = 0f;
        while (percent < 1f)
        {
            if (IsInterrupted)
            {
                break;
            }

            percent += Time.unscaledDeltaTime / time;

            if (isScaleAnim)
            {
                image.rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, percent);
            }

            if (isPositionAnim)
            {
                rectTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, percent);
            }

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, percent);

            yield return null;
        }

        if (IsInterrupted)
        {
            yield break;
        }

        if (isScaleAnim)
        {
            image.rectTransform.localScale = targetScale;
        }

        if (isPositionAnim)
        {
            rectTransform.localPosition = initialPosition;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);

        yield break;
    }

    public void FadeIn(Image image, float time, string animation = "none")
    {
        StartCoroutine(ProcessFadeIn(image, time, animation));
    }

    private IEnumerator ProcessFadeIn(Image image, float time, string animation = "none")
    {
        if (image == null || IsInterrupted)
        {
            yield break;
        }

        image.color = image.color.WithA(0f);
        image.gameObject.SetActive(true);

        RectTransform rectTransform = image.GetComponent<RectTransform>();

        bool isScaleAnim = false;
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = Vector3.one;

        bool isPositionAnim = false;
        Vector3 initialPosition = Vector3.one;
        Vector3 targetPosition = Vector3.one;

        if (animation != "none")
        {
            string[] anim = animation.Split(" ");

            isScaleAnim = anim[0] == "bigger" || anim[0] == "smaller";
            if (isScaleAnim)
            {
                initialScale = Vector3.one * float.Parse(anim[1]);
                targetScale = Vector3.one * float.Parse(anim[2]);
            }

            isPositionAnim = anim[0] == "up" || anim[0] == "down";
            if (isPositionAnim)
            {
                if (anim[0] == "up")
                {
                    initialPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y - 50f,
                        rectTransform.localPosition.z);
                }
                else if (anim[0] == "down")
                {
                    initialPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + 50f,
                        rectTransform.localPosition.z);
                }

                targetPosition = rectTransform.localPosition;
                rectTransform.localPosition = initialPosition;
            }
        }

        float percent = 0f;
        while (percent < 1f)
        {
            if (IsInterrupted)
            {
                break;
            }

            percent += Time.unscaledDeltaTime / time;

            if (isScaleAnim)
            {
                image.rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, percent);
            }

            if (isPositionAnim)
            {
                rectTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, percent);
            }

            image.color = image.color.WithA(Mathf.Lerp(0f, 1f, percent));

            yield return null;
        }

        if (IsInterrupted)
        {
            yield break;
        }

        if (isScaleAnim)
        {
            image.rectTransform.localScale = targetScale;
        }

        if (isPositionAnim)
        {
            rectTransform.localPosition = targetPosition;
        }

        image.color = image.color.WithA(1f);

        yield break;
    }

    public void FadeOut(Image image, float time, string animation = "none")
    {
        StartCoroutine(ProcessFadeOut(image, time, animation));
    }

    private IEnumerator ProcessFadeOut(Image image, float time, string animation = "none")
    {
        if (image == null || IsInterrupted)
        {
            yield break;
        }

        RectTransform rectTransform = image.GetComponent<RectTransform>();

        bool isScaleAnim = false;
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = Vector3.one;

        bool isPositionAnim = false;
        Vector3 initialPosition = Vector3.one;
        Vector3 targetPosition = Vector3.one;

        if (animation != "none")
        {
            string[] anim = animation.Split(" ");

            isScaleAnim = anim[0] == "bigger" || anim[0] == "smaller";
            if (isScaleAnim)
            {
                initialScale = Vector3.one * float.Parse(anim[1]);
                targetScale = Vector3.one * float.Parse(anim[2]);
            }

            isPositionAnim = anim[0] == "up" || anim[0] == "down";
            if (isPositionAnim)
            {
                if (anim[0] == "up")
                {
                    targetPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + 50f,
                        rectTransform.localPosition.z);
                }
                else if (anim[0] == "down")
                {
                    targetPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y - 50f,
                        rectTransform.localPosition.z);
                }

                initialPosition = rectTransform.localPosition;
            }
        }

        float percent = 0f;
        while (percent < 1f)
        {
            if (IsInterrupted)
            {
                break;
            }

            percent += Time.unscaledDeltaTime / time;

            if (isScaleAnim)
            {
                image.rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, percent);
            }

            if (isPositionAnim)
            {
                rectTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, percent);
            }

            image.color = image.color.WithA(Mathf.Lerp(1f, 0f, percent));

            yield return null;
        }

        if (IsInterrupted)
        {
            yield break;
        }

        if (isScaleAnim)
        {
            image.rectTransform.localScale = targetScale;
        }

        if (isPositionAnim)
        {
            rectTransform.localPosition = initialPosition;
        }

        image.color = image.color.WithA(0f);
        image.gameObject.SetActive(false);

        yield break;
    }

    public void ShowToast(string msg, float time, Color color = default)
    {
        StartCoroutine(ProcessShowToast(msg, time, color));
    }

    private IEnumerator ProcessShowToast(string msg, float time, Color color = default)
    {
        if (color == default)
        {
            color = Color.white;
        }

        for (int i = 0; i < toasts.Count; i++)
        {
            if (toasts[i].gameObject.activeSelf && toasts[i].IsShowing)
            {
                toasts[i].Hide();
            }
        }

        Toast toast = null;
        for (int i = 0; i < toasts.Count; i++)
        {
            if (!toasts[i].gameObject.activeSelf)
            {
                toast = toasts[i];
                break;
            }
        }

        if (toast == null)
        {
            toast = Instantiate(toastPrefab);
            DontDestroyOnLoad(toast.gameObject);
            toasts.Add(toast);
        }

        toast.Show(msg, color);

        yield return new WaitForSecondsRealtime(time);

        toast.Hide();

        yield break;
    }
}