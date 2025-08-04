using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a double-precision floating-point value.
    /// </summary>
    [NetMessageClass(typeof(NetDoubleMessage), MessageType.Double)]
    public class NetDoubleMessage : BaseReflectionMessage<double>
    {
        double data;

        /// <summary>
        /// Initializes a new instance of the NetDoubleMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The double data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetDoubleMessage(MessagePriority messagePriority, double data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Double;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetDoubleMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetDoubleMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Double;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a double value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized double value.</returns>
        public override double Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(double))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            double extractedValue = BitConverter.ToDouble(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleDouble(extractedValue))
                return data;

            return extractedValue;
        }

        /// <summary>
        /// Checks if a double value is plausible (not NaN or infinity).
        /// </summary>
        /// <param name="value">The double value to check.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleDouble(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// Gets the double data contained in the message.
        /// </summary>
        /// <returns>The double value.</returns>
        public double GetData()
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