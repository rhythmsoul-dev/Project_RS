using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ComponentExpensions
{
    // Extension: Transform
    public static T FindChildByName<T>(this Transform trans, string name) where T : Component
    {
        // 비활성화된 것까지 전부 
        T[] children = trans.GetComponentsInChildren<T>(true);
        foreach (T child in children)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    public static GameObject FindChildByName(this Transform trans, string name)
    {
        Transform[] children = trans.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }
    
    // 리플렉션
    public static List<Component> GetSerializedComponents(MonoBehaviour target)
    {
        List<Component> result = new List<Component>();

        var fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // [SerializeField] 속성 확인
            if (field.IsDefined(typeof(SerializeField), true))
            {
                var value = field.GetValue(target);

                if (value is Component comp)
                {
                    result.Add(comp);
                }
            }
        }

        return result;
    }

    public static List<GameObject> GetSerializedGameObjects(MonoBehaviour target)
    {
        List<GameObject> result = new List<GameObject>();

        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(SerializeField), true))
            {
                var value = field.GetValue(target);

                if (value is GameObject go)
                {
                    result.Add(go);
                }
            }
        }

        return result;
    }
}
