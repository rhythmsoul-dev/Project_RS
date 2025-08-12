using JetBrains.Annotations;
using System;
using UnityEngine;

public static class UnityExtensions
{
    [CanBeNull]
    public static T Nullable<T>(this T obj) where T : UnityEngine.Object
    {
        if (obj != null)
        {
            return obj;
        }

        return null;
    }
}
