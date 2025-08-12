using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class TutorialContext : Context
{
    [JsonIgnore]
    public override string FileName => "tutorial.json";

    public override void Init()
    {
        if (Completed == null)
        {
            Completed = new HashSet<TutorialType>();
        }
    }

    public override void Save()
    {
        SaveLoadManager.Save(this);
    }

    [JsonProperty("completed")]
    public HashSet<TutorialType> Completed;
}
