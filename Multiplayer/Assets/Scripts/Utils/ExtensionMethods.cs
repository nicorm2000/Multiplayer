using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Net;

[NetExtensionClass]
public static class ExtensionMethods
{
    private static BindingFlags INSTANCE_NONPUBLIC_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;

    /// <summary>
    /// Gets the fields of a Vector2 with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="vector2">The Vector2 to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Vector3 with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="vector3">The Vector3 to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Vector4 with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="vector4">The Vector4 to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Vector2Int with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="vector2Int">The Vector2Int to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Vector3Int with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="vector3Int">The Vector3Int to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Quaternion with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="quaternion">The Quaternion to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Color with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="color">The Color to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Color32 with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="color32">The Color32 to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Rect with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="rect">The Rect to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Matrix4x4 with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="matrix">The Matrix4x4 to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Bounds with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="bounds">The Bounds to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Gets the fields of a Plane with NetVariable attributes for network synchronization.
    /// </summary>
    /// <param name="plane">The Plane to inspect.</param>
    /// <param name="netAuthority">The network authority level for the fields.</param>
    /// <returns>A list of tuples containing FieldInfo and NetVariable attributes for each field.</returns>
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

    /// <summary>
    /// Converts a Transform's position, rotation, and scale into a TRS (Transform-Rotation-Scale) structure.
    /// </summary>
    /// <param name="transform">The Transform to convert.</param>
    /// <returns>A TRS structure containing the transform's position, rotation, scale, and active state.</returns>
    public static TRS TranslateTRS(this Transform transform)
    {
        TRS trs = new TRS();

        trs.position = (transform.position.x, transform.position.y, transform.position.z);
        trs.rotation = (transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        trs.scale = (transform.localScale.x, transform.localScale.y, transform.localScale.z);
        trs.isActive = transform.gameObject.activeSelf;

        return trs;
    }

    /// <summary>
    /// Applies a TRS (Transform-Rotation-Scale) structure to a Transform, with optional synchronization flags.
    /// </summary>
    /// <param name="transform">The Transform to modify.</param>
    /// <param name="tRS">The TRS structure containing the new values.</param>
    /// <param name="syncValue">Flags indicating which components should be synchronized.</param>
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