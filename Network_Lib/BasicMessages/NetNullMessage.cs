using Net;
using System;
using System.Collections.Generic;

namespace Network_Lib.BasicMessages
{
    [NetMessageClass(typeof(NetNullMessage), MessageType.Null)]
    public class NetNullMessage : BaseReflectionMessage<bool>
    {
        public bool data = true;

        public NetNullMessage(MessagePriority messagePriority, bool data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Null;
            this.data = data;
        }

        public NetNullMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Null;
            this.data = Deserialize(data);
        }

        public override bool Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            data = BitConverter.ToBoolean(message, messageHeaderSize);

            return data;
        }

        public bool GetData()
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
