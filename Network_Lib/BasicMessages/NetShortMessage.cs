﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetShortMessage : BaseMessage<short>
    {
        short data;

        public NetShortMessage(MessagePriority messagePriority, short data) : base(messagePriority)
        {
            currentMessageType = MessageType.Short;
            this.data = data;
        }

        public NetShortMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Short;
            this.data = Deserialize(data);
        }

        public override short Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToInt16(message, messageHeaderSize);
            }
            return data;
        }

        public short GetData()
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