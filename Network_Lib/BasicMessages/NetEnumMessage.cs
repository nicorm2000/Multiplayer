using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Initializes a new instance of the Empty class.
    /// </summary>
    [NetMessageClass(typeof(NetEnumMessage), MessageType.Enum)]
    public class NetEnumMessage : BaseReflectionMessage<Enum>
    {
        private Enum data;
        private string enumTypeName;

        /// <summary>
        /// Initializes a new instance of the NetEnumMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The enumeration value to send.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetEnumMessage(MessagePriority messagePriority, Enum data, List<RouteInfo> messageRoute)
            : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Enum;
            this.data = data;
            this.enumTypeName = data.GetType().AssemblyQualifiedName;
        }

        /// <summary>
        /// Initializes a new instance of the NetEnumMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetEnumMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Enum;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into an enumeration value.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>The deserialized enumeration value.</returns>
        public override Enum Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            int offset = messageHeaderSize;

            // Read enum type name
            int typeNameLength = BitConverter.ToInt32(message, offset);
            offset += sizeof(int);
            string receivedEnumTypeName = System.Text.Encoding.UTF8.GetString(message, offset, typeNameLength);
            offset += typeNameLength;

            // Read enum value
            int enumValue = BitConverter.ToInt32(message, offset);

            Type enumType = Type.GetType(receivedEnumTypeName);
            if (enumType == null || !enumType.IsEnum)
            {
                return null;
            }

            return (Enum)Enum.ToObject(enumType, enumValue);
        }

        /// <summary>
        /// Gets the enumeration data contained in the message.
        /// </summary>
        /// <returns>The enumeration value.</returns>
        public Enum GetData()
        {
            return data;
        }

        /// <summary>
        /// Gets the enumeration data contained in the message.
        /// </summary>
        /// <returns>The enumeration value.</returns>
        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);

            // Write enum type name
            byte[] typeNameBytes = System.Text.Encoding.UTF8.GetBytes(enumTypeName);
            outData.AddRange(BitConverter.GetBytes(typeNameBytes.Length));
            outData.AddRange(typeNameBytes);

            // Write enum value (as underlying int)
            outData.AddRange(BitConverter.GetBytes(Convert.ToInt32(data)));

            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}