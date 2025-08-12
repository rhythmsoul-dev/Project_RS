using Michsky.UI.Dark;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatUIController : UIControllerBase
{
    [Header("오브젝트")]
    [SerializeField] private GameObject combatUIObject;
    [SerializeField] private Canvas backgroundCanvas;
    [SerializeField] private Canvas combatCanvas;
    [SerializeField] private Canvas blendCanvas;
    [SerializeField] private Canvas resultCanvas;
    [SerializeField] private UIDissolveEffect fadeEffect;
    [SerializeField] private UIDissolveEffect blendEffect;
    [SerializeField] private UIDissolveEffect resultEffect;
    
    public Canvas ResultCanvas => resultCanvas;

    [Header("배경 이미지들")]
    [SerializeField] private Sprite graveyardBackground;
    [SerializeField] private Sprite dungeonBackground;
    
    [Header("전투")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image enemyImage;
    [SerializeField] private Image enemyImagePrev;
    
    public Image EnemyImage => enemyImage;

    [SerializeField] private Slider enemyHpBar;
    [SerializeField] private Slider enemyHpDamagedBar;
    [SerializeField] private Slider enemyBalanceBar;
    [SerializeField] private Slider enemyBalanceDamagedBar;

    [SerializeField] private Slider playerHpBar;
    [SerializeField] private Slider playerHpDamagedBar;
    [SerializeField] private Slider playerBalanceBar;
    [SerializeField] private Slider playerBalanceDamagedBar;

    [SerializeField] private Button potionButton;
    [SerializeField] private TMP_Text potionCountText;
    [SerializeField] private Image vignette;
    [SerializeField] private Image groggyEffect;
    private Tween vignetteTween;
    private Tween groggyTween;
    
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text combo;
    private Tween comboTween;
    private Tween textTween;
    private float comboFontSize;

    [SerializeField] private LifeSlots lifeSlots;

    [Header("결과")]
    [SerializeField] private TMP_Text earnedGoldText;
    [SerializeField] private TMP_Text curGoldText;
    [SerializeField] private Button backButton;

    public UIDissolveEffect BlendEffect => blendEffect;
    public UIDissolveEffect FadeEffect => fadeEffect;

    Tween eHpTween, eBalTween, eHpDamagedTween, eBalDamagedTween;
    Tween pHpTween, pBalTween, pHpDamagedTween, pBalDamagedTween;

    private Player Player => PlayerManager.Instance().LocalPlayer;
    private Enemy enemy;

    private void Awake()
    {
        fadeEffect.location = 1f;
        fadeEffect.gameObject.SetActive(false);
        fadeEffect.DissolveOutOver = () => fadeEffect.gameObject.SetActive(false);

        blendEffect.location = 1f;
        blendEffect.gameObject.SetActive(false);
        blendEffect.DissolveOutOver = () => blendEffect.gameObject.SetActive(false);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => StartCoroutine(ProcessBackToAdventrue()));

        potionButton.onClick.RemoveAllListeners();
        potionButton.onClick.AddListener(() => Player.Stats.UsePotion());

        switch (SceneLoader.Instance().CurMapType)
        {
            case MapType.Graveyard:
                backgroundImage.sprite = graveyardBackground;
                break;
            case MapType.Dungeon:
                backgroundImage.sprite = dungeonBackground;
                break;
            default:
                break;
        }

        Player.UsedPotion += UpdatePotionUI;
    }

    protected override void SetUp()
    {
        blendCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        blendCanvas.worldCamera = CameraManager.Instance().MainCamera;
        blendCanvas.sortingLayerName = "Combat";

        backgroundCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        backgroundCanvas.worldCamera = CameraManager.Instance().MainCamera;
        backgroundCanvas.sortingLayerName = "CombatUI";
        //backgroundCanvas.sortingOrder = 1;
        
        combatCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        combatCanvas.worldCamera = CameraManager.Instance().MainCamera;
        combatCanvas.sortingLayerName = "CombatUI";
        //combatCanvas.sortingOrder = 3;

        resultCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        resultCanvas.worldCamera = CameraManager.Instance().MainCamera;
        resultCanvas.sortingLayerName = "CombatUI";

        comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f);
        comboFontSize = comboText.fontSize;

        UIManager.Instance().RegisterController(this);
    }

    private void OnEnable()
    {
        if (Player != null)
        {
            Player.GroggyEvent += OnPlayerGroggy;
        }
    }

    private void OnDisable()
    {
        if (Player != null)
        {
            Player.GroggyEvent -= OnPlayerGroggy;
        }
    }

    private void OnDestroy()
    {
        vignette.DOKill();
    }

    public void Init(Enemy enemy)
    {
        this.enemy = enemy;

        enemyHpBar.value = 1f;
        enemyBalanceBar.value = 1f;
        playerHpBar.value = 1f;
        playerBalanceBar.value = 1f;

        lifeSlots.Init(enemy.GetTotalPhase());

        UpdatePotionUI(Player.Stats.PotionCount);
    }

    public override void Show()
    {
        combatUIObject.SetActive(true);
    }

    public override void Hide()
    {
        combatUIObject.SetActive(false);
    }

    public void ResetEnemyImage()
    {
        enemyImagePrev.sprite = null;
        enemyImage.sprite = null;
    }

    public void ChangeEnemyImage(Sprite sprite)
    {
        enemyImagePrev.gameObject.SetActive(true);
        enemyImagePrev.color = enemyImagePrev.color.WithA(1f);
        enemyImagePrev.sprite = enemyImage.sprite;

        enemyImage.gameObject.SetActive(true);
        enemyImage.color = enemyImage.color.WithA(0f);
        enemyImage.sprite = sprite;

        UIManager.Instance().FadeIn(enemyImage, 0.5f);
        UIManager.Instance().FadeOut(enemyImagePrev, 0.5f);
    }

    public void UpdateEnemyUI()
    {
        int maxHealth = enemy.Stats.MaxHealth;
        int maxBalance = enemy.Stats.MaxBalanceGauge;
        int curHealth = enemy.Stats.CurHealth;
        int curBalance = enemy.Stats.CurBalanceGauge;

        float hpPct = Mathf.Clamp01((float)curHealth / maxHealth);
        float balPct = Mathf.Clamp01((float)curBalance / maxBalance);

        eHpTween?.Kill();
        eBalTween?.Kill();
        eHpDamagedTween?.Kill();
        eBalDamagedTween?.Kill();

        if (hpPct <= enemyHpBar.value || balPct <= enemyBalanceBar.value)
        {
            eHpTween = enemyHpBar.DOValue(hpPct, 0.5f).SetEase(Ease.OutQuad);
            eBalTween = enemyBalanceBar.DOValue(balPct, 0.5f).SetEase(Ease.OutQuad);

            eHpDamagedTween = enemyHpDamagedBar.DOValue(hpPct, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f);
            eBalDamagedTween = enemyBalanceDamagedBar.DOValue(balPct, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f);
        }
        else
        {
            eHpTween = enemyHpBar.DOValue(hpPct, 1f).SetEase(Ease.OutQuad);
            eBalTween = enemyBalanceBar.DOValue(balPct, 1f).SetEase(Ease.OutQuad);

            eHpDamagedTween = enemyHpDamagedBar.DOValue(hpPct, 1f).SetEase(Ease.OutQuad);
            eBalDamagedTween = enemyBalanceDamagedBar.DOValue(balPct, 1f).SetEase(Ease.OutQuad);
        }
    }

    public void UpdatePlayerUI()
    {
        int maxHealth = Player.Stats.TotalHealth;
        int maxBalance = Player.Stats.TotalBalance;
        int curHealth = Player.Stats.CurHealth;
        int curBalance = Player.Stats.CurBalanceGauge;
        
        float hpPct  = Mathf.Clamp01((float)curHealth / maxHealth);
        float balPct = Mathf.Clamp01((float)curBalance / maxBalance);

        pHpTween?.Kill();
        pBalTween?.Kill();
        pHpDamagedTween?.Kill();
        pBalDamagedTween?.Kill();

        pHpTween  = playerHpBar.DOValue(hpPct, 0.5f).SetEase(Ease.OutQuad);
        pBalTween = playerBalanceBar.DOValue(balPct, 0.5f).SetEase(Ease.OutQuad);
        
        pHpDamagedTween = playerHpDamagedBar.DOValue(hpPct, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f);
        pBalDamagedTween = playerBalanceDamagedBar.DOValue(balPct, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f);
        
    }

    public void UpdatePotionUI(int count)
    {
        potionCountText.SetText(count.ToString());
    }

    public void ShowFadeEffect()
    {
        fadeEffect.gameObject.SetActive(true);
        fadeEffect.DissolveIn();
    }

    public void HideFadeEffect()
    {
        fadeEffect.gameObject.SetActive(true);
        fadeEffect.DissolveOut();
    }

    public void ShowAndHideFadeEffect(float duration)
    {
        ShowFadeEffect();
        Invoke(nameof(HideFadeEffect), duration);
    }

    public void ShowBlendEffect()
    {
        blendEffect.gameObject.SetActive(true);
        blendEffect.DissolveIn();
    }

    public void HideBlendEffect()
    {
        blendEffect.gameObject.SetActive(true);
        blendEffect.DissolveOut();
    }

    public void ShowAndHideBlendEffect(float duration)
    {
        ShowBlendEffect();
        Invoke(nameof(HideBlendEffect), duration);
    }

    public void ShowResult(long earnedGold)
    {
        earnedGoldText.SetText($"획득한 골드: +{earnedGold}G");
        curGoldText.SetText($"현재 골드: {Player.Stats.Gold}G");

        resultCanvas.gameObject.SetActive(true);
    }

    public void HideResult()
    { 
        PlayerManager.Instance().SwitchPlayerInputState(PlayerInputState.Move_State);
        
        resultCanvas.gameObject.SetActive(false);
    }

    private IEnumerator ProcessBackToAdventrue()
    {
        ShowBlendEffect();
        yield return new WaitForSecondsRealtime(0.5f);

        HideResult();
        yield return new WaitForSecondsRealtime(0.3f);

        HideBlendEffect();
        CombatSystem.Instance().BackToAdventrue();

        yield break;
    }

    public void ShowVignetteEffect()
    {
        vignetteTween.Kill();
        vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, 0f);
        vignetteTween = DOTween.Sequence().Append(vignette.DOFade(1f, 0.3f)).AppendInterval(0.1f).Append(vignette.DOFade(0f, 0.3f)).OnComplete(() => vignetteTween = null); 
    }

    public void ComboTextEffect(int comboCnt)
    {
        comboTween?.Kill();
        textTween?.Kill();
        comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f);
        if (comboCnt == 0)
        {
            comboText.SetText("");
            combo.SetText("");
            comboText.fontSize = comboFontSize;
            combo.color = Color.white;
            return;
        }
        comboText.SetText(comboCnt.ToString());
        combo.SetText("Combo");
        if (comboCnt >= 3)
        {
            comboText.fontSize = comboFontSize + (comboCnt * 2);
            comboText.color = Color.red;
        }
        else if (comboCnt >= 2)
        {
            comboText.fontSize = comboFontSize + (comboCnt * 2);
            comboText.color = Color.yellow;
        }
        else if (comboCnt >= 1)
        {
            comboText.fontSize = comboFontSize + (comboCnt * 2);
            comboText.color = Color.white;
        }
        comboTween = DOTween.Sequence().Append(comboText.DOFade(1f, 0.5f)).AppendInterval(0.6f).Append(comboText.DOFade(0f, 0.3f)).OnComplete(() => comboTween = null); 
        textTween = DOTween.Sequence().Append(combo.DOFade(1f, 0.5f)).AppendInterval(0.6f).Append(combo.DOFade(0f, 0.3f)).OnComplete(() => textTween = null); 
    }

    private void OnPlayerGroggy()
    {
        groggyTween?.Kill();
        groggyEffect.color = new Color(groggyEffect.color.r, groggyEffect.color.g, groggyEffect.color.b, 0f);
        groggyTween = DOTween.Sequence().Append(groggyEffect.DOFade(0.4f, 0.3f)).AppendInterval(2.4f).Append(groggyEffect.DOFade(0f, 0.3f)).OnComplete(() => groggyTween = null); 
    }

    public void ChangePhase(Enemy enemy)
    {
        StartCoroutine(ProcessChangedPhase(enemy));
    }

    private IEnumerator ProcessChangedPhase(Enemy enemy)
    {
        if (enemy.GetTotalPhase() <= 1)
        {
            yield break;
        }

        enemyImage.gameObject.SetActive(false);
        enemyImagePrev.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        enemyImage.gameObject.SetActive(true);
        enemyImagePrev.gameObject.SetActive(true);

        lifeSlots.UpdateSlots(enemy.GetCurPhase());

        yield break;
    }
}
