using System;
using System.Collections.Generic;

namespace Net
{
    [Serializable]
    public class MatchMakerPlayerListUpdateMessage : BaseMessage<char[]>
    {
        private string data;

        public MatchMakerPlayerListUpdateMessage(MessagePriority priority, List<string> usernames) : base(priority)
        {
            currentMessageType = MessageType.MatchMakerPlayerListUpdate;
            data = string.Join("|", usernames);
        }

        public MatchMakerPlayerListUpdateMessage(byte[] message) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.MatchMakerPlayerListUpdate;
            data = new string(Deserialize(message));
        }

        public List<string> GetData()
        {
            return string.IsNullOrEmpty(data)
                ? new List<string>()
                : new List<string>(data.Split('|'));
        }

        public override char[] Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                return MessageChecker.DeserializeString(message, ref messageHeaderSize).ToCharArray();
            }

            return Array.Empty<char>();
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);

            outData.AddRange(MessageChecker.SerializeString(data.ToCharArray()));

            byte[] checksum = MessageChecker.SerializeCheckSum(outData);
            outData.AddRange(checksum);

            return outData.ToArray();
        }
    }
}
