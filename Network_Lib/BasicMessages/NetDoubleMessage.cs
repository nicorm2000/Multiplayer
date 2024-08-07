﻿using System;
using System.Collections.Generic;

namespace Net
{
    [NetMessageClass(typeof(NetDoubleMessage), MessageType.Double)]
    public class NetDoubleMessage : BaseReflectionMessage<double>
    {
        double data;

        public NetDoubleMessage(MessagePriority messagePriority, double data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Double;
            this.data = data;
        }

        public NetDoubleMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Double;
            this.data = Deserialize(data);
        }

        public override double Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToDouble(message, messageHeaderSize);
            }
            return data;
        }

        public double GetData()
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
