using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SortableMessage
{
    private GameManager gm;
    private NetworkManager nm;

    Dictionary<int, Dictionary<MessageType, int>> orderLastMessageReceivedFromServer;
    Dictionary<int, Dictionary<MessageType, int>> orderLastMessageReceivedFromClients;

    /// <summary>
    /// Initializes the SortableMessage instance and subscribes to relevant events.
    /// </summary>
    public SortableMessage()
    {
        nm = NetworkManager.Instance;
        gm = GameManager.Instance;

        nm.OnReceivedMessage += OnReceivedData;

        gm.OnNewPlayer += AddNewClient;
        gm.OnRemovePlayer += RemoveClient;

        orderLastMessageReceivedFromClients = new Dictionary<int, Dictionary<MessageType, int>>();
        orderLastMessageReceivedFromServer = new Dictionary<int, Dictionary<MessageType, int>>();
    }

    /// <summary>
    /// Handles incoming data and updates message order information accordingly.
    /// </summary>
    /// <param name="data">The received data.</param>
    /// <param name="ip">The IP address of the sender.</param>
    private void OnReceivedData(byte[] data, IPEndPoint ip)
    {
        MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

        if ((messagePriority & MessagePriority.Sortable) != 0)
        {
            MessageType messageType = MessageChecker.CheckMessageType(data);

            if (nm.isServer)
            {
                if (nm.ipToId.ContainsKey(ip))
                {
                    if (orderLastMessageReceivedFromClients.ContainsKey(nm.ipToId[ip]))
                    {
                        if (!orderLastMessageReceivedFromClients[nm.ipToId[ip]].ContainsKey(messageType))
                        {
                            orderLastMessageReceivedFromClients[nm.ipToId[ip]].Add(messageType, 0);
                        }
                        else
                        {
                            orderLastMessageReceivedFromClients[nm.ipToId[ip]][messageType]++;
                        }
                    }
                }
            }
            else
            {
                if (messageType == MessageType.Position)
                {
                    int clientId = new NetVector3(data).GetData().id;
                    Debug.Log(clientId + " - " + new NetVector3(data).MessageOrder);

                    if (orderLastMessageReceivedFromServer.ContainsKey(clientId))
                    {
                        if (!orderLastMessageReceivedFromServer[clientId].ContainsKey(messageType))
                        {
                            orderLastMessageReceivedFromServer[clientId].Add(messageType, 0);
                        }
                        else
                        {
                            orderLastMessageReceivedFromServer[clientId][messageType]++;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if the received message order from clients is valid.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="messageOrder">The message order.</param>
    public bool CheckMessageOrderReceivedFromClients(int clientID, MessageType messageType, int messageOrder)
    {
        if (!orderLastMessageReceivedFromClients[clientID].ContainsKey(messageType))
        {
            orderLastMessageReceivedFromClients[clientID].Add(messageType, 0);
        }

        return orderLastMessageReceivedFromClients[clientID][messageType] < messageOrder;
    }

    /// <summary>
    /// Checks if the received message order from the server is valid.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="messageOrder">The message order.</param>
    public bool CheckMessageOrderReceivedFromServer(int clientID, MessageType messageType, int messageOrder)
    {
        if (!orderLastMessageReceivedFromServer[clientID].ContainsKey(messageType))
        {
            orderLastMessageReceivedFromServer[clientID].Add(messageType, 0);
        }

        return orderLastMessageReceivedFromServer[clientID][messageType] < messageOrder;
    }

    /// <summary>
    /// Adds a new client to the message order tracking dictionary.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    private void AddNewClient(int clientID)
    {
        if (nm.isServer)
        {
            orderLastMessageReceivedFromClients.Add(clientID, new Dictionary<MessageType, int>());
        }
        else
        {
            if (!orderLastMessageReceivedFromServer.ContainsKey(clientID))
            {
                orderLastMessageReceivedFromServer.Add(clientID, new Dictionary<MessageType, int>());
            }
        }
    }

    /// <summary>
    /// Removes a client from the message order tracking dictionary.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    private void RemoveClient(int clientID)
    {
        if (nm.isServer)
        {
            orderLastMessageReceivedFromClients.Remove(clientID);
        }
        else
        {
            orderLastMessageReceivedFromServer.Remove(clientID);
        }
    }
}