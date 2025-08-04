using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing an unsigned long integer value.
    /// </summary>
    [NetMessageClass(typeof(NetULongMessage), MessageType.Ulong)]
    public class NetULongMessage : BaseReflectionMessage<ulong>
    {
        ulong data;

        /// <summary>
        /// Initializes a new instance of the NetULongMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The unsigned long integer data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetULongMessage(MessagePriority messagePriority, ulong data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Ulong;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetULongMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetULongMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Ulong;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into an unsigned long integer value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized unsigned long integer value.</returns>
        public override ulong Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(ulong))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            ulong extractedValue = BitConverter.ToUInt64(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleULong(extractedValue))
                return data;

            return extractedValue;
        }

        /// <summary>
        /// Validates that an unsigned long integer value is within expected bounds.
        /// </summary>
        /// <param name="value">The unsigned long integer value to validate.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleULong(ulong value)
        {
            const ulong MAX_EXPECTED_VALUE = 1000000000000;
            return value <= MAX_EXPECTED_VALUE;
        }

        /// <summary>
        /// Gets the unsigned long integer data contained in the message.
        /// </summary>
        /// <returns>The unsigned long integer value.</returns>
        public ulong GetData()
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