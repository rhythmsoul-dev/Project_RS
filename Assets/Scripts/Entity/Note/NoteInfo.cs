using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using UnityEngine;

public enum NoteSizeType
{
    Small,
    Normal,
    Big
}

public enum NoteSpawnType
{
    Fade,
    RightToLeft,
    UpToDown,
    DownToUp
}

public enum NoteMoveType
{
    Linear,
    Curve
}

[Serializable]
public struct NoteInfo
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("description")]
    public string Description;

    [JsonProperty("speed")]
    public float Speed;

    [JsonProperty("size_type"), JsonConverter(typeof(StringEnumConverter))]
    public NoteSizeType SizeType;

    [JsonProperty("spawn_type"), JsonConverter(typeof(StringEnumConverter))]
    public NoteSpawnType SpawnType;

    [JsonProperty("move_type"), JsonConverter(typeof(StringEnumConverter))]
    public NoteMoveType MoveType;

    [JsonProperty("sprite_path")]
    public string SpritePath;

    [JsonProperty("target_positions")]
    public Vector2[] TargetPositions;
}
