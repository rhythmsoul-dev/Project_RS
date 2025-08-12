using System;
using System.Collections.Generic;
using UnityEngine;

public class BezierSegment
{
    public BezierSegment()
    {
    }

    public BezierSegment(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        P1 = p1;
        P2 = p2;
        P3 = p3;

        Length = VectorUtility.BezierLength(P1, P2, P3, 50);
    }

    public Vector2 P1;
    public Vector2 P2;
    public Vector2 P3;

    public float Length;
}

public static class VectorUtility
{
    public static Vector2 BezierCurves(Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        //Vector2 p4 = Vector2.Lerp(p1, p2, t);
        //Vector2 p5 = Vector2.Lerp(p2, p3, t);

        //return Vector2.Lerp(p4, p5, t);

        float u = 1f - t;
        return u * u * p1 + 2f * u * t * p2 + t * t * p3;
    }

    public static float BezierLength(Vector2 p1, Vector2 p2, Vector2 p3, int resolution = 20)
    {
        float length = 0f;
        Vector2 prev = p1;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector2 point = BezierCurves(p1, p2, p3, t);
            length += Vector2.Distance(prev, point);
            prev = point;
        }

        return length;
    }

    public static float BezierLength(List<Vector2> path)
    {
        float length = 0f;

        for (int i = 0; i < path.Count - 1; i++)
        {
            length += Vector2.Distance(path[i], path[i + 1]);
        }

        return length;
    }

    public static List<Vector2> SampleBezierPath(List<BezierSegment> segments, int resolution)
    {
        List<Vector2> result = new List<Vector2>();

        foreach (BezierSegment segment in segments)
        {
            for (int i = 0; i < resolution; i++)
            {
                float t = (float)i / resolution;
                Vector2 point = BezierCurves(segment.P1, segment.P2, segment.P3, t);
                result.Add(point);
            }
        }

        if (segments.Count > 0)
        {
            BezierSegment last = segments.Last();
            result.Add(BezierCurves(last.P1, last.P2, last.P3, 1f));
        }

        return result;
    }
}
