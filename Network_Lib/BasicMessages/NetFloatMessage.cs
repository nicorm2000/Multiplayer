using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a single-precision floating-point value.
    /// </summary>
    [NetMessageClass(typeof(NetFloatMessage), MessageType.Float)]
    public class NetFloatMessage : BaseReflectionMessage<float>
    {
        float data;

        /// <summary>
        /// Initializes a new instance of the NetFloatMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The float data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetFloatMessage(MessagePriority messagePriority, float data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Float;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetFloatMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetFloatMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Float;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a float value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized float value.</returns>
        public override float Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(float))
                return data;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            float extractedValue = BitConverter.ToSingle(message, messageHeaderSize);

            if (!checksumValid && !IsPlausibleFloat(extractedValue))
                return data;

            return extractedValue;
        }

        /// <summary>
        /// Validates that a float value is plausible (not NaN or infinity).
        /// </summary>
        /// <param name="value">The float value to validate.</param>
        /// <returns>True if the value is plausible; otherwise, false.</returns>
        private bool IsPlausibleFloat(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        /// <summary>
        /// Gets the float data contained in the message.
        /// </summary>
        /// <returns>The float value.</returns>
        public float GetData()
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