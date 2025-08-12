using System;
using System.Collections;
using System.Collections.Generic;

public class ObservableList<T> : IList<T>
{
    private readonly List<T> list = new List<T>();
    private static readonly EqualityComparer<T> comparer = EqualityComparer<T>.Default;

    public Action<List<T>> Changed;

    public T this[int index]
    {
        get
        {
            return list[index];
        }
        set
        {
            if (!comparer.Equals(list[index], value))
            {
                list[index] = value;
                OnChanged();
            }
        }
    }

    public int Count => list.Count;

    public bool IsReadOnly => false;

    private void OnChanged()
    {
        Changed?.Invoke(new List<T>(list));
    }

    public int IndexOf(T item)
    {
        return list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        T prev = list[index];
        list.Insert(index, item);
        if (!comparer.Equals(prev, list[index]))
        {
            OnChanged();
        }
    }

    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
        OnChanged();
    }

    public void Add(T item)
    {
        list.Add(item);
        OnChanged();
    }

    public void Clear()
    {
        list.Clear();
        OnChanged();
    }

    public bool Contains(T item)
    {
        return list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        bool result = list.Remove(item);
        if (result)
        {
            OnChanged();
        }

        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void SetList(IEnumerable<T> items, bool initial = false)
    {
        list.Clear();
        list.AddRange(items);
        if (!initial)
        {
            OnChanged();
        }
    }
}
