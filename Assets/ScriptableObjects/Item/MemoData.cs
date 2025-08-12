using UnityEngine;

[CreateAssetMenu(fileName = "MemoData_", menuName = "Scriptable Objects/Memo Data")]
public class MemoData : ScriptableObject
{
    [TextArea] public string Text;
    public string ImagePath;
    public string MemoId;
}
