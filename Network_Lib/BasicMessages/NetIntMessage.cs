using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing an integer value.
    /// </summary>
    [NetMessageClass(typeof(NetIntMessage), MessageType.Int)]
    public class NetIntMessage : BaseReflectionMessage<int>
    {
        int data;

        /// <summary>
        /// Initializes a new instance of the NetIntMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The integer data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetIntMessage(MessagePriority messagePriority, int data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Int;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetIntMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetIntMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Int;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into an integer value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized integer value.</returns>
        public override int Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            bool checksumValid = MessageChecker.DeserializeCheckSum(message);

            if (message.Length < messageHeaderSize + sizeof(int))
                return default;

            int extractedValue = BitConverter.ToInt32(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleInt(extractedValue))
                return default;

            return extractedValue;
        }

        /// <summary>
        /// Validates that an integer value is within expected bounds.
        /// </summary>
        /// <param name="value">The integer value to validate.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleInt(int value)
        {
            const int MAX_EXPECTED_VALUE = 1000000;
            const int MIN_EXPECTED_VALUE = -1000000;

            return value >= MIN_EXPECTED_VALUE && value <= MAX_EXPECTED_VALUE;
        }

        /// <summary>
        /// Gets the integer data contained in the message.
        /// </summary>
        /// <returns>The integer value.</returns>
        public int GetData()
        {
            return data;
        }

        /// <summary>
        /// Gets the integer data contained in the message.
        /// </summary>
        /// <returns>The integer value.</returns>
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