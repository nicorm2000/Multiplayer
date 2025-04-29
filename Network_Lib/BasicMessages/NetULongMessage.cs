using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetULongMessage), MessageType.Ulong)]
    public class NetULongMessage : BaseReflectionMessage<ulong>
    {
        ulong data;

        public NetULongMessage(MessagePriority messagePriority, ulong data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Ulong;
            this.data = data;
        }

        public NetULongMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Ulong;
            this.data = Deserialize(data);
        }

        public override ulong Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(ulong))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            ulong extractedValue = BitConverter.ToUInt64(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleULong(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleULong(ulong value)
        {
            const ulong MAX_EXPECTED_VALUE = 1000000000000;
            return value <= MAX_EXPECTED_VALUE;
        }

        public ulong GetData()
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
