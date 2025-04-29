using Net;
using System.Collections.Generic;

namespace Network_Lib.BasicMessages
{
    [NetMessageClass(typeof(NetEmptyMessage), MessageType.Empty)]
    internal class NetEmptyMessage : BaseReflectionMessage<Empty>
    {
        public Empty data = null;

        public NetEmptyMessage(MessagePriority messagePriority, Empty data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Empty;
            this.data = data;
        }

        public NetEmptyMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Empty;
            this.data = Deserialize(data);
        }

        public override Empty Deserialize(byte[] message)
        {
            DeserializeHeader(message);
            return new Empty();
        }

        public Empty GetData()
        {
            return data;
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

public class Empty
{
    public Empty() { }
}