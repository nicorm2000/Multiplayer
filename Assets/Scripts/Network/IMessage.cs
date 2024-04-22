using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using UnityEditor.VersionControl;
using UnityEngine.UI;

public enum MessageType
{
    SetClientID = -2,
    HandShake = -1,
    Console = 0,
    Position = 1
}

public interface IMessage<T>
{
    public MessageType GetMessageType();
    public byte[] Serialize();
    public T Deserialize(byte[] message);
}

public class NetHandShake : IMessage<(long, int)>
{
    (long, int) data;

    public NetHandShake((long, int) data)
    {
        this.data = data;
    }

    public NetHandShake(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public (long, int) Deserialize(byte[] message)
    {
        (long, int) outData;

        outData.Item1 = BitConverter.ToInt64(message, sizeof(int));
        outData.Item2 = BitConverter.ToInt32(message, sizeof(int) + sizeof(long));

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.HandShake;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));


        return outData.ToArray();
    }
}

public class NetVector3 : IMessage<UnityEngine.Vector3>
{
    private static ulong lastMsgID = 0;
    private Vector3 data;

    public NetVector3(Vector3 data)
    {
        this.data = data;
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, sizeof(int));
        outData.y = BitConverter.ToSingle(message, sizeof(long) + sizeof(int));
        outData.z = BitConverter.ToSingle(message, sizeof(long) * sizeof(char));

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Position;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(lastMsgID++));
        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }

    //Dictionary<Client,Dictionary<msgType,int>>
}

public class NetSetClientID : IMessage<int>
{
    int data;

    public NetSetClientID(int data)
    {
        this.data = data;
    }

    public NetSetClientID(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public int GetData()
    {
        return data;
    }

    public int Deserialize(byte[] message)
    {
        int outdata;

        outdata = BitConverter.ToInt32(message, sizeof(int));

        return outdata;
    }

    public MessageType GetMessageType()
    {
        return MessageType.SetClientID;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(data));

        return outData.ToArray();
    }
}

[Serializable]
public class NetConsole
{
    char[] data;

    public NetConsole(char[] data)
    {
        this.data = data;
    }

    public NetConsole(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public char[] GetData()
    {
        return data;
    }

    private char[] Deserialize(byte[] message)
    {
        int dataSize = message.Length - sizeof(int) / sizeof(char);

        char[] outdata = new char[dataSize];

        for (int i = 0; i < dataSize; i++)
        {
            outdata[i] = BitConverter.ToChar(message, sizeof(int) + i * sizeof(char));
        }

        return outdata;
    }

    public static void Deserialize(byte[] message, out char[] outdata, out int sum)
    {
        int dataSize = message.Length - sizeof(int) / sizeof(char);

        outdata = new char[dataSize];

        for (int i = 0; i < dataSize; i++)
        {
            outdata[i] = BitConverter.ToChar(message, sizeof(int) + i * sizeof(char));
        }

        sum = BitConverter.ToInt32(message, message.Length - sizeof(int));
    }

    public MessageType GetMessageType()
    {
        return MessageType.Console;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        int sum = 0;

        for (int i = 0; i < data.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(data[i]));
            sum += data[i];
        }

        outData.AddRange(BitConverter.GetBytes(sum));

        return outData.ToArray();
    }
}