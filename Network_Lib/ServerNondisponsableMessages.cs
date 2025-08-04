using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Handles non-disposable messages for the server, ensuring reliable delivery.
    /// </summary>
    public class ServerNondisponsableMessage : NondisponsableMessageBase
    {
        private Dictionary<int, Dictionary<MessageType, Queue<byte[]>>> LastMessageBroadcastToClients = new Dictionary<int, Dictionary<MessageType, Queue<byte[]>>>();
        private Dictionary<int, Dictionary<MessageType, float>> resendPackageCounterToClients = new Dictionary<int, Dictionary<MessageType, float>>();

        /// <summary>
        /// Initializes a new instance of the ServerNondisponsableMessage class.
        /// </summary>
        /// <param name="networkEntity">The network entity to associate with.</param>
        public ServerNondisponsableMessage(NetworkEntity networkEntity) : base(networkEntity)
        {
            networkEntity.OnNewPlayer += AddNewClient;
            networkEntity.OnRemovePlayer += RemoveClient;
        }

        /// <summary>
        /// Handles confirmation messages from clients.
        /// </summary>
        /// <param name="netConfirm">The confirmation message.</param>
        /// <param name="id">The client ID.</param>
        protected override void HandleConfirmationMessage(NetConfirmMessage netConfirm, int id)
        {
            if (LastMessageBroadcastToClients.ContainsKey(id))
            {
                Dictionary<MessageType, Queue<byte[]>> clientMessages = LastMessageBroadcastToClients[id];

                if (clientMessages.ContainsKey(netConfirm.GetData()) && clientMessages[netConfirm.GetData()].Count > 0)
                {
                    byte[] message = clientMessages[netConfirm.GetData()].Peek();

                    if (MessagesHistory.ContainsKey(message))
                    {
                        clientMessages[netConfirm.GetData()].Dequeue();
                    }
                    else
                    {
                        MessagesHistory.Add(clientMessages[netConfirm.GetData()].Dequeue(), secondsToDeleteMessageHistory);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a sent message to the tracking queue.
        /// </summary>
        /// <param name="data">The message data.</param>
        /// <param name="clientId">The client ID.</param>
        public override void AddSentMessages(byte[] data, int clientId)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                if (!LastMessageBroadcastToClients.ContainsKey(clientId))
                {
                    LastMessageBroadcastToClients.Add(clientId, new Dictionary<MessageType, Queue<byte[]>>());
                }

                MessageType messageType = MessageChecker.CheckMessageType(data);

                if (!LastMessageBroadcastToClients[clientId].ContainsKey(messageType))
                {
                    LastMessageBroadcastToClients[clientId].Add(messageType, new Queue<byte[]>());
                }

                LastMessageBroadcastToClients[clientId][messageType].Enqueue(data);
            }
        }

        /// <summary>
        /// Adds a sent message to the tracking queue.
        /// </summary>
        /// <param name="data">The message data.</param>
        /// <param name="clientId">The client ID.</param>
        public override void ResendPackages()
        {
            if (resendPackageCounterToClients.Count > 0)
            {
                foreach (int id in resendPackageCounterToClients.Keys)
                {
                    foreach (MessageType messageType in resendPackageCounterToClients[id].Keys)
                    {
                        resendPackageCounterToClients[id][messageType] += pingPong.deltaTime;

                        if (resendPackageCounterToClients[id][messageType] >= ((ServerPingPong)pingPong).GetLatencyFormClient(id) * 5)
                        {
                            if (LastMessageBroadcastToClients[id][messageType].Count > 0)
                            {
                                Console.WriteLine("Resending package to client " + id);
                                networkEntity.SendMessage(LastMessageBroadcastToClients[id][messageType].Peek(), id);
                                resendPackageCounterToClients[id][messageType] = 0;
                            }
                        }
                    }
                }
            }

            CleanupMessageHistory();
        }

        /// <summary>
        /// Adds a sent message to the tracking queue.
        /// </summary>
        /// <param name="data">The message data.</param>
        /// <param name="clientId">The client ID.</param>
        private void AddNewClient(int clientID)
        {
            LastMessageBroadcastToClients.Add(clientID, new Dictionary<MessageType, Queue<byte[]>>());
            resendPackageCounterToClients.Add(clientID, new Dictionary<MessageType, float>());
        }

        /// <summary>
        /// Removes a client from the tracking system.
        /// </summary>
        /// <param name="clientID">The client ID to remove.</param>
        private void RemoveClient(int clientID)
        {
            LastMessageBroadcastToClients.Remove(clientID);
            resendPackageCounterToClients.Remove(clientID);
        }

        /// <summary>
        /// Cleans up old messages from the message history.
        /// </summary>
        private void CleanupMessageHistory()
        {
            if (MessagesHistory.Count > 0 && pingPong != null)
            {
                List<byte[]> keysToRemove = new List<byte[]>(MessagesHistory.Count);

                foreach (byte[] messageKey in MessagesHistory.Keys)
                {
                    keysToRemove.Add(messageKey);
                }

                foreach (byte[] messageKey in keysToRemove)
                {
                    MessagesHistory[messageKey] -= pingPong.deltaTime;

                    if (MessagesHistory[messageKey] <= 0)
                    {
                        MessagesHistory.Remove(messageKey);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a confirmation message to a client.
        /// </summary>
        /// <param name="messageType">The message type to confirm.</param>
        /// <param name="id">The client ID.</param>
        public override void SendConfirmationMessage(MessageType messageType, int id = -1)
        {
            NetConfirmMessage netConfirmMessage = new NetConfirmMessage(MessagePriority.Default, messageType);
            networkEntity.SendMessage(netConfirmMessage.Serialize(), id);
        }
    }
}