using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

[Serializable]
public class PlayerContext : Context
{
    [JsonIgnore]
    public override string FileName => "player_context.json";

    public override void Init()
    {
        if (Stats == null)
        {
            Stats = new PlayerStats(EntityManager.Instance().GetEntityInfo(1));
        }

        if (SheetMusics == null)
        {
            SheetMusics = new List<SheetMusic>();
        }

        if (Memo == null)
        {
            Memo = new List<Memo>();
        }
        
        if (KilledEnemies == null)
        {
            KilledEnemies = new Dictionary<int, int>();
        }

        if (Dialogues == null)
        {
            Dialogues = new Dictionary<string, bool>();
        }

        if (LootDataIDs == null)
        {
            LootDataIDs = new List<int>();
        }

        if (ActivePortals == null)
        {
            ActivePortals = new List<ActivePortalData>();
            ActivePortals.Add(new ActivePortalData(MapType.Graveyard, 0, 0));
        }
    }

    public override void Save()
    {
        SaveLoadManager.Save(this);
    }
    
    // public static string MakePath()
    // {
    //     string dir = System.IO.Path.Combine(Application.persistentDataPath, "Data");
    //     if (!Directory.Exists(dir))
    //     {
    //         Directory.CreateDirectory(dir);
    //     }
    //
    //     return System.IO.Path.Combine(dir, "player.json");
    // }

    [JsonProperty("stats")]
    public PlayerStats Stats;

    [JsonProperty("sheet_musics")]
    public List<SheetMusic> SheetMusics;
    
    [JsonProperty("memo")]
    public List<Memo> Memo;
    
    [JsonProperty("killed_enemies")] 
    public Dictionary<int, int> KilledEnemies;
    
    [JsonProperty("dialogues")] 
    public Dictionary<string, bool> Dialogues;

    [JsonProperty("death_count")] 
    public int DeathCount;
    
    [JsonProperty("loot_data")]
    public List<int> LootDataIDs;
    
    [JsonProperty("active_portals")]
    public List<ActivePortalData> ActivePortals;
}
