using System;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    Town = 0,
    Graveyard,
    Dungeon,
    Tutorial,
}

[Serializable]
public class ActivePortalData
{
    public MapType MapType;
    public int MapIndex;
    public int PortalIndex;
    public bool IsFirstActiveOver;

    public ActivePortalData() { }

    public ActivePortalData(MapType mapType, int mapIndex, int portalIndex, bool isFirstActiveOver = false)
    {
        MapType = mapType;
        MapIndex = mapIndex;
        PortalIndex = portalIndex;
        IsFirstActiveOver = isFirstActiveOver;
    }

    public void SetData(MapType mapType, int mapIndex, int portalIndex)
    {
        MapType = mapType;
        MapIndex = mapIndex;
        PortalIndex = portalIndex;
    }
    
    public bool Equals(ActivePortalData other)
    {
        return MapType == other.MapType &&
               MapIndex == other.MapIndex &&
               PortalIndex == other.PortalIndex;
    }
    
    public override bool Equals(object obj)
    {
        return obj is ActivePortalData other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + MapType.GetHashCode();
            hash = hash * 23 + MapIndex.GetHashCode();
            hash = hash * 23 + PortalIndex.GetHashCode();
            return hash;
        }
    }
}

[Serializable]
public struct PortalLocation
{
    public int PortalIndex;
    public Vector3 Location;

    public PortalLocation(int index, Vector3 location, bool isActive)
    {
        PortalIndex = index;
        Location = location;
    }
}

[CreateAssetMenu(fileName = "MapPortalData", menuName = "Scriptable Objects/Map Portal Data")]
public class MapPortalData : ScriptableObject
{
    [Header("맵 ID")]
    public MapType MapType;
    public int MapIndex;
    
    [Header("포탈 위치들")]
    public List<PortalLocation> PortalLocations = new();
    
    /// <summary>
    /// 포탈 인덱스로 포탈이 있는지 확인하는 함수
    /// </summary>
    /// <param name="index"></param>
    /// <param name="portal"></param>
    /// <returns></returns>
    public bool TryGetPortal(int index, out PortalLocation portal)
    {
        int i = PortalLocations.FindIndex(p => p.PortalIndex == index);
        
        if (i >= 0)
        {
            portal = PortalLocations[i];
            return true;
        }

        portal = default;
        return false;
    }
    
    /// <summary>
    /// 새 포탈 위치 추가 함수
    /// </summary>
    /// <param name="index"></param>
    /// <param name="position"></param>
    /// <param name="isActive"></param>
    public void AddPortal(int index, Vector3 position, bool isActive)
    {
        if (PortalLocations.Exists(p => p.PortalIndex == index))
        {
            Debug.LogWarning($"포탈 인덱스 {index}번째 포탈이 이미 존재합니다.");
            return;
        }

        PortalLocations.Add(new PortalLocation(index, position, isActive));
        
        PortalLocations.Sort((a, b) => a.PortalIndex.CompareTo(b.PortalIndex));
    }
}