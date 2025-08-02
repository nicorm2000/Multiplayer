using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Represents a network message containing event data with parameters.
    /// </summary>
    [NetMessageClass(typeof(NetEventMessage), MessageType.Event)]
    public class NetEventMessage : BaseReflectionMessage<(int, List<(string, string)>)>
    {
        private (int, List<(string, string)>) data = (0, new List<(string, string)>());

        /// <summary>
        /// Initializes a new instance of the NetEventMessage class with the specified priority, data, and route.
        /// </summary>
        /// <param name="messagePriority">The priority of the message.</param>
        /// <param name="data">The event data containing an event ID and parameter list.</param>
        /// <param name="messageRoute">The route information for the message.</param>
        public NetEventMessage(MessagePriority messagePriority, (int, List<(string, string)>) data, List<RouteInfo> messageRoute)
            : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Event;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the NetEventMessage class from serialized data.
        /// </summary>
        /// <param name="data">The serialized message data.</param>
        public NetEventMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Event;
            this.data = Deserialize(data);
        }

        /// <summary>
        /// Deserializes the message data into event information.
        /// </summary>
        /// <param name="message">The serialized message data.</param>
        /// <returns>A tuple containing the event ID and parameter list.</returns>
        public override (int, List<(string, string)>) Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            int readPosition = messageHeaderSize;

            bool checksumValid = MessageChecker.DeserializeCheckSum(message);

            messageHeaderSize = readPosition;

            if (message.Length < messageHeaderSize + sizeof(int))
                return (0, new List<(string, string)>());

            int eventId = BitConverter.ToInt32(message, messageHeaderSize);
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

            if (!checksumValid && !IsPlausibleEventData(eventId, parameters))
            {
                return (0, new List<(string, string)>());
            }

            return (eventId, parameters);
        }

        /// <summary>
        /// Validates that the event data is plausible.
        /// </summary>
        /// <param name="eventId">The event ID to validate.</param>
        /// <param name="parameters">The parameter list to validate.</param>
        /// <returns>True if the data is plausible; otherwise, false.</returns>
        private bool IsPlausibleEventData(int eventId, List<(string type, string value)> parameters)
        {
            if (eventId < 0) return false;

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
        /// Gets the event data contained in the message.
        /// </summary>
        /// <returns>A tuple containing the event ID and parameter list.</returns>
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