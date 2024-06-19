﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetULongMessage : BaseMessage<ulong>
    {
        ulong data;

        public NetULongMessage(MessagePriority messagePriority, ulong data) : base(messagePriority)
        {
            currentMessageType = MessageType.Ulong;
            this.data = data;
        }

        public NetULongMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Ulong;
            this.data = Deserialize(data);
        }

        public override ulong Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToUInt64(message, messageHeaderSize);
            }
            return data;
        }

        public ulong GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
