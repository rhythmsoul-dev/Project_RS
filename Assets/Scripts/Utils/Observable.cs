using System;
using System.Collections.Generic;

public class Observable<T>
{
    public Action<T> Changed;

    private T value;
    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            if (comparer.Equals(this.value, value))
            {
                return;
            }

            this.value = value;
            Changed?.Invoke(this.value);
        }
    }

    private static readonly EqualityComparer<T> comparer = EqualityComparer<T>.Default;

    public Observable()
    {
        value = default;
    }

    public Observable(T value)
    {
        this.value = value;
    }

    public static implicit operator T(Observable<T> value)
    {
        return value.Value;
    }
}
