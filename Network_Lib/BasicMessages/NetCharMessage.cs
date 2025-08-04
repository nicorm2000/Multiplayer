using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a character value.
    /// </summary>
    [NetMessageClass(typeof(NetCharMessage), MessageType.Char)]
    public class NetCharMessage : BaseReflectionMessage<char>
    {
        char data;

        /// <summary>
        /// Initializes a new instance of the NetCharMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The character data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetCharMessage(MessagePriority messagePriority, char data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Char;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetCharMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetCharMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Char;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a character value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized character value.</returns>
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

        /// <summary>
        /// Checks if a character value is plausible (not a control character unless whitespace).
        /// </summary>
        /// <param name="value">The character to check.</param>
        /// <returns>True if the character is plausible; otherwise, false.</returns>
        private bool IsPlausibleChar(char value)
        {
            return !char.IsControl(value) || char.IsWhiteSpace(value);
        }

        /// <summary>
        /// Gets the character data contained in the message.
        /// </summary>
        /// <returns>The character value.</returns>
        public char GetData()
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
            outData.AddRange(BitConverter.GetBytes(data));
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
