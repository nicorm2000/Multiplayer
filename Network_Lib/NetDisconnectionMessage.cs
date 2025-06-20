using System.Collections.Generic;

namespace Net
{
    public class NetDisconnectionMessage : BaseMessage<DisconnectAll>
    {
        DisconnectAll disconnectAll;

        public NetDisconnectionMessage(DisconnectAll disconnect) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.DisconnectAll;
            this.disconnectAll = disconnect;
        }

        public NetDisconnectionMessage(byte[] message) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.DisconnectAll;
            disconnectAll = Deserialize(message);
        }

        public override DisconnectAll Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            return new DisconnectAll();
        }

        public DisconnectAll GetData()
        {
            return disconnectAll;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}

public class DisconnectAll
{
    public DisconnectAll() { }
}