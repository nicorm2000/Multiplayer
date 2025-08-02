using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a decimal value.
    /// </summary>v
    [NetMessageClass(typeof(NetDecimalMessage), MessageType.Decimal)]
    public class NetDecimalMessage : BaseReflectionMessage<decimal>
    {
        decimal data;

        /// <summary>
        /// Initializes a new instance of the NetDecimalMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The decimal data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetDecimalMessage(MessagePriority messagePriority, decimal data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Decimal;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetDecimalMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetDecimalMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Decimal;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a decimal value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized decimal value.</returns>
        public override decimal Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            const int decimalSize = 16;
            if (message.Length < messageHeaderSize + decimalSize)
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            if (!checksumValid)
                return data;

            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
            {
                bits[i] = BitConverter.ToInt32(message, messageHeaderSize + i * 4);
            }
            return new decimal(bits);
        }

        /// <summary>
        /// Gets the decimal data contained in the message.
        /// </summary>
        /// <returns>The decimal value.</returns>
        public decimal GetData()
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

            int[] bits = decimal.GetBits(data);
            foreach (int bit in bits)
            {
                outData.AddRange(BitConverter.GetBytes(bit));
            }

            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}