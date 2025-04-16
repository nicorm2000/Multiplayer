using Net;
using System.Collections.Generic;

namespace Network_Lib.BasicMessages
{
    [NetMessageClass(typeof(NetNullMessage), MessageType.Null)]
    public class NetNullMessage : BaseReflectionMessage<Null>
    {
        public Null data = null;

        public NetNullMessage(MessagePriority messagePriority, Null data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Null;
            this.data = data;
        }

        public NetNullMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Null;
            this.data = Deserialize(data);
        }

        public override Null Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            return data;
        }

        public override Null GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}

public class Null
{
    public Null() { }
}