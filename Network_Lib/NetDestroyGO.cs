using System;
using System.Collections.Generic;

namespace Net
{
    public class NetDestroyGO : BaseMessage<(int, int)>
    {
        (int, int) value;

        public NetDestroyGO(MessagePriority messagePriority, (int, int) data) : base(messagePriority)
        {
            currentMessageType = MessageType.DestroyNetObj;
            this.value = data;
        }

        public NetDestroyGO(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.DestroyNetObj;
            this.value = Deserialize(data);
        }

        public override (int, int) Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                value.Item1 = BitConverter.ToInt32(message, messageHeaderSize);
                value.Item2 = BitConverter.ToInt32(message, messageHeaderSize);
            }
            return value;
        }

        public (int, int) GetData()
        {
            return value;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(value.Item1));
            outData.AddRange(BitConverter.GetBytes(value.Item2));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}