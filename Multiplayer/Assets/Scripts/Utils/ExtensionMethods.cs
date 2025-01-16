using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Net;

[NetExtensionClass]
public static class ExtensionMethods
{
    [NetExtensionMethod(typeof(Vector3))]
    public static List<(FieldInfo, NetVariable)> GetFields(this Vector3 vector3)
    {
        List<(FieldInfo, NetVariable)> vectorValues = new()
        {
            (vector3.GetType().GetField(nameof(vector3.x)), new NetVariable(0)),
            (vector3.GetType().GetField(nameof(vector3.y)), new NetVariable(1)),
            (vector3.GetType().GetField(nameof(vector3.z)), new NetVariable(2))
        };

        return vectorValues;
    }

    //public static float Serialize(this Vector3 a)
    //{
    //    return 0.0f;
    //}
    //
    //public static float Deserialize(this Vector3 a)
    //{
    //    return 0.0f;
    //}

    //public static List<byte> ToMsg(this Quaternion quaternionValue, char[] fieldName)
    //{
    //    List<byte> output = new List<byte>();
    //
    //    AddHeaderMessage(output, fieldName);
    //
    //    output.AddRange(BitConverter.GetBytes(quaternionValue.x));
    //    output.AddRange(BitConverter.GetBytes(quaternionValue.y));
    //    output.AddRange(BitConverter.GetBytes(quaternionValue.z));
    //    output.AddRange(BitConverter.GetBytes(quaternionValue.w));
    //
    //    return output;
    //}
}