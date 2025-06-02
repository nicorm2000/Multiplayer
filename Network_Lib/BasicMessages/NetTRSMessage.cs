using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetTRSMessage), MessageType.TRS)]
    public class NetTRSMessage : BaseReflectionMessage<TRS>
    {
        TRS data;

        public NetTRSMessage(MessagePriority messagePriority, TRS data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.TRS;
            this.data = data;
        }

        public NetTRSMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.TRS;
            this.data = Deserialize(data);
        }

        public override TRS Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (message.Length < messageHeaderSize + sizeof(int))
                return data;

            int messageHeaderSizeOffset = messageHeaderSize;
            bool checksumValid = MessageChecker.DeserializeCheckSum(message);
            TRS extractedValue = new TRS();
            extractedValue.position = (BitConverter.ToSingle(message, messageHeaderSizeOffset), BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float)), BitConverter.ToSingle(message, messageHeaderSizeOffset) + sizeof(float) * 2);
            messageHeaderSizeOffset += sizeof(float) * 3;
            extractedValue.rotation = (BitConverter.ToSingle(message, messageHeaderSizeOffset), BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float)), BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float) * 2), BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float) * 3));
            messageHeaderSizeOffset += sizeof(float) * 4;
            extractedValue.scale = (BitConverter.ToSingle(message, messageHeaderSizeOffset), BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float)), BitConverter.ToSingle(message, messageHeaderSizeOffset + sizeof(float) * 2));
            messageHeaderSizeOffset += sizeof(float) * 3;
            extractedValue.isActive = BitConverter.ToBoolean(message, messageHeaderSizeOffset);

            return extractedValue;
        }

        public TRS GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);
            outData.AddRange(BitConverter.GetBytes(data.position.Item1));
            outData.AddRange(BitConverter.GetBytes(data.position.Item2));
            outData.AddRange(BitConverter.GetBytes(data.position.Item3));
            outData.AddRange(BitConverter.GetBytes(data.rotation.Item1));
            outData.AddRange(BitConverter.GetBytes(data.rotation.Item2));
            outData.AddRange(BitConverter.GetBytes(data.rotation.Item3));
            outData.AddRange(BitConverter.GetBytes(data.rotation.Item4));
            outData.AddRange(BitConverter.GetBytes(data.scale.Item1));
            outData.AddRange(BitConverter.GetBytes(data.scale.Item2));
            outData.AddRange(BitConverter.GetBytes(data.scale.Item3));
            outData.AddRange(BitConverter.GetBytes(data.isActive));
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }
}
