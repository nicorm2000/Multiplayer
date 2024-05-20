using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NonDisposableMessage
{
    private GameManager gm;
    private NetworkManager nm;
    private PingPong pingPong;

    Dictionary<MessageType, Queue<byte[]>> lastMessageSentToServer;
    Dictionary<int, Dictionary<MessageType, Queue<byte[]>>> lastMessageBroadcastedToClients;

    private int secondsToDeleteMessageHistory = 15;
    Dictionary<byte[], float> messagesHistory = new();

    Dictionary<int, Dictionary<MessageType, float>> resendPackageCounterToClients;
    Dictionary<MessageType, float> resendPackageCounterToServer;

    /// <summary>
    /// Initializes the NonDisposableMessage instance and subscribes to relevant events.
    /// </summary>
    public NonDisposableMessage()
    {
        nm = NetworkManager.Instance;
        gm = GameManager.Instance;

        pingPong = nm.checkActivity;

        nm.OnReceivedMessage += OnReceivedData;

        gm.OnNewPlayer += AddNewClient;
        gm.OnRemovePlayer += RemoveClient;

        lastMessageSentToServer = new Dictionary<MessageType, Queue<byte[]>>();
        lastMessageBroadcastedToClients = new Dictionary<int, Dictionary<MessageType, Queue<byte[]>>>();

        resendPackageCounterToClients = new Dictionary<int, Dictionary<MessageType, float>>();
        resendPackageCounterToServer = new Dictionary<MessageType, float>();
    }

    /// <summary>
    /// Handles incoming data and updates message order information accordingly.
    /// </summary>
    /// <param name="data">The received data.</param>
    /// <param name="ip">The IP address of the sender.</param>
    private void OnReceivedData(byte[] data, IPEndPoint ip)
    {
        MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);
        MessageType messageType = MessageChecker.CheckMessageType(data);

        if ((messagePriority & MessagePriority.NonDisposable) != 0)
        {
            NetConfirmMessage netConfirmMessage = new (MessagePriority.Default, messageType);

            if (nm.isServer)
            {
                nm.Broadcast(netConfirmMessage.Serialize(), ip);
            }
            else
            {
                nm.SendToServer(netConfirmMessage.Serialize());
            }
        }

        if (messageType == MessageType.Confirm)
        {
            NetConfirmMessage netConfirm = new(data);

            if (nm.isServer)
            {
                if (nm.ipToId.ContainsKey(ip))
                {
                    if (lastMessageBroadcastedToClients.ContainsKey(nm.ipToId[ip]))
                    {
                        if (messagesHistory.ContainsKey(lastMessageBroadcastedToClients[nm.ipToId[ip]][netConfirm.GetData()].Peek()))
                        {
                            lastMessageBroadcastedToClients[nm.ipToId[ip]][netConfirm.GetData()].Dequeue();
                        }
                        else
                        {
                            messagesHistory.Add(lastMessageBroadcastedToClients[nm.ipToId[ip]][netConfirm.GetData()].Dequeue(), secondsToDeleteMessageHistory);
                        }
                    }
                }
            }
            else
            {
                if (messagesHistory.ContainsKey(lastMessageSentToServer[netConfirm.GetData()].Peek()))
                {
                    lastMessageSentToServer[netConfirm.GetData()].Dequeue();
                }
                else
                {
                    messagesHistory.Add(lastMessageSentToServer[netConfirm.GetData()].Dequeue(), secondsToDeleteMessageHistory);
                }
            }
        }
    }

    /// <summary>
    /// Adds messages sent from the server to clients.
    /// </summary>
    /// <param name="data">The received data.</param>
    /// <param name="clientID">The received clientID.</param>
    public void AddSentMessagesFromServer(byte[] data, int clientID)
    {
        if (nm.isServer)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                if (!lastMessageBroadcastedToClients.ContainsKey(clientID))
                {
                    lastMessageBroadcastedToClients.Add(clientID, new Dictionary<MessageType, Queue<byte[]>>());
                }

                MessageType messageType = MessageChecker.CheckMessageType(data);

                if (!lastMessageBroadcastedToClients[clientID].ContainsKey(messageType))
                {
                    lastMessageBroadcastedToClients[clientID].Add(messageType, new Queue<byte[]>());
                }

                lastMessageBroadcastedToClients[clientID][messageType].Enqueue(data);
            }
        }
    }

    /// <summary>
    /// Adds messages sent from the client to the server.
    /// </summary>
    /// <param name="data">The received data.</param>
    public void AddSentMessagesFromClients(byte[] data)
    {
        if (!nm.isServer)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                MessageType messageType = MessageChecker.CheckMessageType(data);

                if (!lastMessageSentToServer.ContainsKey(messageType))
                {
                    lastMessageSentToServer.Add(messageType, new Queue<byte[]>());
                }

                lastMessageSentToServer[messageType].Enqueue(data);
            }
        }
    }

    /// <summary>
    /// Adds client to the server.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    private void AddNewClient(int clientID)
    {
        if (nm.isServer)
        {
            lastMessageBroadcastedToClients.Add(clientID, new Dictionary<MessageType, Queue<byte[]>>());
            resendPackageCounterToClients.Add(clientID, new Dictionary<MessageType, float>());
        }
    }

    /// <summary>
    /// Removes client from the Server.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    private void RemoveClient(int clientID)
    {
        if (nm.isServer)
        {
            lastMessageBroadcastedToClients.Remove(clientID);
            resendPackageCounterToClients.Remove(clientID);
        }
    }

    /// <summary>
    /// Resends packages to ensure delivery, managing resending from the server to clients and from clients to the server.
    /// </summary>
    public void ResendPackages()
    {
        if (nm.isServer)
        {
            if (resendPackageCounterToClients.Count > 0)
            {
                foreach (int id in resendPackageCounterToClients.Keys)
                {
                    foreach (MessageType messageType in resendPackageCounterToClients[id].Keys)
                    {
                        resendPackageCounterToClients[id][messageType] += Time.deltaTime;

                        // Reset the resend counter for this package
                        if (resendPackageCounterToClients[id][messageType] >= pingPong.GetServerLatency() * 5)
                        {
                            Debug.Log("Package sent back to Client " + id);
                            nm.Broadcast(lastMessageBroadcastedToClients[id][messageType].Peek(), nm.clients[id].ipEndPoint);
                            // Reset the resend counter for this package
                            resendPackageCounterToClients[id][messageType] = 0;
                        }
                    }
                }
            }
        }
        else
        {
            if (resendPackageCounterToServer.Count > 0)
            {
                foreach (MessageType messageType in resendPackageCounterToServer.Keys)
                {
                    resendPackageCounterToServer[messageType] += Time.deltaTime;

                    // Check if it's time to resend the package
                    if (resendPackageCounterToServer[messageType] >= pingPong.GetServerLatency() * 5)
                    {
                        Debug.Log("Package sent back to Server");
                        nm.SendToServer(lastMessageSentToServer[messageType].Peek());
                        // Reset the resend counter for this package
                        resendPackageCounterToServer[messageType] = 0;
                    }
                }
            }
        }

        // Check and remove old messages from history
        if (messagesHistory.Count > 0)
        {
            List<byte[]> keysToRemove = new (messagesHistory.Count);

            // Find messages that have expired
            foreach (byte[] messageKey in messagesHistory.Keys)
            {
                keysToRemove.Add(messageKey);
            }

            // Remove expired messages
            foreach (byte[] messageKey in keysToRemove)
            {
                messagesHistory[messageKey] -= Time.deltaTime;

                if (messagesHistory[messageKey] <= 0)
                {
                    messagesHistory.Remove(messageKey);
                }
            }
        }
    }
}