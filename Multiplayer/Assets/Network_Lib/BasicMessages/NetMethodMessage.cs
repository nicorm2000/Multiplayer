using Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Network_Lib.BasicMessages
{
    [NetMessageClass(typeof(NetMethodMessage), MessageType.Method)]
    public class NetMethodMessage : BaseReflectionMessage<(int, List<(string, string)>)>
    {
        private (int, List<(string, string)>) data = (0, new List<(string, string)>());

        public NetMethodMessage(MessagePriority messagePriority, (int, List<(string, string)>) data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Method;
            this.data = data;
        }

        public NetMethodMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Method;
            this.data = Deserialize(data);
        }

        public override (int, List<(string, string)>) Deserialize(byte[] message)
        {
            bool bypassChecksum = true;

            if (!bypassChecksum && !MessageChecker.DeserializeCheckSum(message))
            {
                return (0, new List<(string, string)>());
            }

            // Proceed with deserialization...
            DeserializeHeader(message);
            data.Item1 = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            int paramCount = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            for (int i = 0; i < paramCount; i++)
            {
                string type = MessageChecker.DeserializeString(message, ref messageHeaderSize);
                string value = MessageChecker.DeserializeString(message, ref messageHeaderSize);
                data.Item2.Add((type, value));
            }

            return data;
        }

        public (int, List<(string, string)>) GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> messageWithoutChecksum = new List<byte>();

            SerializeHeader(ref messageWithoutChecksum);
            messageWithoutChecksum.AddRange(BitConverter.GetBytes(data.Item1));
            messageWithoutChecksum.AddRange(BitConverter.GetBytes(data.Item2.Count));

            foreach (var (type, value) in data.Item2)
            {
                messageWithoutChecksum.AddRange(MessageChecker.SerializeString(type.ToCharArray()));
                messageWithoutChecksum.AddRange(MessageChecker.SerializeString(value.ToCharArray()));
            }

            int finalLength = messageWithoutChecksum.Count + sizeof(int);

            List<byte> tempForChecksum = new List<byte>(messageWithoutChecksum);
            tempForChecksum.AddRange(new byte[sizeof(int)]);

            byte[] checksum = MessageChecker.SerializeCheckSum(tempForChecksum);

            List<byte> finalMessage = new List<byte>(messageWithoutChecksum);
            finalMessage.AddRange(checksum);

            return finalMessage.ToArray();
        }
    }
}
