using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a short integer value.
    /// </summary>
    [NetMessageClass(typeof(NetShortMessage), MessageType.Short)]
    public class NetShortMessage : BaseReflectionMessage<short>
    {
        short data;

        /// <summary>
        /// Initializes a new instance of the NetShortMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The short integer data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetShortMessage(MessagePriority messagePriority, short data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Short;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetShortMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetShortMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Short;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a short integer value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized short integer value.</returns>
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

        /// <summary>
        /// Validates that a short integer value is within expected bounds.
        /// </summary>
        /// <param name="value">The short integer value to validate.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleShort(short value)
        {
            const short MAX_EXPECTED_VALUE = 10000;
            const short MIN_EXPECTED_VALUE = -10000;
            return value >= MIN_EXPECTED_VALUE && value <= MAX_EXPECTED_VALUE;
        }

        /// <summary>
        /// Gets the short integer data contained in the message.
        /// </summary>
        /// <returns>The short integer value.</returns>
        public short GetData()
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
