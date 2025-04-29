using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Net
{
    [NetMessageClass(typeof(NetFloatMessage), MessageType.Float)]
    public class NetFloatMessage : BaseReflectionMessage<float>
    {
        float data;

        public NetFloatMessage(MessagePriority messagePriority, float data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Float;
            this.data = data;
        }

        public NetFloatMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Float;
            this.data = Deserialize(data);
        }

        public override float Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(float))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            float extractedValue = BitConverter.ToSingle(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleFloat(extractedValue))
                return data;

            return extractedValue;
        }

        private bool IsPlausibleFloat(float value)
        {
            const float MAX_EXPECTED = 1e20f;
            const float MIN_EXPECTED = -1e20f;
            return !float.IsNaN(value) && value >= MIN_EXPECTED && value <= MAX_EXPECTED;
        }

        public float GetData()
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
