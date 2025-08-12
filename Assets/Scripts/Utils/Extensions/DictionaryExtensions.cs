using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

public static class DictionaryExtensions
{
    /// <summary>
    /// 딕셔너리의 값을 가져옵니다.
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    [CanBeNull]
    public static TV Get<TK, TV>(this IDictionary<TK, TV> source, [CanBeNull] TK key, TV defaultValue = default)
    {
        if (key == null)
        {
            return defaultValue;
        }

        return source.TryGetValue(key, out TV tv) ? tv : defaultValue;
    }

    /// <summary>
    /// 딕셔너리 키-값에 다른 딕셔너리의 키-값을 모두 추가합니다.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
    {
        if (source == null)
        {
            throw new NullReferenceException("source is null");
        }

        if (target == null)
        {
            return;
        }

        foreach (KeyValuePair<TKey, TValue> keyValuePair in target)
        {
            source[keyValuePair.Key] = keyValuePair.Value;
        }
    }
}
