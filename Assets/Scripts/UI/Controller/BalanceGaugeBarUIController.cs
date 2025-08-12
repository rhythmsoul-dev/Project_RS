using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BalanceGaugeBarUIController : UIControllerBase
{
    [SerializeField] private GameObject BalanceGaugeBarUI;
    [SerializeField] private Slider BalanceGaugeBarUISlider;
    private PlayerStats playerStats;
    [Header("보간 시간 지정")]
    [SerializeField] private float duration = 0.5f;
    
    protected override void SetUp()
    {
        playerStats = (PlayerManager.Instance().LocalPlayer != null) switch
        {
            true => PlayerManager.Instance().LocalPlayer.Stats,
            false => PlayerManager.Instance().LocalContext.Stats
        };

        SetBalanceGauge(playerStats.CurBalanceGauge);
        playerStats.ObservableBalanceGauge.Changed += SetBalanceGauge;
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.ObservableBalanceGauge.Changed -= SetBalanceGauge;
        }
    }

    public override void Show()
    {
        BalanceGaugeBarUI.SetActive(true);
    }

    public override void Hide()
    {
        BalanceGaugeBarUI.SetActive(false);
    }

    private Coroutine balanceCoroutine;

    private void SetBalanceGauge(int value)
    {
        BalanceGaugeBarUISlider.maxValue = playerStats.TotalBalance;

        if (balanceCoroutine != null)
            StopCoroutine(balanceCoroutine);

        balanceCoroutine = StartCoroutine(LerpSliderValue(BalanceGaugeBarUISlider, value));
    }
    
    private IEnumerator LerpSliderValue(Slider slider, float targetValue)
    {
        float startValue = slider.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null;
        }

        slider.value = targetValue;
    }
}
