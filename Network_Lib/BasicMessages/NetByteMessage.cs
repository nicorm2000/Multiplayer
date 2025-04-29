using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetByteMessage), MessageType.Byte)]
    public class NetByteMessage : BaseReflectionMessage<byte>
    {
        byte data;

        public NetByteMessage(MessagePriority messagePriority, byte data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Byte;
            this.data = data;
        }

        public NetByteMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Byte;
            this.data = Deserialize(data);
        }

        public override byte Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(byte))
                return data;

            return message[messageHeaderSize];
        }

        public byte GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);
            outData.Add(data);
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
