using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.Net;

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

        outData.Item1 = BitConverter.ToInt64(message, 4);
        outData.Item2 = BitConverter.ToInt32(message, 12);
        
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

public class NetConsole : IMessage<string>
{
    string data;
    public string Deserialize(byte[] message)
    {
        string outData;

        outData = BitConverter.ToString(message, 4);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Console;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(Encoding.ASCII.GetBytes(data));

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

        outData.x = BitConverter.ToSingle(message, 8);
        outData.y = BitConverter.ToSingle(message, 12);
        outData.z = BitConverter.ToSingle(message, 16);

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

    public NetSetClientID (int data)
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

        outdata = BitConverter.ToInt32(message, 4);

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