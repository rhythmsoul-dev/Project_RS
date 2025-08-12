using UnityEngine;

[CreateAssetMenu(fileName = "NoteData_", menuName = "Scriptable Objects/Note Data")]
public class NoteData : ScriptableObject
{
    public int Id;
    public float Speed;
    public NoteSpawnType SpawnType;
    public Vector2[] TargetPositions;
}
