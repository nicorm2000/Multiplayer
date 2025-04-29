using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetSByteMessage), MessageType.Sbyte)]
    public class NetSByteMessage : BaseReflectionMessage<sbyte>
    {
        sbyte data;

        public NetSByteMessage(MessagePriority messagePriority, sbyte data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Sbyte;
            this.data = data;
        }

        public NetSByteMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Sbyte;
            this.data = Deserialize(data);
        }

        public override sbyte Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(sbyte))
                return data;

            return (sbyte)message[messageHeaderSize];
        }

        public sbyte GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);
            outData.Add((byte)data);
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
