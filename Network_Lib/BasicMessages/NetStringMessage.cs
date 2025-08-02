using System.Collections.Generic;
using System.Text;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a string value.
    /// </summary>
    [NetMessageClass(typeof(NetStringMessage), MessageType.String)]
    public class NetStringMessage : BaseReflectionMessage<string>
    {
        string data;

        /// <summary>
        /// Initializes a new instance of the NetStringMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The string data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetStringMessage(MessagePriority messagePriority, string data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.String;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetStringMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetStringMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.String;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a string value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized string value.</returns>
        public override string Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            bool checksumValid = MessageChecker.DeserializeCheckSum(message);

            int stringLength = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

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

        /// <summary>
        /// Validates that a string value is plausible (not empty, within size limits, and contains valid characters).
        /// </summary>
        /// <param name="value">The string value to validate.</param>
        /// <returns>True if the string is plausible; otherwise, false.</returns>
        private bool IsPlausibleString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            const int MAX_BYTES = 4096;
            if (Encoding.UTF8.GetByteCount(value) > MAX_BYTES)
                return false;

            foreach (char c in value)
            {
                if (char.IsControl(c) && !char.IsWhiteSpace(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the string data contained in the message.
        /// </summary>
        /// <returns>The string value.</returns>

        public string GetData()
        {
            return data;
        }

        /// <summary>
        /// Serializes the message into a byte array.
        /// </summary>
        /// <returns>The serialized message data.</returns>
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