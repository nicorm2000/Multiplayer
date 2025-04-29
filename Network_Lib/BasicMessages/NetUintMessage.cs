using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetUIntMessage), MessageType.Uint)]
    public class NetUIntMessage : BaseReflectionMessage<uint>
    {
        uint data;

        public NetUIntMessage(MessagePriority messagePriority, uint data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Uint;
            this.data = data;
        }

        public NetUIntMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Uint;
            this.data = Deserialize(data);
        }

        public override uint Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(uint))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            uint extractedValue = BitConverter.ToUInt32(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleUInt(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleUInt(uint value)
        {
            const uint MAX_EXPECTED_VALUE = 1000000;
            return value <= MAX_EXPECTED_VALUE;
        }

        public uint GetData()
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
