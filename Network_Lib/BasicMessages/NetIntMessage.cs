using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetIntMessage), MessageType.Int)]
    public class NetIntMessage : BaseReflectionMessage<int>
    {
        int data;

        public NetIntMessage(MessagePriority messagePriority, int data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Int;
            this.data = data;
        }

        public NetIntMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Int;
            this.data = Deserialize(data);
        }

        public override int Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            bool checksumValid = MessageChecker.DeserializeCheckSum(message);

            if (message.Length < messageHeaderSize + sizeof(int))
                return default;

            int extractedValue = BitConverter.ToInt32(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleInt(extractedValue))
                return default;

            return extractedValue;
        }

        private bool IsPlausibleInt(int value)
        {
            const int MAX_EXPECTED_VALUE = 1000000;
            const int MIN_EXPECTED_VALUE = -1000000;

            return value >= MIN_EXPECTED_VALUE && value <= MAX_EXPECTED_VALUE;
        }

        public int GetData()
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
