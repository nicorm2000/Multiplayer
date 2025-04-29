using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetDoubleMessage), MessageType.Double)]
    public class NetDoubleMessage : BaseReflectionMessage<double>
    {
        double data;

        public NetDoubleMessage(MessagePriority messagePriority, double data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Double;
            this.data = data;
        }

        public NetDoubleMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Double;
            this.data = Deserialize(data);
        }

        public override double Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(double))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            double extractedValue = BitConverter.ToDouble(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleDouble(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleDouble(double value)
        {
            const double MAX_EXPECTED = 1e100;
            const double MIN_EXPECTED = -1e100;
            return !double.IsNaN(value) && value >= MIN_EXPECTED && value <= MAX_EXPECTED;
        }

        public double GetData()
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
