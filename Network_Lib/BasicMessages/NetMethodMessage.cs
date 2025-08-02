using System.Collections.Generic;
using System;
using Net;

namespace Network_Lib.BasicMessages
{
    /// <summary>
    /// Represents a network message containing method invocation data with parameters.
    /// </summary>
    [NetMessageClass(typeof(NetMethodMessage), MessageType.Method)]
    public class NetMethodMessage : BaseReflectionMessage<(int, List<(string, string)>)>
    {
        private (int, List<(string, string)>) data = (0, new List<(string, string)>());

        /// <summary>
        /// Initializes a new instance of the NetMethodMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The method data containing a method ID and parameter list.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetMethodMessage(MessagePriority messagePriority, (int, List<(string, string)>) data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Method;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetMethodMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetMethodMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Method;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into method information.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>A tuple containing the method ID and parameter list.</returns>
        public override (int, List<(string, string)>) Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            int readPosition = messageHeaderSize;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);

            messageHeaderSize = readPosition;

            if (message.Length < messageHeaderSize + sizeof(int))
                return (0, new List<(string, string)>());

            int methodId = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            if (message.Length < messageHeaderSize + sizeof(int))
                return (0, new List<(string, string)>());

            int paramCount = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            List<(string, string)> parameters = new List<(string, string)>();
            for (int i = 0; i < paramCount; i++)
            {
                if (message.Length < messageHeaderSize + sizeof(int))
                    break;

                string type = MessageChecker.DeserializeString(message, ref messageHeaderSize);
                string value = MessageChecker.DeserializeString(message, ref messageHeaderSize);

                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(value))
                {
                    parameters.Add((type, value));
                }
            }

            if (!checksumValid && !IsPlausibleMethodData(methodId, parameters))
            {
                return (0, new List<(string, string)>());
            }

            return (methodId, parameters);
        }

        /// <summary>
        /// Validates that the method data is plausible.
        /// </summary>
        /// <param name="methodId">The method ID to validate.</param>
        /// <param name="parameters">The parameter list to validate.</param>
        /// <returns>True if the data is plausible; otherwise, false.</returns>
        private bool IsPlausibleMethodData(int methodId, List<(string type, string value)> parameters)
        {
            if (methodId < 0) return false;

            const int MAX_PARAMS = 20;
            if (parameters.Count > MAX_PARAMS) return false;

            foreach ((string type, string value) param in parameters)
            {
                if (string.IsNullOrEmpty(param.type) || string.IsNullOrEmpty(param.value))
                    return false;

                if (param.type.Length > 256 || param.value.Length > 1024)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the method data contained in the message.
        /// </summary>
        /// <returns>A tuple containing the method ID and parameter list.</returns>
        public (int, List<(string, string)>) GetData()
        {
            return data;
        }

        /// <summary>
        /// Serializes the message into a byte array.
        /// </summary>
        /// <returns>The serialized message data.</returns>
        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data.Item1));

            outData.AddRange(BitConverter.GetBytes(data.Item2.Count));

            foreach ((string type, string value) in data.Item2)
            {
                outData.AddRange(MessageChecker.SerializeString(type.ToCharArray()));
                outData.AddRange(MessageChecker.SerializeString(value.ToCharArray()));
            }

            byte[] checksum = MessageChecker.SerializeCheckSum(outData);

            outData.AddRange(checksum);

            return outData.ToArray();
        }
    }
}