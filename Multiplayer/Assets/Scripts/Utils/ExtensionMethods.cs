using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Net;

[NetExtensionClass]
public static class ExtensionMethods
{
    private static BindingFlags INSTANCE_NONPUBLIC_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;

    [NetExtensionMethod(typeof(Vector2))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector2 vector2, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> vector2Values = new()
        {
            (vector2.GetType().GetField(nameof(vector2.x)), new NetVariable(0, netAuthority)),
            (vector2.GetType().GetField(nameof(vector2.y)), new NetVariable(1, netAuthority))
        };
        
        return vector2Values;
    }

    [NetExtensionMethod(typeof(Vector3))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector3 vector3, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> vector3Values = new()
        {
            (vector3.GetType().GetField(nameof(vector3.x)), new NetVariable(0, netAuthority)),
            (vector3.GetType().GetField(nameof(vector3.y)), new NetVariable(1, netAuthority)),
            (vector3.GetType().GetField(nameof(vector3.z)), new NetVariable(2, netAuthority))
        };

        return vector3Values;
    }

    [NetExtensionMethod(typeof(Vector4))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector4 vector4, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> vector4Values = new()
        {
            (vector4.GetType().GetField(nameof(vector4.x)), new NetVariable(0, netAuthority)),
            (vector4.GetType().GetField(nameof(vector4.y)), new NetVariable(1, netAuthority)),
            (vector4.GetType().GetField(nameof(vector4.z)), new NetVariable(2, netAuthority)),
            (vector4.GetType().GetField(nameof(vector4.w)), new NetVariable(3, netAuthority))
        };

        return vector4Values;
    }

    [NetExtensionMethod(typeof(Vector2Int))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector2Int vector2Int, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> vector2IntValues = new()
        {
            (vector2Int.GetType().GetField("m_X", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(0, netAuthority)),
            (vector2Int.GetType().GetField("m_Y", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(1, netAuthority))
        };

        return vector2IntValues;
    }

    [NetExtensionMethod(typeof(Vector3Int))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector3Int vector3Int, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> vector3IntValues = new()
        {
            (vector3Int.GetType().GetField("m_X", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(0, netAuthority)),
            (vector3Int.GetType().GetField("m_Y", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(1, netAuthority)),
            (vector3Int.GetType().GetField("m_Z", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(2, netAuthority))
        };

        return vector3IntValues;
    }

    [NetExtensionMethod(typeof(Quaternion))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Quaternion quaternion, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> quaternionValues = new()
        {
            (quaternion.GetType().GetField(nameof(quaternion.x)), new NetVariable(0, netAuthority)),
            (quaternion.GetType().GetField(nameof(quaternion.y)), new NetVariable(1, netAuthority)),
            (quaternion.GetType().GetField(nameof(quaternion.z)), new NetVariable(2, netAuthority)),
            (quaternion.GetType().GetField(nameof(quaternion.w)), new NetVariable(3, netAuthority))
        };

        return quaternionValues;
    }

    [NetExtensionMethod(typeof(Color))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Color color, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> colorValues = new()
        {
            (color.GetType().GetField(nameof(color.r)), new NetVariable(0, netAuthority)),
            (color.GetType().GetField(nameof(color.g)), new NetVariable(1, netAuthority)),
            (color.GetType().GetField(nameof(color.b)), new NetVariable(2, netAuthority)),
            (color.GetType().GetField(nameof(color.a)), new NetVariable(3, netAuthority))
        };

        return colorValues;
    }
    
    [NetExtensionMethod(typeof(Color32))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Color32 color32, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> color32Values = new()
        {
            (color32.GetType().GetField(nameof(color32.r)), new NetVariable(0, netAuthority)),
            (color32.GetType().GetField(nameof(color32.g)), new NetVariable(1, netAuthority)),
            (color32.GetType().GetField(nameof(color32.b)), new NetVariable(2, netAuthority)),
            (color32.GetType().GetField(nameof(color32.a)), new NetVariable(3, netAuthority))
        };

        return color32Values;
    }

    [NetExtensionMethod(typeof(Rect))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Rect rect, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> rectValues = new()
        {
            (rect.GetType().GetField("m_XMin", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(0, netAuthority)),
            (rect.GetType().GetField("m_YMin", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(1, netAuthority)),
            (rect.GetType().GetField("m_Width", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(2, netAuthority)),
            (rect.GetType().GetField("m_Height", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(3, netAuthority))
        };

        return rectValues;
    }

    [NetExtensionMethod(typeof(Bounds))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Bounds bounds, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> boundValues = new()
        {
            (bounds.GetType().GetField("m_Center", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(0, netAuthority)),
            (bounds.GetType().GetField("m_Extents", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(1, netAuthority))
        };

        return boundValues;
    }

    [NetExtensionMethod(typeof(Matrix4x4))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Matrix4x4 matrix, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> matrix4x4Values = new()
        {
            (matrix.GetType().GetField(nameof(matrix.m00)), new NetVariable(0, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m01)), new NetVariable(1, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m02)), new NetVariable(2, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m03)), new NetVariable(3, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m10)), new NetVariable(4, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m11)), new NetVariable(5, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m12)), new NetVariable(6, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m13)), new NetVariable(7, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m20)), new NetVariable(8, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m21)), new NetVariable(9, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m22)), new NetVariable(10, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m23)), new NetVariable(11, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m30)), new NetVariable(12, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m31)), new NetVariable(13, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m32)), new NetVariable(14, netAuthority)),
            (matrix.GetType().GetField(nameof(matrix.m33)), new NetVariable(15, netAuthority))
        };

        return matrix4x4Values;
    }

    [NetExtensionMethod(typeof(Plane))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Plane plane, NETAUTHORITY netAuthority)
    {
        List<(FieldInfo, NetVariable)> planeValues = new()
        {
            (plane.GetType().GetField("m_Normal", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(0, netAuthority)),
            (plane.GetType().GetField("m_Distance", INSTANCE_NONPUBLIC_FLAGS), new NetVariable(1, netAuthority))
        };

        return planeValues;
    }

    public static TRS TranslateTRS(this Transform transform)
    {
        TRS trs = new TRS();

        trs.position = (transform.position.x, transform.position.y, transform.position.z);
        trs.rotation = (transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        trs.scale = (transform.localScale.x, transform.localScale.y, transform.localScale.z);
        trs.isActive = transform.gameObject.activeSelf;

        return trs;
    }

    public static void FromTRS(this Transform transform, TRS tRS, NetTRS.SYNC syncValue)
    {
        if (!syncValue.HasFlag(NetTRS.SYNC.NOTPOSITION))
        transform.position = new Vector3(tRS.position.Item1, tRS.position.Item2, tRS.position.Item3);
        if (!syncValue.HasFlag(NetTRS.SYNC.NOTROTATION))
        transform.rotation = new Quaternion(tRS.rotation.Item1, tRS.rotation.Item2, tRS.rotation.Item3, tRS.rotation.Item4);
        if (!syncValue.HasFlag(NetTRS.SYNC.NOTSCALE))
        transform.localScale = new Vector3(tRS.scale.Item1, tRS.scale.Item2, tRS.scale.Item3);
        if (!syncValue.HasFlag(NetTRS.SYNC.NOTISACTIVE))
        transform.gameObject.SetActive(tRS.isActive);
    }
}