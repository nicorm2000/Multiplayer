using System.Collections.Generic;
using System;

namespace Net
{
    public class ClientNondisponsableMessage : NondisponsableMessageBase
    {
        private Dictionary<MessageType, Queue<byte[]>> LastMessageSendToServer = new Dictionary<MessageType, Queue<byte[]>>();
        private Dictionary<MessageType, float> resendPackageCounterToServer = new Dictionary<MessageType, float>();

        /// <summary>
        /// Initializes a new instance of the ClientNondisponsableMessage class with the specified network entity.
        /// </summary>
        /// <param name="networkEntity">The network entity associated with this message handler.</param>
        public ClientNondisponsableMessage(NetworkEntity networkEntity) : base(networkEntity) { }

        /// <summary>
        /// Handles confirmation messages from the server.
        /// </summary>
        /// <param name="netConfirm">The confirmation message.</param>
        /// <param name="id">The client ID associated with the message.</param>
        protected override void HandleConfirmationMessage(NetConfirmMessage netConfirm, int id)
        {
            if (LastMessageSendToServer.ContainsKey(netConfirm.GetData()) && LastMessageSendToServer[netConfirm.GetData()].Count > 0)
            {
                byte[] message = LastMessageSendToServer[netConfirm.GetData()].Peek();

                if (MessagesHistory.ContainsKey(message))
                {
                    LastMessageSendToServer[netConfirm.GetData()].Dequeue();
                }
                else
                {
                    MessagesHistory.Add(LastMessageSendToServer[netConfirm.GetData()].Dequeue(), secondsToDeleteMessageHistory);
                }
            }
        }

        /// <summary>
        /// Adds a sent message to the queue for potential resending.
        /// </summary>
        /// <param name="data">The message data.</param>
        /// <param name="clientId">The client ID associated with the message.</param>
        public override void AddSentMessages(byte[] data, int clientId = -1)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                MessageType messageType = MessageChecker.CheckMessageType(data);

                if (!LastMessageSendToServer.ContainsKey(messageType))
                {
                    LastMessageSendToServer.Add(messageType, new Queue<byte[]>());
                }

                LastMessageSendToServer[messageType].Enqueue(data);
            }
        }

        /// <summary>
        /// Resends packages that haven't been confirmed by the server.
        /// </summary>
        public override void ResendPackages()
        {
            if (resendPackageCounterToServer.Count > 0)
            {
                foreach (MessageType messageType in resendPackageCounterToServer.Keys)
                {
                    resendPackageCounterToServer[messageType] += pingPong.deltaTime;

                    if (resendPackageCounterToServer[messageType] >= ((ClientPingPong)pingPong).GetLatencyFormServer() * 5)
                    {
                        if (LastMessageSendToServer[messageType].Count > 0)
                        {
                            Console.WriteLine("Resending package to server");
                            networkEntity.SendMessage(LastMessageSendToServer[messageType].Peek());
                            resendPackageCounterToServer[messageType] = 0;
                        }
                    }
                }
            }

            CleanupMessageHistory();
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
        /// Sends a confirmation message to the server for a specific message type.
        /// </summary>
        /// <param name="messageType">The type of message to confirm.</param>
        /// <param name="id">The client ID associated with the message.</param>
        public override void SendConfirmationMessage(MessageType messageType, int id = -1)
        {
            NetConfirmMessage netConfirmMessage = new NetConfirmMessage(MessagePriority.Default, messageType);
            networkEntity.SendMessage(netConfirmMessage.Serialize());
        }
    }
}