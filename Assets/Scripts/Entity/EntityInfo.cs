using Newtonsoft.Json;
using System;
using UnityEngine;

public struct EntityInfo
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("description")]
    public string Description;

    [JsonProperty("max_health")]
    public int BaseMaxHealth;

    [JsonProperty("attack_power")]
    public float BaseAttackPower;
    
    [JsonProperty("balance_attack_power")]
    public float BaseBalanceAttackPower;

    [JsonProperty("attack_speed")]
    public float BaseAttackSpeed;

    [JsonProperty("defense_power")]
    public float BaseDefensePower;

    [JsonProperty("balance_gauge")]
    public int BaseMaxBalanceGauge;

    [JsonProperty("move_speed")]
    public float BaseMoveSpeed;

    [JsonProperty("gold")]
    public long Gold;

    [JsonProperty("encount_note_id")]
    public int EncountNoteId;

    [JsonProperty("pattern_ids")]
    public int[] PatternIds;
}
