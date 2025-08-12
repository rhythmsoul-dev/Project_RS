using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PotionUIController : UIControllerBase
{
    [SerializeField] private GameObject potionUI;
    [SerializeField] private TMP_Text potionText;
    private PlayerStats playerStats;
    protected override void SetUp()
    {
        playerStats = (PlayerManager.Instance().LocalPlayer != null) switch
        {
            true => PlayerManager.Instance().LocalPlayer.Stats,
            false => PlayerManager.Instance().LocalContext.Stats
        };

        SetPotionCount(playerStats.PotionCount);
        playerStats.ObservablePotionCount.Changed += SetPotionCount;
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.ObservablePotionCount.Changed -= SetPotionCount;
        }
    }

    public override void Show()
    {
        potionUI.SetActive(true);
    }

    public override void Hide()
    {
        potionUI.SetActive(false);
    }

    private void SetPotionCount(int count)
    {
        potionText.SetText(count.ToString()); 
    }
}
