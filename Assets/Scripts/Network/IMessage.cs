using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Specifies the priority levels for messages.
/// </summary>
[Flags]
public enum MessagePriority
{
    Default = 0,
    Sorteable = 1,
    NonDisposable = 2
}

/// <summary>
/// Specifies the types of messages.
/// </summary>
public enum MessageType
{
    Default = -100,
    Confirm = -5,
    Error = -4,
    Ping = -3,
    ServerToClientHandShake = -2,
    ClientToServerHandShake = -1,
    Console = 0,
    Position = 1,
    BulletInstatiate = 2,
    Disconnection = 3,
    UpdateLobbyTimer = 4,
    UpdateGameplayTimer = 5,
    UpdateLobbyTimerForNewPlayers = 6,
    Winner = 7
};

/// <summary>
/// Interface for message serialization and deserialization.
/// </summary>
/// <typeparam name="T">Type of message data.</typeparam>
public interface IMessage<T>
{
    /// <summary>
    /// Serializes the message.
    /// </summary>
    /// <returns>The serialized message data.</returns
    public byte[] Serialize();
    /// <summary>
    /// Deserializes the message.
    /// </summary>
    /// <param name="message">The message data to deserialize.</param>
    /// <returns>The deserialized message data.</returns>
    public T Deserialize(byte[] message);
}

/// <summary>
/// Abstract base class for defining message types.
/// </summary>
/// <typeparam name="T">Type of message data.</typeparam>
public abstract class BaseMessage<T> : IMessage<T>
{
    protected int messageHeaderSize = sizeof(int) * 2; // Size of message header containing MessageType and MessagePriority

    protected MessagePriority currentMessagePriority;
    protected MessageType currentMessageType;
    protected int messageOrder = 0; // Order of the message in case of sortable messages

    #region Properties

    /// <summary>
    /// Gets or sets the current message priority.
    /// </summary>
    public MessagePriority CurrentMessagePriority
    {
        get { return currentMessagePriority; }
        set { currentMessagePriority = value; }
    }

    /// <summary>
    /// Gets or sets the current message type.
    /// </summary>
    public MessageType CurrentMessageType
    {
        get { return currentMessageType; }
        set { currentMessageType = value; }
    }

    /// <summary>
    /// Gets or sets the message order (used for sortable messages).
    /// </summary>
    public int MessageOrder
    {
        get { return messageOrder; }
        set { messageOrder = value; }
    }

    /// <summary>
    /// Checks if the message is sortable.
    /// </summary>
    public bool IsSorteableMessage
    {
        get { return ((currentMessagePriority & MessagePriority.Sorteable) != 0); }
    }

    /// <summary>
    /// Checks if the message is non-disposable.
    /// </summary>
    public bool IsNonDisposableMessage
    {
        get { return ((currentMessagePriority & MessagePriority.NonDisposable) != 0); }
    }

    #endregion

    /// <summary>
    /// Constructor to initialize the message with a given priority.
    /// </summary>
    /// <param name="messagePriority">The priority of the message.</param>
    public BaseMessage(MessagePriority messagePriority)
    {
        currentMessagePriority = messagePriority;
    }

    /// <summary>
    /// Deserializes the header of the message.
    /// </summary>
    /// <param name="message">The message data to deserialize.</param>
    public void DeserializeHeader(byte[] message)
    {
        currentMessageType = (MessageType)BitConverter.ToInt32(message, 0);
        currentMessagePriority = (MessagePriority)BitConverter.ToInt32(message, sizeof(int));

        if (IsSorteableMessage)
        {
            messageOrder = BitConverter.ToInt32(message, sizeof(int) * 2);
            messageHeaderSize += sizeof(int);
        }
    }

    /// <summary>
    /// Serializes the header of the message.
    /// </summary>
    /// <param name="outData">The list to which the serialized header will be added.</param>
    public void SerializeHeader(ref List<byte> outData)
    {
        outData.AddRange(BitConverter.GetBytes((int)currentMessageType));
        outData.AddRange(BitConverter.GetBytes((int)currentMessagePriority));

        if (IsSorteableMessage)
        {
            outData.AddRange(BitConverter.GetBytes(messageOrder));
            messageHeaderSize += sizeof(int);
        }
    }

    /// <summary>
    /// Serializes the message queue.
    /// </summary>
    /// <param name="data">The list to which the serialized message queue will be added.</param>
    public void SerializeQueue(ref List<byte> data)
    {
        data.AddRange(MessageChecker.SerializeCheckSum(data));
    }

    /// <summary>
    /// Abstract method to serialize the message.
    /// </summary>
    /// <returns>The serialized message data.</returns>
    public abstract byte[] Serialize();

    /// <summary>
    /// Abstract method to deserialize the message.
    /// </summary>
    /// <param name="message">The message data to deserialize.</param>
    /// <returns>The deserialized message data.</returns>
    public abstract T Deserialize(byte[] message);

}

/// <summary>
/// Represents a message for client-to-server network handshake.
/// </summary>
public class ClientToServerNetHandShake : BaseMessage<(long, int, string)>
{
    private (long ip, int port, string name) data;

    /// <summary>
    /// Initializes a new instance of the ClientToServerNetHandShake class with the specified message priority and data.
    /// </summary>
    /// <param name="messagePriority">The priority of the message.</param>
    /// <param name="data">The data to be sent in the handshake.</param>
    public ClientToServerNetHandShake(MessagePriority messagePriority, (long, int, string) data) : base(messagePriority)
    {
        currentMessageType = MessageType.ClientToServerHandShake;
        this.data = data;
    }

    /// <summary>
    /// Initializes a new instance of the ClientToServerNetHandShake class from serialized data.
    /// </summary>
    /// <param name="data">The serialized data of the handshake.</param>
    public ClientToServerNetHandShake(byte[] data) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.ClientToServerHandShake;
        this.data = Deserialize(data);
    }

    /// <summary>
    /// Deserializes the message data.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The deserialized data of the handshake.</returns>
    public override (long, int, string) Deserialize(byte[] message)
    {
        (long, int, string) outData = (0, 0, "");

        if (MessageChecker.DeserializeCheckSum(message))
        {
            DeserializeHeader(message);

            outData.Item1 = BitConverter.ToInt64(message, messageHeaderSize);
            messageHeaderSize += sizeof(long);
            outData.Item2 = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            outData.Item3 = MessageChecker.DeserializeString(message, messageHeaderSize);
        }

        return outData;
    }

    /// <summary>
    /// Gets the data of the handshake.
    /// </summary>
    /// <returns>The data of the handshake.</returns>
    public (long, int, string) GetData()
    {
        return data;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the handshake.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));

        outData.AddRange(MessageChecker.SerializeString(data.name.ToCharArray()));

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message for transmitting a Vector3 position over the network.
/// </summary>
public class NetVector3 : BaseMessage<(int, Vector3)>
{
    private (int id, Vector3 position) data;

    /// <summary>
    /// Initializes a new instance of the NetVector3 class with the specified message priority and data.
    /// </summary>
    /// <param name="messagePriority">The priority of the message.</param>
    /// <param name="data">The data to be sent, containing an ID and a Vector3 position.</param>
    public NetVector3(MessagePriority messagePriority, (int, Vector3) data) : base(messagePriority)
    {
        currentMessageType = MessageType.Position;
        this.data = data;
    }

    /// <summary>
    /// Initializes a new instance of the NetVector3 class from serialized data.
    /// </summary>
    /// <param name="data">The serialized data of the message.</param>
    public NetVector3(byte[] data) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.Position;
        this.data = Deserialize(data);
    }

    /// <summary>
    /// Gets the data contained in the message.
    /// </summary>
    /// <returns>The ID and Vector3 position contained in the message.</returns>
    public (int id, Vector3 position) GetData()
    {
        return data;
    }

    /// <summary>
    /// Deserializes the message data.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The ID and Vector3 position deserialized from the message.</returns>
    public override (int, Vector3) Deserialize(byte[] message)
    {
        (int id, Vector3 position) outData = (-1, Vector3.zero);

        if (MessageChecker.DeserializeCheckSum(message))
        {
            DeserializeHeader(message);

            outData.id = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            outData.position.x = BitConverter.ToSingle(message, messageHeaderSize);
            messageHeaderSize += sizeof(float);
            outData.position.y = BitConverter.ToSingle(message, messageHeaderSize);
            messageHeaderSize += sizeof(float);
            outData.position.z = BitConverter.ToSingle(message, messageHeaderSize);
        }

        return outData;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(BitConverter.GetBytes(data.id));

        outData.AddRange(BitConverter.GetBytes(data.position.x));
        outData.AddRange(BitConverter.GetBytes(data.position.y));
        outData.AddRange(BitConverter.GetBytes(data.position.z));

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message for transmitting a handshake from server to client.
/// </summary>
public class ServerToClientHandShake : BaseMessage<List<(int clientID, string clientName)>>
{
    private List<(int clientID, string clientName)> data;

    /// <summary>
    /// Represents a message for transmitting a handshake from server to client.
    /// </summary>
    public ServerToClientHandShake(MessagePriority messagePriority, List<(int clientID, string clientName)> data) : base(messagePriority)
    {
        currentMessageType = MessageType.ServerToClientHandShake;
        this.data = data;

    }

    /// <summary>
    /// Initializes a new instance of the ServerToClientHandShake class from serialized data.
    /// </summary>
    /// <param name="data">The serialized data of the message.</param>
    public ServerToClientHandShake(byte[] data) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.ServerToClientHandShake;
        this.data = Deserialize(data);
    }

    /// <summary>
    /// Gets the data contained in the message.
    /// </summary>
    /// <returns>The list of client IDs and names contained in the message.</returns>
    public List<(int clientID, string clientName)> GetData()
    {
        return data;
    }

    /// <summary>
    /// Deserializes the message data.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The list of client IDs and names deserialized from the message.</returns>
    public override List<(int clientID, string clientName)> Deserialize(byte[] message)
    {
        List<(int clientID, string clientName)> outData = new ();

        if (MessageChecker.DeserializeCheckSum(message))
        {
            DeserializeHeader(message);

            int listCount = BitConverter.ToInt32(message, messageHeaderSize);

            int offSet = messageHeaderSize + sizeof(int);
            for (int i = 0; i < listCount; i++)
            {
                int clientID = BitConverter.ToInt32(message, offSet);
                offSet += sizeof(int);
                int clientNameLength = BitConverter.ToInt32(message, offSet);
                string name = MessageChecker.DeserializeString(message, offSet);
                offSet += sizeof(char) * clientNameLength + sizeof(int);

                outData.Add((clientID, name));
            }
        }
        return outData;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(BitConverter.GetBytes(data.Count));

        foreach ((int clientID, string clientName) clientInfo in data)
        {
            outData.AddRange(BitConverter.GetBytes(clientInfo.clientID)); // Client ID
            outData.AddRange(MessageChecker.SerializeString(clientInfo.clientName.ToCharArray())); // Name
        }

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message containing text data.
/// </summary>
[Serializable]
public class NetMessage : BaseMessage<char[]>
{
    private char[] data;

    /// <summary>
    /// Initializes a new instance of the NetMessage class with the specified priority and text data.
    /// </summary>
    /// <param name="priority">The priority of the message.</param>
    /// <param name="data">The text data to be transmitted.</param>
    public NetMessage(MessagePriority priority, char[] data) : base(priority)
    {
        currentMessageType = MessageType.Console;
        this.data = data;
    }

    /// <summary>
    /// Initializes a new instance of the NetMessage class from serialized data.
    /// </summary>
    /// <param name="data">The serialized data of the message.</param>
    public NetMessage(byte[] data) : base(MessagePriority.Default) // Deserialize updates this
    {
        currentMessageType = MessageType.Console;
        this.data = Deserialize(data);
    }

    /// <summary>
    /// Gets the text data contained in the message.
    /// </summary>
    /// <returns>The text data contained in the message.</returns>
    public char[] GetData()
    {
        return data;
    }

    /// <summary>
    /// Deserializes the message data.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The text data deserialized from the message.</returns>
    public override char[] Deserialize(byte[] message)
    {
        string text = "";

        DeserializeHeader(message);

        if (MessageChecker.DeserializeCheckSum(message))
        {
            text = MessageChecker.DeserializeString(message, messageHeaderSize);
        }

        return text.ToCharArray();
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);


        outData.AddRange(MessageChecker.SerializeString(data));


        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message used for ping.
/// </summary>
public class NetPing
{
    private MessageType messageType = MessageType.Ping;

    /// <summary>
    /// Gets the message type.
    /// </summary>
    /// <returns>The message type.</returns>
    public MessageType GetMessageType()
    {
        return messageType;
    }

    /// <summary>
    /// Serializes the ping message.
    /// </summary>
    /// <returns>The serialized ping message.</returns>
    public byte[] Serialize()
    {
        List<byte> outData = new ();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes((int)MessagePriority.Default));

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message containing a client ID for disconnection.
/// </summary>
public class NetIDMessage : BaseMessage<int>
{
    private int clientID;

    /// <summary>
    /// Initializes a new instance of the NetIDMessage class with the specified priority and client ID.
    /// </summary>
    /// <param name="messagePriority">The priority of the message.</param>
    /// <param name="clientID">The client ID.</param>
    public NetIDMessage(MessagePriority messagePriority, int clientID) : base(messagePriority)
    {
        currentMessageType = MessageType.Disconnection;
        this.clientID = clientID;
    }

    /// <summary>
    /// Initializes a new instance of the NetIDMessage class from serialized data.
    /// </summary>
    /// <param name="data">The serialized data of the message.</param>
    public NetIDMessage(byte[] data) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.Disconnection;
        this.clientID = Deserialize(data);
    }

    /// <summary>
    /// Deserializes the message data to obtain the client ID.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The client ID obtained from the message data.</returns>
    public override int Deserialize(byte[] message)
    {
        DeserializeHeader(message);

        if (MessageChecker.DeserializeCheckSum(message))
        {
            clientID = BitConverter.ToInt32(message, messageHeaderSize);
        }
        return clientID;
    }

    /// <summary>
    /// Gets the client ID contained in the message.
    /// </summary>
    /// <returns>The client ID contained in the message.</returns>
    public int GetData()
    {
        return clientID;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(BitConverter.GetBytes(clientID));

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message containing an error string.
/// </summary>
public class NetErrorMessage : BaseMessage<string>
{
    private string error;

    /// <summary>
    /// Initializes a new instance of the NetErrorMessage class with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    public NetErrorMessage(string error) : base(MessagePriority.Default) // Always set to Default
    {
        currentMessageType = MessageType.Error;
        this.error = error;
    }

    /// <summary>
    /// Initializes a new instance of the NetErrorMessage class from serialized data.
    /// </summary>
    /// <param name="message">The serialized data of the message.</param>
    public NetErrorMessage(byte[] message) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.Error;
        error = Deserialize(message);
    }

    /// <summary>
    /// Deserializes the message data to obtain the error message.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The error message obtained from the message data.</returns>
    public override string Deserialize(byte[] message)
    {
        DeserializeHeader(message);

        if (MessageChecker.DeserializeCheckSum(message))
        {
            error = MessageChecker.DeserializeString(message, messageHeaderSize);
        }

        return error;
    }

    /// <summary>
    /// Gets the error message contained in the message.
    /// </summary>
    /// <returns>The error message contained in the message.</returns>
    public string GetData()
    {
        return error;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(MessageChecker.SerializeString(error.ToCharArray()));

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message used for updating timers.
/// </summary>
public class NetUpdateTimer : BaseMessage<bool>
{
    private bool initTimer;

    /// <summary>
    /// Initializes a new instance of the NetUpdateTimer class with the specified priority and timer initialization flag.
    /// </summary>
    /// <param name="messagePriority">The priority of the message.</param>
    /// <param name="initTimer">The flag indicating whether the timer should be initialized.</param>
    public NetUpdateTimer(MessagePriority messagePriority, bool initTimer) : base(messagePriority)
    {
        currentMessageType = MessageType.UpdateLobbyTimer;
        this.initTimer = initTimer;
    }

    /// <summary>
    /// Initializes a new instance of the NetUpdateTimer class from serialized data.
    /// </summary>
    /// <param name="data">The serialized data of the message.</param>
    public NetUpdateTimer(byte[] data) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.UpdateLobbyTimer;
        this.initTimer = Deserialize(data);
    }

    /// <summary>
    /// Gets the timer initialization flag contained in the message.
    /// </summary>
    /// <returns>The timer initialization flag contained in the message.</returns>
    public bool GetData()
    {
        return initTimer;
    }

    /// <summary>
    /// Deserializes the message data to obtain the timer initialization flag.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The timer initialization flag obtained from the message data.</returns>
    public override bool Deserialize(byte[] message)
    {
        DeserializeHeader(message);

        if (MessageChecker.DeserializeCheckSum(message))
        {
            return BitConverter.ToBoolean(message, messageHeaderSize);
        }

        return false;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(BitConverter.GetBytes(initTimer));

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message used for confirming another message type.
/// </summary>
public class NetConfirmMessage : BaseMessage<MessageType>
{
    private MessageType messageTypeToConfirm = MessageType.Default;

    /// <summary>
    /// Initializes a new instance of the NetConfirmMessage class with the specified priority and message type to confirm.
    /// </summary>
    /// <param name="messagePriority">The priority of the message.</param>
    /// <param name="messageTypeToConfirm">The message type to confirm.</param>
    public NetConfirmMessage(MessagePriority messagePriority, MessageType messageTypeToConfirm) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.Confirm;
        this.messageTypeToConfirm = messageTypeToConfirm;
    }

    /// <summary>
    /// Initializes a new instance of the NetConfirmMessage class from serialized data.
    /// </summary>
    /// <param name="message">The serialized data of the message.</param>
    public NetConfirmMessage(byte[] message) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.Confirm;
        this.messageTypeToConfirm = Deserialize(message);
    }

    /// <summary>
    /// Gets the message type to confirm contained in the message.
    /// </summary>
    /// <returns>The message type to confirm contained in the message.</returns>
    public MessageType GetData()
    {
        return messageTypeToConfirm;
    }

    /// <summary>
    /// Deserializes the message data to obtain the message type to confirm.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The message type to confirm obtained from the message data.</returns>
    public override MessageType Deserialize(byte[] message)
    {
        DeserializeHeader(message);

        if (MessageChecker.DeserializeCheckSum(message))
        {
            messageTypeToConfirm = (MessageType)BitConverter.ToInt32(message, messageHeaderSize);
        }

        return messageTypeToConfirm;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(BitConverter.GetBytes((int)messageTypeToConfirm));

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}

/// <summary>
/// Represents a message used for updating the timer for new players.
/// </summary>
public class NetUpdateNewPlayersTimer : BaseMessage<float>
{
    private float timer = -1;

    /// <summary>
    /// Initializes a new instance of the NetUpdateNewPlayersTimer class with the specified priority and timer value.
    /// </summary>
    /// <param name="messagePriority">The priority of the message.</param>
    /// <param name="timer">The timer value.</param>
    public NetUpdateNewPlayersTimer(MessagePriority messagePriority, float timer) : base(messagePriority)
    {
        currentMessageType = MessageType.UpdateLobbyTimerForNewPlayers;
        this.timer = timer;
    }

    /// <summary>
    /// Initializes a new instance of the NetUpdateNewPlayersTimer class from serialized data.
    /// </summary>
    /// <param name="data">The serialized data of the message.</param>
    public NetUpdateNewPlayersTimer(byte[] data) : base(MessagePriority.Default)
    {
        currentMessageType = MessageType.UpdateLobbyTimerForNewPlayers;
        timer = Deserialize(data);
    }

    /// <summary>
    /// Gets the timer value contained in the message.
    /// </summary>
    /// <returns>The timer value contained in the message.</returns>
    public float GetData()
    {
        return timer;
    }

    /// <summary>
    /// Deserializes the message data to obtain the timer value.
    /// </summary>
    /// <param name="message">The serialized message data.</param>
    /// <returns>The timer value obtained from the message data.</returns>
    public override float Deserialize(byte[] message)
    {
        DeserializeHeader(message);

        if (MessageChecker.DeserializeCheckSum(message))
        {
            timer = BitConverter.ToSingle(message, messageHeaderSize);
        }

        return timer;
    }

    /// <summary>
    /// Serializes the message data.
    /// </summary>
    /// <returns>The serialized data of the message.</returns>
    public override byte[] Serialize()
    {
        List<byte> outData = new ();

        SerializeHeader(ref outData);

        outData.AddRange(BitConverter.GetBytes(timer));

        SerializeQueue(ref outData);

        return outData.ToArray();
    }
}