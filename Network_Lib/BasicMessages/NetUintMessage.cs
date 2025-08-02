using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing an unsigned integer value.
    /// </summary>
    [NetMessageClass(typeof(NetUIntMessage), MessageType.Uint)]
    public class NetUIntMessage : BaseReflectionMessage<uint>
    {
        uint data;

        /// <summary>
        /// Initializes a new instance of the NetUIntMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The unsigned integer data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetUIntMessage(MessagePriority messagePriority, uint data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Uint;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetUIntMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetUIntMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Uint;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into an unsigned integer value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized unsigned integer value.</returns>
        public override uint Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(uint))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            uint extractedValue = BitConverter.ToUInt32(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleUInt(extractedValue))
                return data;

            return extractedValue;
        }

        /// <summary>
        /// Validates that an unsigned integer value is within expected bounds.
        /// </summary>
        /// <param name="value">The unsigned integer value to validate.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleUInt(uint value)
        {
            const uint MAX_EXPECTED_VALUE = 1000000;
            return value <= MAX_EXPECTED_VALUE;
        }

        /// <summary>
        /// Gets the unsigned integer data contained in the message.
        /// </summary>
        /// <returns>The unsigned integer value.</returns>
        public uint GetData()
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