using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NonDisposableMessage //TODO: Reworkear para utilizar BitMatrix en vez de diccionarios anidados
{
    GameManager gm;
    NetworkManager nm;

    Dictionary<MessageType, Queue<byte[]>> LastMessageSendToServer;
    Dictionary<int, Dictionary<MessageType, Queue<byte[]>>> LastMessageBroadcastToClients;

    Dictionary<byte[], float> MessagesHistory = new();
    int secondsToDeleteMessageHistory = 15;

    PingPong pingPong;

    Dictionary<int, Dictionary<MessageType, float>> resendPackageCounterToClients;
    Dictionary<MessageType, float> resendPackageCounterToServer;

    /// <summary>
    /// Initializes the NonDisposableMessage instance and subscribes to relevant events.
    /// </summary>
    public NonDisposableMessage()
    {
        nm = NetworkManager.Instance;
        gm = GameManager.Instance;

        nm.onInitPingPong += () => pingPong = nm.networkEntity.checkActivity;

        nm.onInitEntity += () => nm.networkEntity.OnReceivedMessage += OnReceivedData;

        gm.OnNewPlayer += AddNewClient;
        gm.OnRemovePlayer += RemoveClient;

        LastMessageSendToServer = new Dictionary<MessageType, Queue<byte[]>>();
        LastMessageBroadcastToClients = new Dictionary<int, Dictionary<MessageType, Queue<byte[]>>>();

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
            NetConfirmMessage netConfirmMessage = new NetConfirmMessage(MessagePriority.Default, messageType);

            if (nm.isServer)
            {
                nm.GetNetworkServer().Broadcast(netConfirmMessage.Serialize(), ip);
            }
            else
            {
                nm.GetNetworkClient().SendToServer(netConfirmMessage.Serialize());
            }
        }

        if (messageType == MessageType.Confirm)
        {
            NetConfirmMessage netConfirm = new(data);

            if (nm.isServer)
            {
                NetworkServer server = nm.GetNetworkServer();

                if (server.ipToId.ContainsKey(ip))
                {
                    if (LastMessageBroadcastToClients.ContainsKey(server.ipToId[ip]))
                    {
                        var clientMessages = LastMessageBroadcastToClients[server.ipToId[ip]];

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

                            //  Debug.Log("Confirm message with CLIENT " + server.clients[server.ipToId[ip]].id + " - " + MessageChecker.CheckMessageType(message)); ;
                        }
                    }
                }
            }
            else
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

                    // Debug.Log("Confirm message with SERVER " + " - " + MessageChecker.CheckMessageType(message)); ;
                }
            }
        }
    }

    /// <summary>
    /// Adds messages sent from the server to clients.
    /// </summary>
    /// <param name="data">The received data.</param>
    /// <param name="clientID">The received clientID.</param>
    public void AddSentMessagesFromServer(byte[] data, int clientId)
    {
        if (nm.isServer)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                Debug.Log("Add Sent Message SERVER to " + clientId + " - " + MessageChecker.CheckMessageType(data));

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
                Debug.Log("Add Sent Message CLIENT to SERVER" + " - " + MessageChecker.CheckMessageType(data));

                MessageType messageType = MessageChecker.CheckMessageType(data);

                if (!LastMessageSendToServer.ContainsKey(messageType))
                {
                    LastMessageSendToServer.Add(messageType, new Queue<byte[]>());
                }

                LastMessageSendToServer[messageType].Enqueue(data);
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
            LastMessageBroadcastToClients.Add(clientID, new Dictionary<MessageType, Queue<byte[]>>());
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
            LastMessageBroadcastToClients.Remove(clientID);
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
            NetworkServer server = nm.GetNetworkServer();

            if (resendPackageCounterToClients.Count > 0)
            {
                foreach (int id in resendPackageCounterToClients.Keys)
                {
                    foreach (MessageType messageType in resendPackageCounterToClients[id].Keys)
                    {
                        resendPackageCounterToClients[id][messageType] += Time.deltaTime;

                        if (resendPackageCounterToClients[id][messageType] >= pingPong.GetServerLatency() * 5)
                        {
                            if (LastMessageBroadcastToClients[id][messageType].Count > 0)
                            {
                                Debug.Log("Se envio el packete de nuevo hacia el cliente " + id);
                                server.Broadcast(LastMessageBroadcastToClients[id][messageType].Peek(), server.clients[id].ipEndPoint);
                                resendPackageCounterToClients[id][messageType] = 0;
                            }
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

                    if (resendPackageCounterToServer[messageType] >= pingPong.GetServerLatency() * 5)
                    {
                        if (LastMessageSendToServer[messageType].Count > 0)
                        {
                            Debug.Log("Se envio el packete de nuevo hacia el server");
                            nm.GetNetworkClient().SendToServer(LastMessageSendToServer[messageType].Peek());
                            resendPackageCounterToServer[messageType] = 0;
                        }
                    }
                }
            }
        }

        if (MessagesHistory.Count > 0)
        {
            List<byte[]> keysToRemove = new List<byte[]>(MessagesHistory.Count);

            foreach (byte[] messageKey in MessagesHistory.Keys)
            {
                keysToRemove.Add(messageKey);
            }

            foreach (byte[] messageKey in keysToRemove)
            {
                MessagesHistory[messageKey] -= Time.deltaTime;

                if (MessagesHistory[messageKey] <= 0)
                {
                    MessagesHistory.Remove(messageKey);
                }
            }
        }
    }
}