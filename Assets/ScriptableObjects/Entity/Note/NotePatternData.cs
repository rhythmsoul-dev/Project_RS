using UnityEngine;

[CreateAssetMenu(fileName = "NotePatternData_", menuName = "Scriptable Objects/Note Pattern Data")]
public class NotePatternData : ScriptableObject
{
    public int Id;
    public string Description;
    public float Delay;
}
