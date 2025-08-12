using System.Collections.Generic;
using UnityEngine;

public enum LootType
{
    Gold,
    Memo,
    Sheet,
    Potion,
}

[CreateAssetMenu(fileName = "LootData_", menuName = "Scriptable Objects/Loot Data")]
public class LootData : ScriptableObject
{
    [Header("획득 아이템 종류")]
    public LootType Type;
    public int LootID;
    
    [Header("골드 획등량")]
    public int Gold;
    [Header("획득할 악보")]
    public SheetMusicData SheetMusic;
    [Header("포션 획득량")]
    public int PotionAmount;
    [Header("세계관 설명 메모")]
    public MemoData Memo;
}
