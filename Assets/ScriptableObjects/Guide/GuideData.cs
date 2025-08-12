using System;
using System.Collections.Generic;
using UnityEngine;

public enum GuideType
{
    NormalGuide,
    BossGuide,
    MemoGuide,
    AfterDialogueGuide,
}

[Serializable]
public struct GuideStruct
{
    public string Name;
    public GuideType Type;
    public int Priority;
    public int BossId;
    public string MemoId;
    public string DialogueId;
    [TextArea] public string Text;
}

[CreateAssetMenu(fileName = "GuideList", menuName = "Scriptable Objects/Guide List")]
public class GuideData : ScriptableObject
{
    public List<GuideStruct> GuideList;
}
