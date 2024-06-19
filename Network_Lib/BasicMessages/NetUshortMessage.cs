﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetUShortMessage : BaseMessage<ushort>
    {
        ushort data;

        public NetUShortMessage(MessagePriority messagePriority, ushort data) : base(messagePriority)
        {
            currentMessageType = MessageType.Ushort;
            this.data = data;
        }

        public NetUShortMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Ushort;
            this.data = Deserialize(data);
        }

        public override ushort Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToUInt16(message, messageHeaderSize);
            }
            return data;
        }

        public ushort GetData()
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