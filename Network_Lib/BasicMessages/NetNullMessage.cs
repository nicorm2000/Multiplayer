using System.Collections.Generic;
using Net;

namespace Network_Lib.BasicMessages
{
    /// <summary>
    /// Represents a network message containing a null value.
    /// </summary>
    [NetMessageClass(typeof(NetNullMessage), MessageType.Null)]
    public class NetNullMessage : BaseReflectionMessage<Null>
    {
        public Null data = null;

        /// <summary>
        /// Initializes a new instance of the NetNullMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The null data object.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetNullMessage(MessagePriority messagePriority, Null data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Null;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetNullMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetNullMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Null;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data (no actual deserialization needed for null messages).
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>A null object.</returns>
        public override Null Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            return new Null();
        }

        /// <summary>
        /// Gets the null data contained in the message.
        /// </summary>
        /// <returns>A null object.</returns>
        public Null GetData()
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
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}

/// <summary>
/// Represents a null data structure for use with null messages.
/// </summary>
public class Null
{
    /// <summary>
    /// Initializes a new instance of the Null class.
    /// </summary>
    public Null() { }
}