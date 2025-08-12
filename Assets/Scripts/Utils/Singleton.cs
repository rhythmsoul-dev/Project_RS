using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool hasInstance;

    protected bool IsUsed = false;

    private void Awake()
    {
        CheckDuplication();
        if (IsUsed && CheckDontDestroyOnLoad())
        {
            DontDestroyOnLoad(gameObject);
        }

        OnAwake();
    }

    private void OnDestroy()
    {
        if (IsUsed)
        {
            hasInstance = false;
            instance = (T)((object)null);
        }

        OnDestroyed();
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnDestroyed()
    {

    }

    protected virtual bool CheckDontDestroyOnLoad()
    {
        return true;
    }

    public void CheckDuplication()
    {
        Singleton<T> singleton = this;
        T[] array = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (array.Length >= 2)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Singleton<T> singleton2 = array[i] as Singleton<T>;
                if (singleton2.IsUsed)
                {
                    singleton = singleton2;
                    break;
                }

                if (this != singleton2)
                {
                    Destroy(singleton2.gameObject);
                }
            }
        }

        if (singleton != this)
        {
            Destroy(gameObject);
        }

        if (instance == null)
        {
            instance = (singleton as T);
            hasInstance = instance != null;
            singleton.IsUsed = true;
        }
    }

    private static void FindOrCreateInstance()
    {
        T[] array = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (array.Length >= 1)
        {
            SetInstance(array[0]);
        }
        else
        {
            Create(typeof(T).ToString());
        }
    }

    private static void FindInstance()
    {
        T[] array = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (array.Length >= 1)
        {
            SetInstance(array[0]);
        }
    }

    protected static void SetInstance(T instance)
    {
        Singleton<T>.instance = instance;
        hasInstance = Singleton<T>.instance != null;
        Singleton<T> singleton = Singleton<T>.instance as Singleton<T>;
        singleton.IsUsed = true;
    }

    public static bool Exist()
    {
        if (!hasInstance)
        {
            FindInstance();
        }

        return hasInstance;
    }

    public static T Create(string name)
    {
        if (hasInstance)
        {
            return instance;
        }

        GameObject gameObject = new GameObject(name);
        if (!Application.isPlaying)
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        T t = gameObject.AddComponent<T>();
        SetInstance(t);

        return t;
    }

    public static T Create(GameObject prefab)
    {
        if (hasInstance)
        {
            return instance;
        }

        GameObject gameObject = Instantiate(prefab);
        if (!Application.isPlaying)
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        T t = gameObject.GetComponent<T>();
        SetInstance(t);

        return t;
    }

    public static T Instance()
    {
        if (!hasInstance)
        {
            FindOrCreateInstance();
        }

        return instance;
    }
}
