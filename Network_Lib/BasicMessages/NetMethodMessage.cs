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
            DeserializeHeader(message);

            if (!MessageChecker.DeserializeCheckSum(message))
                return (0, new List<(string, string)>());

            if (message.Length < messageHeaderSize + sizeof(int))
                return (0, new List<(string, string)>());

            int methodId = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            if (message.Length < messageHeaderSize + sizeof(int))
                return (0, new List<(string, string)>());

            int paramCount = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            var parameters = new List<(string, string)>();
            for (int i = 0; i < paramCount; i++)
            {
                if (message.Length < messageHeaderSize + sizeof(int))
                    break;

                string type = MessageChecker.DeserializeString(message, ref messageHeaderSize);
                string value = MessageChecker.DeserializeString(message, ref messageHeaderSize);

                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(value))
                {
                    parameters.Add((type, value));
                }
            }

            return (methodId, parameters);
        }

        public (int, List<(string, string)>) GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data.Item1));
            outData.AddRange(BitConverter.GetBytes(data.Item2.Count));

            foreach ((string type, string value) in data.Item2)
            {
                outData.AddRange(MessageChecker.SerializeString(type.ToCharArray()));
                outData.AddRange(MessageChecker.SerializeString(value.ToCharArray()));
            }

            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
