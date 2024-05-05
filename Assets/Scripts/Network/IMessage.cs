using System.Collections.Generic;
using UnityEngine;
using System;

public enum MessageType
{
    Error = -4,
    Ping = -3,
    ServerToClientHandShake = -2,
    ClientToServerHandShake = -1,
    Console = 0,
    Position = 1,
    NewCustomerNotice = 2,
    Disconnection = 3,
    ThereIsNoPlace = 4,
    RepeatMessage = 5
};

public interface IMessage<T>
{
    public const int _INT = sizeof(int);

    public MessageType GetMessageType();
    public byte[] Serialize();
    public T Deserialize(byte[] message);
}

public class ClientToServerNetHandShake : IMessage<(long, int, string)>
{
    (long ip, int port, string name) data;

    public ClientToServerNetHandShake((long, int, string) data)
    {
        this.data = data;
    }

    public ClientToServerNetHandShake(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public (long, int, string) Deserialize(byte[] message)
    {
        (long, int, string) outData = (0, 0, "");

        outData.Item1 = BitConverter.ToInt64(message, 4);
        outData.Item2 = BitConverter.ToInt32(message, 12);

        outData.Item3 = MessageChecker.DeserializeString(message, sizeof(long) + sizeof(int) * 2);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.ClientToServerHandShake;
    }

    public (long, int, string) GetData()
    {
        return data;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));

        outData.AddRange(MessageChecker.SerializeString(data.name.ToCharArray()));

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

public class ServerToClientHandShake : IMessage<List<(int clientID, string clientName)>>
{
    private List<(int clientID, string clientName)> data;

    public ServerToClientHandShake(List<(int clientID, string clientName)> data)
    {
        this.data = data;
    }

    public ServerToClientHandShake(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public List<(int clientID, string clientName)> GetData()
    {
        return data;
    }

    public List<(int clientID, string clientName)> Deserialize(byte[] message)
    {
        List<(int clientID, string clientName)> outData = new List<(int, string)>();

        int listCount = BitConverter.ToInt32(message, sizeof(int));

        int offSet = sizeof(int) * 2;

        for (int i = 0; i < listCount; i++)
        {
            int clientID = BitConverter.ToInt32(message, offSet);
            offSet += sizeof(int);
            int clientNameLength = BitConverter.ToInt32(message, offSet);
            string name = MessageChecker.DeserializeString(message, offSet);
            offSet += sizeof(char) * clientNameLength + sizeof(int);

            outData.Add((clientID, name));
        }

        return outData;
    }


    public MessageType GetMessageType()
    {
        return MessageType.ServerToClientHandShake;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(BitConverter.GetBytes(data.Count));

        foreach ((int clientID, string clientName) clientInfo in data)
        {
            outData.AddRange(BitConverter.GetBytes(clientInfo.clientID)); // ID del client
            outData.AddRange(MessageChecker.SerializeString(clientInfo.clientName.ToCharArray())); //Nombre
        }

        return outData.ToArray();
    }
}

[Serializable]
public class NetMessage : IMessage<char[]>
{
    char[] data;


    public NetMessage(char[] data)
    {
        this.data = data;
    }

    public NetMessage(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public char[] GetData()
    {
        return data;
    }

    public char[] Deserialize(byte[] message)
    {
        string text = "";

        if (MessageChecker.DeserializeCheckSum(message))
        {
            text = MessageChecker.DeserializeString(message, sizeof(int));
        }
        else
        {
            text = "Message corrupted.";
            Debug.LogError(text);
        }

        return text.ToCharArray();
    }

    public MessageType GetMessageType()
    {
        return MessageType.Console;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(MessageChecker.SerializeString(data));

        outData.AddRange(MessageChecker.SerializeCheckSum(outData));

        return outData.ToArray();
    }
}

public class NetPing
{
    public MessageType GetMessageType()
    {
        return MessageType.Ping;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        return outData.ToArray();
    }
}

public class NetDisconnection : IMessage<int>
{
    int clientToDisconect;

    public NetDisconnection(int clientToDisconect)
    {
        this.clientToDisconect = clientToDisconect;
    }

    public NetDisconnection(byte[] data)
    {
        this.clientToDisconect = Deserialize(data);
    }

    public int Deserialize(byte[] message)
    {
        return BitConverter.ToInt32(message, sizeof(int));
    }

    public MessageType GetMessageType()
    {
        return MessageType.Disconnection;
    }

    public int GetData()
    {
        return clientToDisconect;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(clientToDisconect));

        return outData.ToArray();
    }
}

public class NetErrorMessage : IMessage<string>
{
    string error;

    public NetErrorMessage(string error)
    {
        this.error = error;
    }
    public NetErrorMessage(byte[] message)
    {
        error = Deserialize(message);
    }

    public string Deserialize(byte[] message)
    {
        if (MessageChecker.DeserializeCheckSum(message))
        {

            error = MessageChecker.DeserializeString(message, sizeof(int));
        }
        else
        {
            error = "Corrupted Package";
            Debug.LogError(error);
        }

        return error;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Error;
    }

    public string GetData()
    {
        return error;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(MessageChecker.SerializeString(error.ToCharArray()));
        outData.AddRange(MessageChecker.SerializeCheckSum(outData));

        return outData.ToArray();
    }
}