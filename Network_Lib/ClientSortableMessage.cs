namespace Net
{
    public class ClientSortableMessage : SortableMessagesBase
    {
        /// <summary>
        /// Initializes a new instance of the ClientSortableMessage class with the specified network entity.
        /// </summary>
        /// <param name="networkEntity">The network entity associated with this message handler.</param>
        public ClientSortableMessage(NetworkEntity networkEntity) : base(networkEntity)
        {
            networkEntity.OnNewPlayer += AddNewClient;
            networkEntity.OnRemovePlayer += RemoveClient;
        }

        /// <summary>
        /// Processes received data and updates message ordering information.
        /// </summary>
        /// <param name="data">The received message data.</param>
        /// <param name="id">The client ID associated with the message.</param>
        public override void OnRecievedData(byte[] data, int id)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.Sorteable) != 0)
            {
                MessageType messageType = MessageChecker.CheckMessageType(data);
                int messageTypeIndex = (int)messageType;

                if (messageType == MessageType.Position)
                {
                    int clientId = new NetVector3(data).GetData().id;
                    if (clientToRowMapping.ContainsKey(clientId))
                    {
                        int row = clientToRowMapping[clientId];
                        OrderLastMessageReciveFromServer.Set(row, messageTypeIndex, !OrderLastMessageReciveFromServer.Get(row, messageTypeIndex));
                    }
                }
            }
        }
    }
}