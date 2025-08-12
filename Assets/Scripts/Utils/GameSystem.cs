using System;
using UnityEngine;

public abstract class GameSystem<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool hasInstance;
    private static T instance;

    private void OnDestroy()
    {
        instance = (T)((object)null);
        hasInstance = false;

        OnDestroyed();
    }

    protected virtual void OnDestroyed()
    {

    }

    public static T Instance()
    {
        if (!hasInstance)
        {
            GameObject gameObject = new GameObject(typeof(T).ToString());
            T instance = gameObject.AddComponent<T>();

            GameSystem<T>.instance = instance;
            hasInstance = (GameSystem<T>.instance != null);
        }

        return instance;
    }

    public static bool Exist()
    {
        return hasInstance;
    }
}
