using Net;
using System.Collections.Generic;
using System;

[NetMessageClass(typeof(NetRemoveMessage), MessageType.Remove)]
public class NetRemoveMessage : BaseReflectionMessage<Remove>
{
    private Remove data;

    public NetRemoveMessage(MessagePriority priority, int keyHash, List<RouteInfo> route) : base(priority, route)
    {
        currentMessageType = MessageType.Remove;
        this.data = new Remove(keyHash);
    }

    public NetRemoveMessage(byte[] serializedData) : base(MessagePriority.Default, new List<RouteInfo>())
    {
        currentMessageType = MessageType.Remove;
        this.data = Deserialize(serializedData);
    }

    public override Remove Deserialize(byte[] message)
    {
        DeserializeHeader(message);
        bool checksumValid = MessageChecker.DeserializeCheckSum(message);

        if (message.Length < messageHeaderSize + sizeof(int))
            return new Remove(-1);

        int keyHash = BitConverter.ToInt32(message, messageHeaderSize);

        if (!checksumValid && !IsValidKeyHash(keyHash))
            return new Remove(-1);

        return new Remove(keyHash);
    }

    private bool IsValidKeyHash(int keyHash)
    {
        return keyHash != -1 && keyHash != 0;
    }

    public Remove GetData()
    {
        return data;
    }

    public override byte[] Serialize()
    {
        List<byte> outData = new List<byte>();
        SerializeHeader(ref outData);
        outData.AddRange(BitConverter.GetBytes(data.KeyHash));
        outData.AddRange(MessageChecker.SerializeCheckSum(outData));
        return outData.ToArray();
    }
}

public class Remove
{
    public int KeyHash { get; }

    public Remove(int keyHash)
    {
        KeyHash = keyHash;
    }
}