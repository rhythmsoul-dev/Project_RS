using System;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// 벡터의 x값만 변경하여 새로 생성합니다.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static Vector3 WithX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    /// <summary>
    /// 벡터의 y값만 변경하여 새로 생성합니다.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    /// <summary>
    /// 벡터의 z값만 변경하여 새로 생성합니다.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 WithZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    /// <summary>
    /// 벡터에 스케일을 적용하여 새로 생성합니다.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="scaleZ"></param>
    /// <returns></returns>
    public static Vector3 WithScale(this Vector3 v, float scaleX, float scaleY, float scaleZ)
    {
        return new Vector3(v.x * scaleX, v.y * scaleY, v.z * scaleZ);
    }

    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    /// <summary>
    /// 현재 벡터의 크기가 비교할 벡터의 크기보다 크거나 같은지 확인합니다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsGreaterOrEqual(this Vector3 a, Vector3 b)
    {
        return a.x >= b.x && a.y >= b.y && a.z >= b.z;
    }

    public static Vector2 WithX(this Vector2 v, float x)
    {
        return new Vector2(x, v.y);
    }

    public static Vector2 WithY(this Vector2 v, float y)
    {
        return new Vector2(v.x, y);
    }

    public static Vector2 WithScale(this Vector2 v, float scaleX, float scaleY)
    {
        return new Vector2(v.x * scaleX, v.y * scaleY);
    }

    public static bool IsGreaterOrEqual(this Vector2 a, Vector2 b)
    {
        return a.x >= b.x && a.y >= b.y;
    }
}
