using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetShortMessage), MessageType.Short)]
    public class NetShortMessage : BaseReflectionMessage<short>
    {
        short data;

        public NetShortMessage(MessagePriority messagePriority, short data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Short;
            this.data = data;
        }

        public NetShortMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Short;
            this.data = Deserialize(data);
        }

        public override short Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(short))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            short extractedValue = BitConverter.ToInt16(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleShort(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleShort(short value)
        {
            const short MAX_EXPECTED_VALUE = 10000;
            const short MIN_EXPECTED_VALUE = -10000;
            return value >= MIN_EXPECTED_VALUE && value <= MAX_EXPECTED_VALUE;
        }

        public short GetData()
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
