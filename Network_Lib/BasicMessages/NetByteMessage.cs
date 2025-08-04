using System.Collections.Generic;

namespace Net
{
    /// <summary>
    /// Represents a network message containing a byte value.
    /// </summary>
    [NetMessageClass(typeof(NetByteMessage), MessageType.Byte)]
    public class NetByteMessage : BaseReflectionMessage<byte>
    {
        byte data;

        /// <summary>
        /// Initializes a new instance of the NetByteMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The byte data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetByteMessage(MessagePriority messagePriority, byte data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Byte;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetByteMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetByteMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Byte;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into a byte value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized byte value.</returns>
        public override byte Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(byte))
                return data;

            return message[messageHeaderSize];
        }

        /// <summary>
        /// Gets the byte data contained in the message.
        /// </summary>
        /// <returns>The byte value.</returns>
        public byte GetData()
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
            outData.Add(data);
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}