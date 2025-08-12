using System;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class StringExtensions
{
    public static T ToEnum<T>(this string source, T value = default) where T : struct, Enum
    {
        return (!source.TryToEnum(out T t)) ? value : t;
    }

    public static bool TryToEnum<T>(this string source, out T value) where T : struct, Enum
    {
        if (string.IsNullOrEmpty(source))
        {
            value = default;
            return false;
        }

        return Enum.TryParse(source, ignoreCase: true, out value);
    }
}
