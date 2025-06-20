using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    [Flags]
    public enum MessagePriority
    {
        Default = 0,
        Sorteable = 1,
        NonDisposable = 2
    }

    public enum MessageType
    {
        Default = -100,
        Ulong = -99,
        Uint = -98,
        Ushort = -97,
        String = -96,
        Short = -95,
        Sbyte = -94,
        Long = -93,
        Int = -92,
        Float = -91,
        Double = -90,
        Decimal = -89,
        Char = -88,
        Byte = -87,
        Bool = -86,
        Null = -85,
        Empty = -84,
        Method = -83,
        Enum = -82,
        Remove = -81,
        Event = -80,
        TRS = -79,

        DisconnectAll = -13,
        DestroyNetObj = -12,
        MatchMakerPlayerListUpdate = -11,
        Instance = -10,
        InstanceRequest = -9,
        Object = -8,
        Ping = -7,
        AssignServer = -6,
        Confirm = -5,
        Error = -4,
        MatchMakerToClientHandShake = -3,
        ServerToClientHandShake = -2,
        ClientToServerHandShake = -1,
        Console = 0,
        Position = 1,
        BulletInstatiate = 2,
        Disconnection = 3,
        UpdateLobbyTimer = 4,
        UpdateGameplayTimer = 5,
        UpdateLobbyTimerForNewPlayers = 6,
        Winner = 7,
        MatchMakerIp = 8,
    };

    public interface IMessage<T>
    {
        byte[] Serialize(); //Hay que poner el Checksum siempre como ultimo parametro
        T Deserialize(byte[] message);
    }

    public abstract class ParentBaseMessage
    {
        public abstract byte[] Serialize();
    }

    public abstract class BaseMessage<T> : ParentBaseMessage, IMessage<T>
    {
        public int messageHeaderSize = 0; //MessageType y MessagePriority

        protected MessagePriority currentMessagePriority;
        protected MessageType currentMessageType;
        protected int messageOrder = 0;

        #region Properties

        public MessagePriority CurrentMessagePriority
        {
            get { return currentMessagePriority; }
            set { currentMessagePriority = value; }
        }

        public MessageType CurrentMessageType
        {
            get { return currentMessageType; }
            set { currentMessageType = value; }
        }

        public int MessageOrder
        {
            get { return messageOrder; }
            set { messageOrder = value; }
        }

        public bool IsSorteableMessage
        {
            get { return ((currentMessagePriority & MessagePriority.Sorteable) != 0); }
        }

        public bool IsNondisponsableMessage
        {
            get { return ((currentMessagePriority & MessagePriority.NonDisposable) != 0); }
        }

        #endregion

        public BaseMessage(MessagePriority messagePriority)
        {
            currentMessagePriority = messagePriority;
        }

        public virtual void DeserializeHeader(byte[] message)
        {
            messageHeaderSize = 0;
            currentMessageType = (MessageType)BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);
            currentMessagePriority = (MessagePriority)BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            if (IsSorteableMessage)
            {
                messageOrder = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);
            }

            if (IsNondisponsableMessage)
            {
                //Creo que no hay que serializar nada, lo dejo por las dudas
            }
        }

        public virtual void SerializeHeader(ref List<byte> outData)
        {
            outData.AddRange(BitConverter.GetBytes((int)currentMessageType));
            outData.AddRange(BitConverter.GetBytes((int)currentMessagePriority));

            if (IsSorteableMessage)
            {
                outData.AddRange(BitConverter.GetBytes(messageOrder));
            }

            if (IsNondisponsableMessage)
            {
                //Creo que no hay que serializar nada, lo dejo por las dudas
            }
        }

        public void SerializeQueue(ref List<byte> data)
        {
            byte[] checksum = MessageChecker.SerializeCheckSum(data);
            data.AddRange(checksum);
        }

       // public abstract byte[] Serialize();

        public abstract T Deserialize(byte[] message);
    }
}