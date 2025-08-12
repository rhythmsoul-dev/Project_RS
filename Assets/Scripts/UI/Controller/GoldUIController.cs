using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GoldUIController : UIControllerBase
{
    [SerializeField] private GameObject goldUI;
    [SerializeField] private TMP_Text goldText;
    private Tween goldTween;
    private PlayerStats playerStats;

    protected override void SetUp()
    {        
        playerStats = (PlayerManager.Instance().LocalPlayer != null) switch
        {
            true => PlayerManager.Instance().LocalPlayer.Stats,
            false => PlayerManager.Instance().LocalContext.Stats
        };

        InstancySetGoldText(playerStats.Gold);
        playerStats.ObservableGoldAmount.Changed += SetGoldText;
    }
    
    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.ObservableGoldAmount.Changed -= SetGoldText;
        }
    }

    public override void Show()
    {
        goldUI.SetActive(true);
    }

    public override void Hide()
    {
        goldUI.SetActive(false);
    }

    private void InstancySetGoldText(long gold)
    {
        goldText.SetText(gold + " G");
    }
    
    private void SetGoldText(long count)
    {
        long currentCount = long.Parse(goldText.text.Replace(" G", ""));
        
        if (goldTween != null && goldTween.IsActive()) 
            goldTween.Kill();
        
        goldTween = DOVirtual.Int((int)currentCount, (int)count, 1f, value =>
        {
            goldText.SetText(value + " G");
        });
    }
}
