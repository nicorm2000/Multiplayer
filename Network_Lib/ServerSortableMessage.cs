namespace Net
{
    /// <summary>
    /// Handles sortable messages for the server to ensure proper message ordering.
    /// </summary>
    public class ServerSortableMessage : SortableMessagesBase
    {
        /// <summary>
        /// Initializes a new instance of the ServerSortableMessage class.
        /// </summary>
        /// <param name="networkEntity">The network entity to associate with.</param>
        public ServerSortableMessage(NetworkEntity networkEntity) : base(networkEntity)
        {
            networkEntity.OnNewPlayer += AddNewClient;
            networkEntity.OnRemovePlayer += RemoveClient;
        }

        /// <summary>
        /// Processes received data and updates message ordering information.
        /// </summary>
        /// <param name="data">The received message data.</param>
        /// <param name="id">The client ID.</param>
        public override void OnRecievedData(byte[] data, int id)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.Sorteable) != 0)
            {
                MessageType messageType = MessageChecker.CheckMessageType(data);
                int messageTypeIndex = (int)messageType;

                int clientId = id;
                if (clientToRowMapping.ContainsKey(clientId))
                {
                    int row = clientToRowMapping[clientId];
                    OrderLastMessageReciveFromClients.Set(row, messageTypeIndex, !OrderLastMessageReciveFromClients.Get(row, messageTypeIndex));
                }
            }
        }
    }
}