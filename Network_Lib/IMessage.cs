using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
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