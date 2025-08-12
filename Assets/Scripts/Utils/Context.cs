using Newtonsoft.Json;
using System;

public abstract class Context
{
    [JsonIgnore]
    public abstract string FileName { get; }

    public abstract void Init();
    public abstract void Save();
}
