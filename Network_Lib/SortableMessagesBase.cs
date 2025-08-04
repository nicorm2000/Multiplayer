using System.Collections.Generic;

namespace Net
{
    /// <summary>
    /// Abstract base class for handling sortable messages to ensure proper message ordering.
    /// </summary>
    public abstract class SortableMessagesBase
    {
        protected NetworkEntity networkEntity;

        protected DynamicBitMatrix OrderLastMessageReciveFromServer;
        protected DynamicBitMatrix OrderLastMessageReciveFromClients;

        protected Dictionary<int, int> clientToRowMapping;
        protected int messageTypeCount;

        /// <summary>
        /// Initializes a new instance of the SortableMessagesBase class.
        /// </summary>
        /// <param name="networkEntity">The network entity to associate with.</param>
        public SortableMessagesBase(NetworkEntity networkEntity)
        {
            this.networkEntity = networkEntity;

            messageTypeCount = System.Enum.GetValues(typeof(MessageType)).Length;

            OrderLastMessageReciveFromClients = new DynamicBitMatrix(messageTypeCount);
            OrderLastMessageReciveFromServer = new DynamicBitMatrix(messageTypeCount);

            clientToRowMapping = new Dictionary<int, int>();
        }

        /// <summary>
        /// Processes received data and updates message ordering information.
        /// </summary>
        /// <param name="data">The received message data.</param>
        /// <param name="id">The client ID.</param>
        public abstract void OnRecievedData(byte[] data, int id);

        /// <summary>
        /// Checks the message order received from clients.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageOrder">The message order.</param>
        /// <returns>True if the message order is correct; otherwise, false.</returns>
        public bool CheckMessageOrderRecievedFromClients(int clientID, MessageType messageType, int messageOrder)
        {
            if (clientToRowMapping.ContainsKey(clientID))
            {
                int row = clientToRowMapping[clientID];
                int messageTypeIndex = (int)messageType;

                return OrderLastMessageReciveFromClients.Get(row, messageTypeIndex) == (messageOrder % 2 == 1);
            }
            return false;
        }

        /// <summary>
        /// Checks the message order received from the server.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageOrder">The message order.</param>
        /// <returns>True if the message order is correct; otherwise, false.</returns>
        public bool CheckMessageOrderRecievedFromServer(int clientID, MessageType messageType, int messageOrder)
        {
            if (clientToRowMapping.ContainsKey(clientID))
            {
                int row = clientToRowMapping[clientID];
                int messageTypeIndex = (int)messageType;

                return OrderLastMessageReciveFromServer.Get(row, messageTypeIndex) == (messageOrder % 2 == 1);
            }
            return false;
        }

        /// <summary>
        /// Adds a new client to the tracking system.
        /// </summary>
        /// <param name="clientID">The client ID to add.</param>
        protected void AddNewClient(int clientID)
        {
            if (!clientToRowMapping.ContainsKey(clientID))
            {
                int newRow = OrderLastMessageReciveFromClients.Rows;
                clientToRowMapping[clientID] = newRow;
                OrderLastMessageReciveFromClients.Set(newRow, 0, false); // Ensure a new row is added
                OrderLastMessageReciveFromServer.Set(newRow, 0, false); // Ensure a new row is added
            }
        }

        /// <summary>
        /// Adds a new client to the tracking system.
        /// </summary>
        /// <param name="clientID">The client ID to add.</param>
        protected void RemoveClient(int clientID)
        {
            if (clientToRowMapping.ContainsKey(clientID))
            {
                int row = clientToRowMapping[clientID];
                clientToRowMapping.Remove(clientID);
                OrderLastMessageReciveFromClients.ClearRow(row);
                OrderLastMessageReciveFromServer.ClearRow(row);
            }
        }
    }
}