using UnityEngine;
using UnityEngine.Pool;

public class PoolObject : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    public GameObject Get()
    {
        return Pool.Get();
    }

    public void Release()
    {
        Pool.Release(gameObject);
    }

    public virtual void OnGet()
    {

    }

    public virtual void OnReleased()
    {

    }

    public virtual void OnDestroyed()
    {

    }
}