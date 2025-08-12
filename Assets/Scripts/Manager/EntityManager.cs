using System.Collections.Generic;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    /// <summary>
    /// 엔티티 정보 딕셔너리. 키: id, 값: 엔티티 정보
    /// </summary>
    private Dictionary<int, EntityInfo> entityInfos = new Dictionary<int, EntityInfo>();

    /// <summary>
    /// 노트 정보 딕셔너리. 키: id, 값: 노트 정보
    /// </summary>
    private Dictionary<int, NoteInfo> noteInfos = new Dictionary<int, NoteInfo>();

    /// <summary>
    /// 노트 패턴 정보 딕셔너리. 키: id, 값: 패턴 정보
    /// </summary>
    private Dictionary<int, NotePatternInfo> notePatternInfos = new Dictionary<int, NotePatternInfo>();

    public void SetEntityInfo(EntityInfo info)
    {
        entityInfos[info.Id] = info;
    }

    public EntityInfo GetEntityInfo(int id)
    {
        return entityInfos[id];
    }

    public void SetNoteInfo(NoteInfo info)
    {
        noteInfos[info.Id] = info;
    }

    public NoteInfo GetNoteInfo(int id)
    {
        return noteInfos[id];
    }

    public void SetNotePatternInfo(NotePatternInfo info)
    {
        notePatternInfos[info.Id] = info;
    }

    public NotePatternInfo GetNotePatternInfo(int id)
    {
        return notePatternInfos[id];
    }
}
