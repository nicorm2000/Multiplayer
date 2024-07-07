using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetCharMessage), MessageType.Char)]
    public class NetCharMessage : BaseReflectionMessage<char>
    {
        char data;

        public NetCharMessage(MessagePriority messagePriority, char data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Char;
            this.data = data;
        }

        public NetCharMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Char;
            this.data = Deserialize(data);
        }

        public override char Deserialize(byte[] message)
        {
            byte[] tmp = new byte[message.Length - messageHeaderSize];
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                for (int i = messageHeaderSize; i < message.Length; i++)
                {
                    tmp[i - messageHeaderSize] = message[i];
                }
                data = BitConverter.ToChar(tmp);
            }
            return data;
        }

        public char GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
