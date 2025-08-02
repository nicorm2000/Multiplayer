using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing an unsigned short integer value.
    /// </summary>
    [NetMessageClass(typeof(NetUShortMessage), MessageType.Ushort)]
    public class NetUShortMessage : BaseReflectionMessage<ushort>
    {
        ushort data;

        /// <summary>
        /// Initializes a new instance of the NetUShortMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The unsigned short integer data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetUShortMessage(MessagePriority messagePriority, ushort data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Ushort;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetUShortMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetUShortMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Ushort;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into an unsigned short integer value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized unsigned short integer value.</returns>
        public override ushort Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(ushort))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            ushort extractedValue = BitConverter.ToUInt16(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleUShort(extractedValue))
                return data;

            return extractedValue;
        }

        /// <summary>
        /// Validates that an unsigned short integer value is within expected bounds.
        /// </summary>
        /// <param name="value">The unsigned short integer value to validate.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleUShort(ushort value)
        {
            const ushort MAX_EXPECTED_VALUE = 20000;
            return value <= MAX_EXPECTED_VALUE;
        }

        /// <summary>
        /// Gets the unsigned short integer data contained in the message.
        /// </summary>
        /// <returns>The unsigned short integer value.</returns>
        public ushort GetData()
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