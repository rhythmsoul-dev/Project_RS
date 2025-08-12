using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUIController : UIControllerBase
{
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Slider healthBarUISlider;
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

        SetHealthGuage(playerStats.CurHealth);
        playerStats.ObservableCurHealth.Changed += SetHealthGuage;
    }
    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.ObservableCurHealth.Changed -= SetHealthGuage;
        }
    }
    public override void Show()
    {
        healthBarUI.SetActive(true);
    }

    public override void Hide()
    {
        healthBarUI.SetActive(false);
    }

    private Coroutine healthCoroutine;

    private void SetHealthGuage(int value)
    {
        healthBarUISlider.maxValue = playerStats.TotalHealth;

        if (healthCoroutine != null)
            StopCoroutine(healthCoroutine);

        healthCoroutine = StartCoroutine(LerpSliderValue(healthBarUISlider, value));
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
