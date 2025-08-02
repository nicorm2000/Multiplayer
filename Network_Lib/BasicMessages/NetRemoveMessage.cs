using System.Collections.Generic;
using System;
using Net;

/// <summary>
/// Represents a network message for removing an object.
/// </summary>
[NetMessageClass(typeof(NetRemoveMessage), MessageType.Remove)]
public class NetRemoveMessage : BaseReflectionMessage<Remove>
{
    private Remove data;

    /// <summary>
    /// Initializes a new instance of the NetRemoveMessage class with the specified priority and route.
    /// </summary>
    /// <param name="priority">The priority of the message.</param>
    /// <param name="keyHash">The hash key of the object to remove.</param>
    /// <param name="route">The route information for the message.</param>
    public NetRemoveMessage(MessagePriority priority, int keyHash, List<RouteInfo> route) : base(priority, route)
    {
        currentMessageType = MessageType.Remove;
        this.data = new Remove(keyHash);
    }

    /// <summary>
    /// Initializes a new instance of the NetRemoveMessage class from serialized data.
    /// </summary>
    /// <param name="serializedData">The serialized message data.</param>
    public NetRemoveMessage(byte[] serializedData) : base(MessagePriority.Default, new List<RouteInfo>())
    {
        currentMessageType = MessageType.Remove;
        this.data = Deserialize(serializedData);
    }

    /// <summary>
    /// Deserializes the message data into a Remove object.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>A Remove object containing the key hash.</returns>
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

    /// <summary>
    /// Validates that a key hash is valid.
    /// </summary>
    /// <param name="keyHash">The key hash to validate.</param>
    /// <returns>True if the key hash is valid; otherwise, false.</returns>
    private bool IsValidKeyHash(int keyHash)
    {
        return keyHash != -1 && keyHash != 0;
    }

    /// <summary>
    /// Gets the Remove data contained in the message.
    /// </summary>
    /// <returns>A Remove object containing the key hash.</returns>
    public Remove GetData()
    {
        return data;
    }

    /// <summary>
    /// Serializes the message into a byte array.
    /// </summary>
    /// <returns>The serialized message data.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new List<byte>();
        SerializeHeader(ref outData);
        outData.AddRange(BitConverter.GetBytes(data.KeyHash));
        outData.AddRange(MessageChecker.SerializeCheckSum(outData));
        return outData.ToArray();
    }
}

/// <summary>
/// Serializes the message into a byte array.
/// </summary>
/// <returns>The serialized message data.</returns>
public class Remove
{
    public int KeyHash { get; }

    /// <summary>
    /// Initializes a new instance of the Remove class with the specified key hash.
    /// </summary>
    /// <param name="keyHash">The key hash of the object to remove.</param>
    public Remove(int keyHash)
    {
        KeyHash = keyHash;
    }
}