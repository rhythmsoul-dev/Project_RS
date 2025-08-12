using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class EntityStats
{
    public EntityStats()
    {
    }

    public EntityStats(EntityInfo info)
    {
        MaxHealth = info.BaseMaxHealth;
        CurHealth = MaxHealth;
        AttackSpeed = info.BaseAttackSpeed;
        AttackPower = info.BaseAttackPower;
        BalanceAttackPower = info.BaseBalanceAttackPower;
        DefensePower = info.BaseDefensePower;
        MaxBalanceGauge = info.BaseMaxBalanceGauge;
        CurBalanceGauge = MaxBalanceGauge;
        MoveSpeed = info.BaseMoveSpeed;
        Gold = info.Gold;
    }

    [JsonProperty("max_health")]
    public int MaxHealth { get; set; }

    [JsonIgnore]
    public Observable<int> ObservableCurHealth = new Observable<int>();

    [JsonProperty("cur_health")]
    public int CurHealth
    {
        get
        {
            return ObservableCurHealth.Value;
        }
        set
        {
            SetHealth(value);
        }
    }

    [JsonProperty("attack_speed")]
    public float AttackSpeed { get; set; }

    [JsonProperty("attack_power")]
    public float AttackPower { get; set; }

    [JsonProperty("balance_attack_power")]
    public float BalanceAttackPower { get; set; }

    [JsonProperty("defense_power")]
    public float DefensePower { get; set; }

    /// <summary>
    /// 체간. 체력 50% 일 때 체간의 데미지 감소율이 0%
    /// </summary>
    [JsonProperty("max_balance")]
    public int MaxBalanceGauge { get; set; }

    [JsonIgnore]
    public Observable<int> ObservableBalanceGauge = new Observable<int>();

    [JsonProperty("cur_balance")]
    public int CurBalanceGauge
    {
        get
        {
            return ObservableBalanceGauge.Value;
        }
        set
        {
            SetBalanceGauge(value);
        }
    }

    [JsonProperty("move_speed")]
    public float MoveSpeed { get; set; }

    [JsonProperty("gold")]
    public long Gold {
        get
        {
            return ObservableGoldAmount.Value;
        }
        set
        {
            ObservableGoldAmount.Value = value;
        } 
    }
    
    [JsonIgnore]
    public Observable<long> ObservableGoldAmount = new Observable<long>();

    public void UpdateStats(EntityInfo info)
    {
        MaxHealth = info.BaseMaxHealth;
        CurHealth = MaxHealth;
        AttackSpeed = info.BaseAttackSpeed;
        AttackPower = info.BaseAttackPower;
        BalanceAttackPower = info.BaseBalanceAttackPower;
        DefensePower = info.BaseDefensePower;
        MaxBalanceGauge = info.BaseMaxBalanceGauge;
        CurBalanceGauge = MaxBalanceGauge;
        MoveSpeed = info.BaseMoveSpeed;
        Gold = info.Gold;
    }

    protected virtual void SetHealth(int value)
    {
        if (value >= MaxHealth)
        {
            ObservableCurHealth.Value = MaxHealth;
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

    protected virtual void SetBalanceGauge(int value)
    {
        if (value >= MaxBalanceGauge)
        {
            ObservableBalanceGauge.Value = MaxBalanceGauge;
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
}
