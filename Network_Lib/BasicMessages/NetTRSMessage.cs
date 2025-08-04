using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing Transform (Position, Rotation, Scale) data.
    /// </summary>
    [NetMessageClass(typeof(NetTRSMessage), MessageType.TRS)]
    public class NetTRSMessage : BaseReflectionMessage<TRS>
    {
        TRS data;

        /// <summary>
        /// Initializes a new instance of the NetTRSMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The TRS data to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetTRSMessage(MessagePriority messagePriority, TRS data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.TRS;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetTRSMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetTRSMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.TRS;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into TRS (Transform) information.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized TRS data.</returns>
        public override TRS Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(int))
                return data;

            int messageHeaderSizeOffset = messageHeaderSize;
            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            TRS extractedValue = new TRS();

            extractedValue.position = (BitConverter.ToSingle(message, messageHeaderSizeOffset), 
                                       BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float)), 
                                       BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float) * 2));
            messageHeaderSizeOffset += sizeof(float) * 3;

            extractedValue.rotation = (BitConverter.ToSingle(message, messageHeaderSizeOffset), 
                                       BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float)), 
                                       BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float) * 2), 
                                       BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float) * 3));
            messageHeaderSizeOffset += sizeof(float) * 4;

            extractedValue.scale = (BitConverter.ToSingle(message, messageHeaderSizeOffset), 
                                    BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float)), 
                                    BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float) * 2));
            messageHeaderSizeOffset += sizeof(float) * 3;

            extractedValue.isActive = BitConverter.ToBoolean(message, messageHeaderSizeOffset);

            return extractedValue;
        }

        /// <summary>
        /// Gets the TRS data contained in the message.
        /// </summary>
        /// <returns>The TRS data.</returns>
        public TRS GetData()
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
            outData.AddRange(BitConverter.GetBytes(data.position.Item1));
            outData.AddRange(BitConverter.GetBytes(data.position.Item2));
            outData.AddRange(BitConverter.GetBytes(data.position.Item3));

            outData.AddRange(BitConverter.GetBytes(data.rotation.Item1));
            outData.AddRange(BitConverter.GetBytes(data.rotation.Item2));
            outData.AddRange(BitConverter.GetBytes(data.rotation.Item3));
            outData.AddRange(BitConverter.GetBytes(data.rotation.Item4));

            outData.AddRange(BitConverter.GetBytes(data.scale.Item1));
            outData.AddRange(BitConverter.GetBytes(data.scale.Item2));
            outData.AddRange(BitConverter.GetBytes(data.scale.Item3));

            outData.AddRange(BitConverter.GetBytes(data.isActive));
            
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
