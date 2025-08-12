using Newtonsoft.Json;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[Serializable]
public class SheetMusic
{
    public SheetMusic()
    {
    }

    public SheetMusic(SheetMusicData data)
    {
        Id = data.Id;
        Name = data.Name;
        Description = data.Description;
        Type = data.Type; // Base, Mid, Pro
        AttackBonus = data.AttackBonus;
        HealthBonus = data.HealthBonus;
        BalanceBonus = data.BalanceGaugeBonus;
        Price = data.Price;
        IconPath = data.IconPath;

        icon = null;
    }

    [JsonProperty("id")]
    public int Id;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("description")]
    public string Description;

    [JsonProperty("type")]
    public String Type;

    [JsonProperty("attack_bonus")]
    public int AttackBonus;

    [JsonProperty("health_bonus")]
    public int HealthBonus;

    [JsonProperty("balance_bonus")]
    public int BalanceBonus;

    [JsonProperty("price")]
    public long Price;

    [JsonProperty("icon_path")]
    public string IconPath;

    private Sprite icon;

    public void LoadIcon(Action<Sprite> onLoaded = null)
    {
        if (icon != null)
        {
            onLoaded?.Invoke(icon);
            return;
        }

        Addressables.LoadAssetAsync<Sprite>(IconPath).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                icon = handle.Result;
                onLoaded?.Invoke(icon);
            }
            else
            {
                Debug.LogErrorFormat("[Addressable] Cannot Found Icon Path: {0}", IconPath);
            }
        };
    }
}
