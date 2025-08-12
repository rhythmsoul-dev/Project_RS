using Newtonsoft.Json;
using UnityEngine;

public class PlayerStats : EntityStats
{
    [JsonIgnore]
    public Observable<int> ObservablePotionCount = new Observable<int>();
    
    [JsonProperty("maxPotion")]
    private int maxPotionCount = 5;
    [JsonProperty("potion")]
    public int PotionCount
    {
        get
        {
            return ObservablePotionCount.Value;
        }
        set
        {
            SetPotionCount(value);
        }
    }
    public int TotalHealth { get; set; }
    public int TotalBalance { get; set; }
    public float TotalAttack { get; set; }
    public PlayerStats()
    {
    }

    public PlayerStats(EntityInfo info) : base(info)
    {
        PotionCount = 0;
        TotalHealth = MaxHealth;
        TotalBalance = MaxBalanceGauge;
        TotalAttack = AttackPower;
        CurHealth = TotalHealth;
        CurBalanceGauge = TotalBalance;
    }

    public void UsePotion()
    {
        if (PotionCount <= 0)
        {
            UIManager.Instance().ShowToast("포션 개수가 부족합니다.", 2f);
            SoundManager.Instance().Play(GameConstants.Sound.POTION_GRAB);
            return;
        }

        if (CurHealth >= TotalHealth && CurBalanceGauge >= TotalBalance)
        {
            UIManager.Instance().ShowToast("체간, 체력이 이미 모두 회복되었습니다.", 2f);
            return;
        }
        SoundManager.Instance().Play(GameConstants.Sound.USE_POTION);
        PotionCount--;
        CurHealth += Mathf.RoundToInt((float)TotalHealth * 0.35f);
        CurBalanceGauge += Mathf.RoundToInt((float)TotalBalance * 0.5f);

        PlayerManager.Instance().LocalPlayer.UsedPotion?.Invoke(PotionCount);
        PlayerManager.Instance().LocalPlayer.OnContextChanged();
    }

    public void RefillPotion()
    {
        PotionCount = maxPotionCount;
        
        PlayerManager.Instance().OnContextChanged();
    }

    private void SetPotionCount(int value)
    {
        if (value > maxPotionCount)
        {
            ObservablePotionCount.Value = maxPotionCount;
        }
        else if (value < 0)
        {
            ObservablePotionCount.Value = 0;
        }
        else
        {
            ObservablePotionCount.Value = value;
        }
    }

    public void SetStats()
    {
        TotalHealth = MaxHealth;
        TotalBalance = MaxBalanceGauge;
        TotalAttack = AttackPower;
        foreach (var sm in PlayerManager.Instance().LocalContext.SheetMusics)
        {
            TotalHealth += sm.HealthBonus;
            TotalBalance += sm.BalanceBonus;
            TotalAttack += sm.AttackBonus;
        }
    }
    protected override void SetHealth(int value)
    {
        if (value >= TotalHealth)
        {
            ObservableCurHealth.Value = TotalHealth;
        }
        else if (value <= 0)
        {
            ObservableCurHealth.Value = 0;
        }
        else
        {
            ObservableCurHealth.Value = value;
        }
    }
    protected override void SetBalanceGauge(int value)
    {
        if (value >= TotalBalance)
        {
            ObservableBalanceGauge.Value = TotalBalance;
        }
        else if (value <= 0)
        {
            ObservableBalanceGauge.Value = 0;
        }
        else
        {
            ObservableBalanceGauge.Value = value;
        }
    }
    
    public void AddMaxPotion(int value)
    {
        maxPotionCount += value;
        SetPotionCount(PotionCount + value);
    }
}
