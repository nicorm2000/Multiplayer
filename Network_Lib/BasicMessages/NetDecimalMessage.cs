using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetDecimalMessage), MessageType.Decimal)]
    public class NetDecimalMessage : BaseReflectionMessage<decimal>
    {
        decimal data;

        public NetDecimalMessage(MessagePriority messagePriority, decimal data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Decimal;
            this.data = data;
        }

        public NetDecimalMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Decimal;
            this.data = Deserialize(data);
        }

        public override decimal Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            const int decimalSize = 16;
            if (message.Length < messageHeaderSize + decimalSize)
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            if (!checksumValid)
                return data;

            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
            {
                bits[i] = BitConverter.ToInt32(message, messageHeaderSize + i * 4);
            }
            return new decimal(bits);
        }

        public decimal GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);

            int[] bits = decimal.GetBits(data);
            foreach (int bit in bits)
            {
                outData.AddRange(BitConverter.GetBytes(bit));
            }

            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
