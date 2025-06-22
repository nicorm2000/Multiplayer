using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetStringMessage), MessageType.String)]
    public class NetStringMessage : BaseReflectionMessage<string>
    {
        string data;

        public NetStringMessage(MessagePriority messagePriority, string data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.String;
            this.data = data;
        }

        public NetStringMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.String;
            this.data = Deserialize(data);
        }

        public override string Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            bool checksumValid = MessageChecker.DeserializeCheckSum(message);

            // Get string length (first 4 bytes after header)
            int stringLength = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            // Ensure we have enough data
            if (message.Length < messageHeaderSize + stringLength)
            {
                return string.Empty;
            }

            string text = Encoding.UTF8.GetString(message, messageHeaderSize, stringLength);
            messageHeaderSize += stringLength;

            if (checksumValid || IsPlausibleString(text))
            {
                return text;
            }
            return string.Empty;
        }

        private bool IsPlausibleString(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            const int MAX_EXPECTED_LENGTH = 1024;
            if (value.Length > MAX_EXPECTED_LENGTH) return false;

            foreach (char c in value)
            {
                if (char.IsControl(c) && !char.IsWhiteSpace(c))
                    return false;
            }

            return true;
        }


        public string GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);

            byte[] stringData = Encoding.UTF8.GetBytes(data);
            outData.AddRange(BitConverter.GetBytes(stringData.Length));
            outData.AddRange(stringData);

            byte[] checksum = MessageChecker.SerializeCheckSum(outData);
            outData.AddRange(checksum);

            return outData.ToArray();
        }
    }
}
