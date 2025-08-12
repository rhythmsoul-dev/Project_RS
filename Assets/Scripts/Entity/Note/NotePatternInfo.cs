using Newtonsoft.Json;
using System;
using UnityEngine;

/// <summary>
/// 패턴 속 노트의 정보 (같은 id의 노트라도 패턴에 따라 시작 딜레이가 달라질 수 있으므로 따로 관리)
/// </summary>
[Serializable]
public struct NoteInfoInPattern
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("index")]
    public int Index;

    [JsonProperty("delay")]
    public int Delay;
}

[Serializable]
public struct NotePatternInfo
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("description")]
    public string Description;

    [JsonProperty("delay")]
    public int Delay;

    [JsonProperty("note_infos")]
    public NoteInfoInPattern[] NoteInfos;
}
