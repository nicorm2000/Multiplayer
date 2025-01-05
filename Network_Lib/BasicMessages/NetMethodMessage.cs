using Net;
using System;
using System.Collections.Generic;

namespace Network_Lib.BasicMessages
{
    [NetMessageClass(typeof(NetMethodMessage), MessageType.Method)]
    public class NetMethodMessage : BaseReflectionMessage<(int, List<(string, string)>)>
    {
        (int, List<(string, string)>) data;

        public NetMethodMessage(MessagePriority messagePriority, (int, List<(string, string)>) data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Method;
            this.data = data;
        }

        public NetMethodMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Sbyte;
            this.data = Deserialize(data);
        }

        public override (int, List<(string, string)>) Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data.Item1 = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += 4;
                int listSize = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += 4;
                List<(string, string)> list = new List<(string, string)>();

                for (int i = 0; i < listSize; i++)
                {
                    string type = MessageChecker.DeserializeStringPerChar(message, ref messageHeaderSize);
                    string typeData = MessageChecker.DeserializeStringPerChar(message, ref messageHeaderSize);

                    list.Add((type, typeData));
                }

                data.Item2 = list;
            }

            return data;
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

            foreach ((string type, string data) item in data.Item2)
            {
                MessageChecker.SerializeStringPerChar(item.type.ToCharArray());
                MessageChecker.SerializeStringPerChar(item.data.ToCharArray());
            }

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
