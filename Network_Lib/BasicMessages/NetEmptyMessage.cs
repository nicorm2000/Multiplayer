using System.Collections.Generic;
using Net;

namespace Network_Lib.BasicMessages
{
    /// <summary>
    /// Represents an empty network message with no data payload.
    /// </summary>
    [NetMessageClass(typeof(NetEmptyMessage), MessageType.Empty)]
    internal class NetEmptyMessage : BaseReflectionMessage<Empty>
    {
        public Empty data = null;

        /// <summary>
        /// Initializes a new instance of the NetEmptyMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The empty data object.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetEmptyMessage(MessagePriority messagePriority, Empty data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Empty;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetEmptyMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetEmptyMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Empty;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data (no actual deserialization needed for empty messages).
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>An empty object.</returns>
        public override Empty Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            return new Empty();
        }

        /// <summary>
        /// Gets the empty data contained in the message.
        /// </summary>
        /// <returns>An empty object.</returns>
        public Empty GetData()
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
/// Represents an empty data structure for use with empty messages.
/// </summary>
public class Empty
{
    /// <summary>
    /// Initializes a new instance of the Empty class.
    /// </summary>
    public Empty() { }
}