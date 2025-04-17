using Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Network_Lib.BasicMessages
{
    [NetMessageClass(typeof(NetMethodMessage), MessageType.Method)]
    public class NetMethodMessage : BaseReflectionMessage<(int, List<(string, string)>)>
    {
        private (int, List<(string, string)>) data = (0, new List<(string, string)>());

        public NetMethodMessage(MessagePriority messagePriority, (int, List<(string, string)>) data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Method;
            this.data = data;
        }

        public NetMethodMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Method;
            this.data = Deserialize(data);
        }

        public override (int, List<(string, string)>) Deserialize(byte[] message)
        {
            try
            {
                DeserializeHeader(message);

                // Initialize with empty list to ensure Item2 is never null
                data = (0, new List<(string, string)>());

                if (!MessageChecker.DeserializeCheckSum(message))
                {
                    //Debug.LogWarning("Checksum validation failed");
                    return data;
                }

                // Read method ID
                data.Item1 = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);

                // Read parameters list count
                int paramCount = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);

                // Read each parameter
                for (int i = 0; i < paramCount; i++)
                {
                    string typeName = MessageChecker.DeserializeString(message, ref messageHeaderSize);
                    string value = MessageChecker.DeserializeString(message, ref messageHeaderSize);

                    if (!string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(value))
                    {
                        data.Item2.Add((typeName, value));
                    }
                    else
                    {
                        //Debug.LogWarning($"Invalid parameter at index {i} - Type: {typeName}, Value: {value}");
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Deserialization error: {ex.Message}");
                return (0, new List<(string, string)>());
            }
        }

        public (int, List<(string, string)>) GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            // Write method ID
            outData.AddRange(BitConverter.GetBytes(data.Item1));

            // Write parameters count
            outData.AddRange(BitConverter.GetBytes(data.Item2?.Count ?? 0));

            // Write each parameter
            if (data.Item2 != null)
            {
                foreach (var param in data.Item2)
                {
                    outData.AddRange(MessageChecker.SerializeString(param.Item1?.ToCharArray() ?? Array.Empty<char>()));
                    outData.AddRange(MessageChecker.SerializeString(param.Item2?.ToCharArray() ?? Array.Empty<char>()));
                }
            }

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
