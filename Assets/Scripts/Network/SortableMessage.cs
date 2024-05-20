using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SortableMessage
{
    private GameManager gm;
    private NetworkManager nm;

    Dictionary<int, Dictionary<MessageType, int>> OrderLastMessageReciveFromServer;
    Dictionary<int, Dictionary<MessageType, int>> OrderLastMessageReciveFromClients;

    /// <summary>
    /// Initializes the SorteableMessage instance and subscribes to relevant events.
    /// </summary>
    public SortableMessage()
    {
        nm = NetworkManager.Instance;
        gm = GameManager.Instance;

        nm.OnReceivedMessage += OnReceivedData;

        gm.OnNewPlayer += AddNewClient;
        gm.OnRemovePlayer += RemoveClient;

        OrderLastMessageReciveFromClients = new Dictionary<int, Dictionary<MessageType, int>>();
        OrderLastMessageReciveFromServer = new Dictionary<int, Dictionary<MessageType, int>>();
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
                    if (OrderLastMessageReciveFromClients.ContainsKey(nm.ipToId[ip]))
                    {
                        if (!OrderLastMessageReciveFromClients[nm.ipToId[ip]].ContainsKey(messageType))
                        {
                            OrderLastMessageReciveFromClients[nm.ipToId[ip]].Add(messageType, 0);
                        }
                        else
                        {
                            OrderLastMessageReciveFromClients[nm.ipToId[ip]][messageType]++;
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

                    if (OrderLastMessageReciveFromServer.ContainsKey(clientId))
                    {
                        if (!OrderLastMessageReciveFromServer[clientId].ContainsKey(messageType))
                        {
                            OrderLastMessageReciveFromServer[clientId].Add(messageType, 0);
                        }
                        else
                        {
                            OrderLastMessageReciveFromServer[clientId][messageType]++;
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
    public bool CheckMessageOrderRecievedFromClients(int clientID, MessageType messageType, int messageOrder)
    {
        if (!OrderLastMessageReciveFromClients[clientID].ContainsKey(messageType))
        {
            OrderLastMessageReciveFromClients[clientID].Add(messageType, 0);
        }

        return OrderLastMessageReciveFromClients[clientID][messageType] < messageOrder;
    }

    /// <summary>
    /// Checks if the received message order from the server is valid.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="messageOrder">The message order.</param>
    public bool CheckMessageOrderRecievedFromServer(int clientID, MessageType messageType, int messageOrder)
    {
        if (!OrderLastMessageReciveFromServer[clientID].ContainsKey(messageType))
        {
            OrderLastMessageReciveFromServer[clientID].Add(messageType, 0);
        }

        return OrderLastMessageReciveFromServer[clientID][messageType] < messageOrder;
    }

    /// <summary>
    /// Adds a new client to the message order tracking dictionary.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    private void AddNewClient(int clientID)
    {
        if (nm.isServer)
        {
            OrderLastMessageReciveFromClients.Add(clientID, new Dictionary<MessageType, int>());
        }
        else
        {
            if (!OrderLastMessageReciveFromServer.ContainsKey(clientID))
            {
                OrderLastMessageReciveFromServer.Add(clientID, new Dictionary<MessageType, int>());
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
            OrderLastMessageReciveFromClients.Remove(clientID);
        }
        else
        {
            OrderLastMessageReciveFromServer.Remove(clientID);
        }
    }
}