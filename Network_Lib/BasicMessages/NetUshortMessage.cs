using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetUShortMessage), MessageType.Ushort)]
    public class NetUShortMessage : BaseReflectionMessage<ushort>
    {
        ushort data;

        public NetUShortMessage(MessagePriority messagePriority, ushort data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Ushort;
            this.data = data;
        }

        public NetUShortMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Ushort;
            this.data = Deserialize(data);
        }

        public override ushort Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(ushort))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            ushort extractedValue = BitConverter.ToUInt16(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleUShort(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleUShort(ushort value)
        {
            const ushort MAX_EXPECTED_VALUE = 20000;
            return value <= MAX_EXPECTED_VALUE;
        }

        public ushort GetData()
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
