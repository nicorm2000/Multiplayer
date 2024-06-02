using System.Collections.Generic;
using System.Net;

public class SortableMessage
{
    private GameManager gm;
    private NetworkManager nm;

    Dictionary<int, Dictionary<MessageType, int>> OrderLastMessageReciveFromServer;
    Dictionary<int, Dictionary<MessageType, int>> OrderLastMessageReciveFromClients;

    /// <summary>
    /// Initializes the SortableMessage instance and subscribes to relevant events.
    /// </summary>
    public SortableMessage()
    {
        nm = NetworkManager.Instance;
        gm = GameManager.Instance;

        nm.onInitEntity += () => nm.networkEntity.OnReceivedMessage += OnReceivedData;

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
                NetworkServer server = nm.GetNetworkServer();

                if (server.ipToId.ContainsKey(ip))
                {
                    if (OrderLastMessageReciveFromClients.ContainsKey(server.ipToId[ip]))
                    {
                        if (!OrderLastMessageReciveFromClients[server.ipToId[ip]].ContainsKey(messageType))
                        {
                            OrderLastMessageReciveFromClients[server.ipToId[ip]].Add(messageType, 0);
                        }
                        else
                        {
                            OrderLastMessageReciveFromClients[server.ipToId[ip]][messageType]++;
                        }
                    }
                }
            }
            else
            {
                if (messageType == MessageType.Position)
                {
                    int clientId = new NetVector3(data).GetData().id;

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
    public bool CheckMessageOrderReceivedFromClients(int clientID, MessageType messageType, int messageOrder)
    {
        if (!OrderLastMessageReciveFromClients[clientID].ContainsKey(messageType))
        {
            OrderLastMessageReciveFromClients[clientID].Add(messageType, 0);
        }

        // Debug.Log(OrderLastMessageReciveFromClients[clientID][messageType] + " - " + messageOrder + " - " + (OrderLastMessageReciveFromClients[clientID][messageType] < messageOrder));
        return OrderLastMessageReciveFromClients[clientID][messageType] < messageOrder;
    }

    /// <summary>
    /// Checks if the received message order from the server is valid.
    /// </summary>
    /// <param name="clientID">The received clientID.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="messageOrder">The message order.</param>
    public bool CheckMessageOrderReceivedFromServer(int clientID, MessageType messageType, int messageOrder)
    {
        if (!OrderLastMessageReciveFromServer[clientID].ContainsKey(messageType))
        {
            OrderLastMessageReciveFromServer[clientID].Add(messageType, 0);
        }

        //  Debug.Log(OrderLastMessageReciveFromServer[clientID][messageType] + " - " + messageOrder + " - " + (OrderLastMessageReciveFromServer[clientID][messageType] < messageOrder));
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