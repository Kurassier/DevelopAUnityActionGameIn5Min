using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathTools
{
    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }
    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    /// <summary>
    /// 将数值from朝向to移动至多step距离
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static float MoveTo(this float from, float to, float step)
    {
        if (Mathf.Abs(from - to) <= step)
            from = to;
        else
        {
            if (to > from) from += step;
            else from -= step;
        }
        return from;
    }
    /// <summary>
    /// 将向量from朝向to移动至多step距离
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static Vector2 MoveTo(this Vector2 from, Vector2 to, float step)
    {
        if ((from - to).magnitude <= step)
            from = to;
        else
        {
            from = from + (to - from).normalized * step;
        }
        return from;
    }
    /// <summary>
    /// 将向量from朝向to移动至多step距离
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static Vector3 MoveTo(this Vector3 from, Vector3 to, float step)
    {
        if ((from - to).magnitude <= step)
            from = to;
        else
        {
            from = from + (to - from).normalized * step;
        }
        return from;
    }
    public static float GetIncludeAngleInDegree(Vector2 a, Vector2 b)
    {
        float dot = a.x * b.x + a.y * b.y;
        float angle = Mathf.Acos(dot / (a.magnitude * b.magnitude));
        return angle.ToDegree();
    }
    public static bool Contains(this UnityEngine.LayerMask mask, int layer)
    {
        return ((int)mask >> layer) % 2 == 1;
    }
    public static float ToDegree(this float radius)
    {
        return radius * 180 / Mathf.PI;
    }
    public static float ToRadius(this float degree)
    {
        return degree / 180 * Mathf.PI;
    }
    public static Vector2Int GetMapGridPos(this Vector2 pos)
    {
        //转换为整数之前，先平移2000个单位，避免对负数取整
        int unit = 2000;
        Vector2 temp = pos + new Vector2(unit, unit);
        Vector2Int gridPos = new Vector2Int((int)(temp.x), (int)(temp.y));
        gridPos -= new Vector2Int(unit, unit);
        return gridPos;
    }
    public static Vector2Int GetMapGridPos(this Vector3 pos)
    {
        return GetMapGridPos((Vector2)pos);
    }
    public static Vector2 GetMapGridBottomCenter(this Vector2Int pos)
    {
        return pos + new Vector2(0.5f, 0f);
    }
    public static Vector2 GetMapGridCenter(this Vector2Int pos)
    {
        return pos + new Vector2(0.5f, 0.5f);
    }
    public static Vector2 InvalidVector2
    {
        get => new Vector2(float.NaN, float.NaN);
    }
    public static Vector3 InvalidVector3
    {
        get => new Vector3(float.NaN, float.NaN, float.NaN);
    }
    public static bool IsValid(this Vector2 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y);
    }
    public static bool IsValid(this Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z);
    }


}
