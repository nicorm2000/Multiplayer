using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    /// <summary>
    /// Represents a ping message used for network latency checking.
    /// </summary>
    public class NetPing
    {
        MessageType messageType = MessageType.Ping;

        /// <summary>
        /// Gets the message type (always MessageType.Ping).
        /// </summary>
        /// <returns>The message type.</returns>
        public MessageType GetMessageType()
        {
            return messageType;
        }

        /// <summary>
        /// Serializes the ping message into a byte array.
        /// </summary>
        /// <returns>The serialized message data.</returns>
        public byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(BitConverter.GetBytes((int)MessagePriority.Default));

            return outData.ToArray();
        }
    }
}