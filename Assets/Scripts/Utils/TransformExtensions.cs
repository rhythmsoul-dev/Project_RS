using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// 지정된 이름을 가진 자식 오브젝트에서 특정 컴포넌트 타입을 찾아 반환.
    /// 비활성화된 오브젝트도 포함하여 검색.
    /// </summary>
    /// <param name="transform">검색을 시작할 기준 Transform</param>
    /// <param name="name">찾고자 하는 자식 오브젝트의 이름</param>
    /// <typeparam name="T">찾고자 하는 컴포넌트의 타입</typeparam>
    /// <returns>지정된 이름을 가진 오브젝트에서 찾은 컴포넌트, 없으면 null</returns>
    public static T FindComponent<T>(this Transform transform, string name) where T : Component
    {
        T[] t = transform.GetComponentsInChildren<T>(true);
        foreach (T c in t)
        {
            if (c.name == name)
            {
                return c;
            }
        }
        return null;
    }
    /// <summary>
    /// 이 Transform의 모든 자식 GameObject를 삭제합니다.
    /// </summary>
    /// <param name="t">부모 Transform</param>
    public static void DestroyAllChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            var child = t.GetChild(i).gameObject;
            Object.Destroy(child);
        }
    }
}