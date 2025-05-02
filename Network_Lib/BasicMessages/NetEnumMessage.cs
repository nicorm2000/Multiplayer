using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetEnumMessage), MessageType.Enum)]
    public class NetEnumMessage : BaseReflectionMessage<Enum>
    {
        private Enum data;
        private string enumTypeName;

        public NetEnumMessage(MessagePriority messagePriority, Enum data, List<RouteInfo> messageRoute)
            : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Enum;
            this.data = data;
            this.enumTypeName = data.GetType().AssemblyQualifiedName;
        }

        public NetEnumMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Enum;
            this.data = Deserialize(data);
        }

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

        public Enum GetData()
        {
            return data;
        }

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