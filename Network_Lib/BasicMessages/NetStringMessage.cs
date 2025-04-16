using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetStringMessage), MessageType.String)]
    public class NetStringMessage : BaseReflectionMessage<string>
    {
        string data;

        public NetStringMessage(MessagePriority messagePriority, string data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.String;
            this.data = data;
        }

        public NetStringMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.String;
            this.data = Deserialize(data);
        }

        public override string Deserialize(byte[] message)
        {
            string text = "";

            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                text = MessageChecker.DeserializeString(message, ref messageHeaderSize);
            }

            return text;
        }

        public override string GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(MessageChecker.SerializeString(data.ToCharArray()));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
