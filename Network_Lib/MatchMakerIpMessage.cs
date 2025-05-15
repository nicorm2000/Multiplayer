using System.Collections.Generic;

namespace Net
{
    public class MatchMakerIpMessage : BaseMessage<string>
    {
        string ipString;

        public MatchMakerIpMessage(MessagePriority priority, string ip) : base(priority)
        {
            currentMessageType = MessageType.MatchMakerIp;
            ipString = ip;
        }

        public MatchMakerIpMessage(byte[] message) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.MatchMakerIp;
            ipString = Deserialize(message);
        }

        public override string Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            if (MessageChecker.DeserializeCheckSum(message))
            {
                return MessageChecker.DeserializeString(message, ref messageHeaderSize);
            }
            return null;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);
            outData.AddRange(MessageChecker.SerializeString(ipString.ToCharArray()));
            SerializeQueue(ref outData);
            return outData.ToArray();
        }

        public string GetData()
        {
            return ipString;
        }
    }
}
