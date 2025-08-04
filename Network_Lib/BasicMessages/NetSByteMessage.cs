using System.Collections.Generic;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a signed byte value.
    /// </summary>
    [NetMessageClass(typeof(NetSByteMessage), MessageType.Sbyte)]
    public class NetSByteMessage : BaseReflectionMessage<sbyte>
    {
        sbyte data;

        /// <summary>
        /// Initializes a new instance of the NetSByteMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The signed byte data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetSByteMessage(MessagePriority messagePriority, sbyte data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Sbyte;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetSByteMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetSByteMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Sbyte;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a signed byte value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized signed byte value.</returns>
        public override sbyte Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(sbyte))
                return data;

            return (sbyte)message[messageHeaderSize];
        }

        /// <summary>
        /// Gets the signed byte data contained in the message.
        /// </summary>
        /// <returns>The signed byte value.</returns>
        public sbyte GetData()
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
            outData.Add((byte)data);
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}