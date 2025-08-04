using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a long integer value.
    /// </summary>
    [NetMessageClass(typeof(NetLongMessage), MessageType.Long)]
    public class NetLongMessage : BaseReflectionMessage<long>
    {
        long data;

        /// <summary>
        /// Initializes a new instance of the NetLongMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The long data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetLongMessage(MessagePriority messagePriority, long data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Long;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetLongMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetLongMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Long;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a long value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized long value.</returns>
        public override long Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(long))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            long extractedValue = BitConverter.ToInt64(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleLong(extractedValue))
                return data;

            return extractedValue;
        }

        /// <summary>
        /// Validates that a long value is within expected bounds.
        /// </summary>
        /// <param name="value">The long value to validate.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleLong(long value)
        {
            const long MAX_EXPECTED_VALUE = 1000000000000;
            const long MIN_EXPECTED_VALUE = -1000000000000;
            return value >= MIN_EXPECTED_VALUE && value <= MAX_EXPECTED_VALUE;
        }

        /// <summary>
        /// Gets the long data contained in the message.
        /// </summary>
        /// <returns>The long value.</returns>
        public long GetData()
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