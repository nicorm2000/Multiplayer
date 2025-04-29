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

            if (message.Length < messageHeaderSize + sizeof(bool))
                return data;

            return BitConverter.ToBoolean(message, messageHeaderSize);
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
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
