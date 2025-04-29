using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetCharMessage), MessageType.Char)]
    public class NetCharMessage : BaseReflectionMessage<char>
    {
        char data;

        public NetCharMessage(MessagePriority messagePriority, char data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Char;
            this.data = data;
        }

        public NetCharMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Char;
            this.data = Deserialize(data);
        }

        public override char Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(char))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            char extractedValue = BitConverter.ToChar(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleChar(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleChar(char value)
        {
            return value >= 32 && value <= 126;
        }

        public char GetData()
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
