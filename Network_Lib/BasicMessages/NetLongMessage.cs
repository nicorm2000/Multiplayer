using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetLongMessage), MessageType.Long)]
    public class NetLongMessage : BaseReflectionMessage<long>
    {
        long data;

        public NetLongMessage(MessagePriority messagePriority, long data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Long;
            this.data = data;
        }

        public NetLongMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Long;
            this.data = Deserialize(data);
        }

        public override long Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(long))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            long extractedValue = BitConverter.ToInt64(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleLong(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleLong(long value)
        {
            const long MAX_EXPECTED_VALUE = 1000000000000;
            const long MIN_EXPECTED_VALUE = -1000000000000;
            return value >= MIN_EXPECTED_VALUE && value <= MAX_EXPECTED_VALUE;
        }

        public long GetData()
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
