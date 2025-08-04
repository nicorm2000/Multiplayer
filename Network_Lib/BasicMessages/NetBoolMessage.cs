using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a boolean value.
    /// </summary>
    [NetMessageClass(typeof(NetBoolMessage), MessageType.Bool)]
    public class NetBoolMessage : BaseReflectionMessage<bool>
    {
        bool data;

        /// <summary>
        /// Initializes a new instance of the NetBoolMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The boolean data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetBoolMessage(MessagePriority messagePriority, bool data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Bool;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetBoolMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetBoolMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Bool;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a boolean value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized boolean value.</returns>
        public override bool Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(bool))
                return data;

            return BitConverter.ToBoolean(message, messageHeaderSize);
        }

        /// <summary>
        /// Gets the boolean data contained in the message.
        /// </summary>
        /// <returns>The boolean value.</returns>
        public bool GetData()
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