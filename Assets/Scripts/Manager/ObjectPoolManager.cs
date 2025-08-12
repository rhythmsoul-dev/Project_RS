using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable]
    private class ObjectInfo
    {
        public string ObjectName;
        public GameObject Prefab;
        public int Count;
    }

    public bool IsReady { get; private set; }

    [SerializeField] private ObjectInfo[] _objectInfos;

    private string _objName;
    private Dictionary<string, IObjectPool<GameObject>> _ojbectPools = new Dictionary<string, IObjectPool<GameObject>>();
    private Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();

    protected override bool CheckDontDestroyOnLoad()
    {
        return false;
    }

    protected override void OnAwake()
    {
        Init();
    }

    private void Init()
    {
        IsReady = false;
        for (int i = 0; i < _objectInfos.Length; i++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, _objectInfos[i].Count, _objectInfos[i].Count);

            if (_objects.ContainsKey(_objectInfos[i].ObjectName))
            {
                Debug.LogWarningFormat("이미 등록된 오브젝트입니다: {0}", _objectInfos[i].ObjectName);
                return;
            }

            _objects.Add(_objectInfos[i].ObjectName, _objectInfos[i].Prefab);
            _ojbectPools.Add(_objectInfos[i].ObjectName, pool);

            for (int j = 0; j < _objectInfos[i].Count; j++)
            {
                _objName = _objectInfos[i].ObjectName;
                PoolObject poolAbleObj = CreatePooledItem().GetComponent<PoolObject>();
                poolAbleObj.Pool.Release(poolAbleObj.gameObject);
            }
        }

        IsReady = true;
    }

    private GameObject CreatePooledItem()
    {
        GameObject poolObj = Instantiate(_objects[_objName]);
        poolObj.GetComponent<PoolObject>().Pool = _ojbectPools[_objName];
        return poolObj;
    }

    private void OnTakeFromPool(GameObject poolObj)
    {
        poolObj.SetActive(true);
        poolObj.GetComponent<PoolObject>().OnGet();
    }

    private void OnReturnedToPool(GameObject poolObj)
    {
        poolObj.GetComponent<PoolObject>().OnReleased();
        poolObj.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolObj)
    {
        poolObj.GetComponent<PoolObject>().OnDestroyed();
        Destroy(poolObj);
    }

    /// <summary>
    /// 등록된 풀에서 오브젝트를 가져옵니다.
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public GameObject Get(string poolName)
    {
        _objName = poolName;
        if (!_objects.ContainsKey(poolName))
        {
            Debug.LogWarningFormat("오브젝트 풀에 등록되지 않은 오브젝트입니다: {0}", poolName);
            return null;
        }

        return _ojbectPools[poolName].Get();
    }

    public GameObject Get(string poolName, Vector3 position, Quaternion rotation)
    {
        _objName = poolName;
        if (!_objects.ContainsKey(poolName))
        {
            Debug.LogWarningFormat("오브젝트 풀에 등록되지 않은 오브젝트입니다: {0}", poolName);
            return null;
        }

        GameObject obj = _ojbectPools[poolName].Get();
        obj.transform.SetPositionAndRotation(position, rotation);

        return obj;
    }
}
