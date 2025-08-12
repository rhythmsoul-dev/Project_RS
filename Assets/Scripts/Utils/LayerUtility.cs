using System;
using UnityEngine;

public static class LayerUtility
{
    public static void SetLayer(GameObject obj, int layer, Func<GameObject, bool> filter = null)
    {
        obj.layer = layer;
        Transform transform = obj.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (filter == null || !filter(child.gameObject))
            {
                SetLayer(child.gameObject, layer, filter);
            }
        }
    }
}
