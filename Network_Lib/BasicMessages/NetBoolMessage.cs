using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetBoolMessage), MessageType.Bool)]
    public class NetBoolMessage : BaseReflectionMessage<bool>
    {
        bool data;

        public NetBoolMessage(MessagePriority messagePriority, bool data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Bool;
            this.data = data;
        }

        public NetBoolMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Bool;
            this.data = Deserialize(data);
        }

        public override bool Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            data = BitConverter.ToBoolean(message, messageHeaderSize);

            return data;
        }

        public override bool GetData()
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
