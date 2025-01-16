using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Net;

[NetExtensionClass]
public static class ExtensionMethods
{
    [NetExtensionMethod(typeof(Vector2))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector2 vector2)
    {
        List<(FieldInfo, NetVariable)> vector2Values = new()
        {
            (vector2.GetType().GetField(nameof(vector2.x)), new NetVariable(0)),
            (vector2.GetType().GetField(nameof(vector2.y)), new NetVariable(1))
        };

        return vector2Values;
    }

    [NetExtensionMethod(typeof(Vector3))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector3 vector3)
    {
        List<(FieldInfo, NetVariable)> vector3Values = new()
        {
            (vector3.GetType().GetField(nameof(vector3.x)), new NetVariable(0)),
            (vector3.GetType().GetField(nameof(vector3.y)), new NetVariable(1)),
            (vector3.GetType().GetField(nameof(vector3.z)), new NetVariable(2))
        };

        return vector3Values;
    }

    [NetExtensionMethod(typeof(Vector4))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector4 vector4)
    {
        List<(FieldInfo, NetVariable)> vector4Values = new()
        {
            (vector4.GetType().GetField(nameof(vector4.x)), new NetVariable(0)),
            (vector4.GetType().GetField(nameof(vector4.y)), new NetVariable(1)),
            (vector4.GetType().GetField(nameof(vector4.z)), new NetVariable(2)),
            (vector4.GetType().GetField(nameof(vector4.w)), new NetVariable(3))
        };

        return vector4Values;
    }

    [NetExtensionMethod(typeof(Vector2Int))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector2Int vector2Int)
    {
        List<(FieldInfo, NetVariable)> vector2IntValues = new()
        {
            (vector2Int.GetType().GetField(nameof(vector2Int.x)), new NetVariable(0)),
            (vector2Int.GetType().GetField(nameof(vector2Int.y)), new NetVariable(1))
        };

        return vector2IntValues;
    }

    [NetExtensionMethod(typeof(Vector3Int))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector3Int vector3Int)
    {
        List<(FieldInfo, NetVariable)> vector3IntValues = new()
        {
            (vector3Int.GetType().GetField(nameof(vector3Int.x)), new NetVariable(0)),
            (vector3Int.GetType().GetField(nameof(vector3Int.y)), new NetVariable(1)),
            (vector3Int.GetType().GetField(nameof(vector3Int.z)), new NetVariable(2))
        };

        return vector3IntValues;
    }

    [NetExtensionMethod(typeof(Quaternion))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Quaternion quaternion)
    {
        List<(FieldInfo, NetVariable)> quaternionValues = new()
        {
            (quaternion.GetType().GetField(nameof(quaternion.x)), new NetVariable(0)),
            (quaternion.GetType().GetField(nameof(quaternion.y)), new NetVariable(1)),
            (quaternion.GetType().GetField(nameof(quaternion.z)), new NetVariable(2)),
            (quaternion.GetType().GetField(nameof(quaternion.w)), new NetVariable(3))
        };

        return quaternionValues;
    }

    [NetExtensionMethod(typeof(Color))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Color color)
    {
        List<(FieldInfo, NetVariable)> colorValues = new()
        {
            (color.GetType().GetField(nameof(color.r)), new NetVariable(0)),
            (color.GetType().GetField(nameof(color.g)), new NetVariable(1)),
            (color.GetType().GetField(nameof(color.b)), new NetVariable(2)),
            (color.GetType().GetField(nameof(color.a)), new NetVariable(3))
        };

        return colorValues;
    }
    
    [NetExtensionMethod(typeof(Color32))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Color32 color32)
    {
        List<(FieldInfo, NetVariable)> color32Values = new()
        {
            (color32.GetType().GetField(nameof(color32.r)), new NetVariable(0)),
            (color32.GetType().GetField(nameof(color32.g)), new NetVariable(1)),
            (color32.GetType().GetField(nameof(color32.b)), new NetVariable(2)),
            (color32.GetType().GetField(nameof(color32.a)), new NetVariable(3))
        };

        return color32Values;
    }

    [NetExtensionMethod(typeof(Rect))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Rect rect)
    {
        List<(FieldInfo, NetVariable)> rectValues = new()
        {
            (rect.GetType().GetField(nameof(rect.x)), new NetVariable(0)),
            (rect.GetType().GetField(nameof(rect.y)), new NetVariable(1)),
            (rect.GetType().GetField(nameof(rect.width)), new NetVariable(2)),
            (rect.GetType().GetField(nameof(rect.height)), new NetVariable(3))
        };

        return rectValues;
    }

    [NetExtensionMethod(typeof(Bounds))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Bounds bounds)
    {
        List<(FieldInfo, NetVariable)> boundValues = new()
        {
            (bounds.GetType().GetField(nameof(bounds.center)), new NetVariable(0)),
            (bounds.GetType().GetField(nameof(bounds.extents)), new NetVariable(1))
        };

        return boundValues;
    }

    [NetExtensionMethod(typeof(Matrix4x4))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Matrix4x4 matrix)
    {
        List<(FieldInfo, NetVariable)> matrix4x4Values = new()
        {
            (matrix.GetType().GetField(nameof(matrix.m00)), new NetVariable(0)),
            (matrix.GetType().GetField(nameof(matrix.m01)), new NetVariable(1)),
            (matrix.GetType().GetField(nameof(matrix.m02)), new NetVariable(2)),
            (matrix.GetType().GetField(nameof(matrix.m03)), new NetVariable(3)),
            (matrix.GetType().GetField(nameof(matrix.m10)), new NetVariable(4)),
            (matrix.GetType().GetField(nameof(matrix.m11)), new NetVariable(5)),
            (matrix.GetType().GetField(nameof(matrix.m12)), new NetVariable(6)),
            (matrix.GetType().GetField(nameof(matrix.m13)), new NetVariable(7)),
            (matrix.GetType().GetField(nameof(matrix.m20)), new NetVariable(8)),
            (matrix.GetType().GetField(nameof(matrix.m21)), new NetVariable(9)),
            (matrix.GetType().GetField(nameof(matrix.m22)), new NetVariable(10)),
            (matrix.GetType().GetField(nameof(matrix.m23)), new NetVariable(11)),
            (matrix.GetType().GetField(nameof(matrix.m30)), new NetVariable(12)),
            (matrix.GetType().GetField(nameof(matrix.m31)), new NetVariable(13)),
            (matrix.GetType().GetField(nameof(matrix.m32)), new NetVariable(14)),
            (matrix.GetType().GetField(nameof(matrix.m33)), new NetVariable(15))
        };

        return matrix4x4Values;
    }

    [NetExtensionMethod(typeof(Plane))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Plane plane)
    {
        List<(FieldInfo, NetVariable)> planeValues = new()
        {
            (plane.GetType().GetField(nameof(plane.normal)), new NetVariable(0)),
            (plane.GetType().GetField(nameof(plane.distance)), new NetVariable(1))
        };

        return planeValues;
    }

    [NetExtensionMethod(typeof(Gradient))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Gradient gradient)
    {
        List<(FieldInfo, NetVariable)> gradientValues = new()
        {
            (gradient.GetType().GetField(nameof(gradient.colorKeys)), new NetVariable(0)),
            (gradient.GetType().GetField(nameof(gradient.alphaKeys)), new NetVariable(1)),
            (gradient.GetType().GetField(nameof(gradient.mode)), new NetVariable(2))
        };

        return gradientValues;
    }

    [NetExtensionMethod(typeof(AnimationCurve))]
    public static List<(FieldInfo, NetVariable)> GetFields(this AnimationCurve animationCurve)
    {
        List<(FieldInfo, NetVariable)> animationCurveValues = new()
        {
            (animationCurve.GetType().GetField(nameof(animationCurve.keys)), new NetVariable(0)),
            (animationCurve.GetType().GetField(nameof(animationCurve.preWrapMode)), new NetVariable(1)),
            (animationCurve.GetType().GetField(nameof(animationCurve.postWrapMode)), new NetVariable(2))
        };

        return animationCurveValues;
    }
}