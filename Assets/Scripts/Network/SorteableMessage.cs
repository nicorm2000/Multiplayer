using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SorteableMessage
{
    private GameManager gm;
    private NetworkManager nm;

    Dictionary<int, Dictionary<MessageType, int>> OrderLastMessageReciveFromServer;
    Dictionary<int, Dictionary<MessageType, int>> OrderLastMessageReciveFromClients;

    public SorteableMessage()
    {
        nm = NetworkManager.Instance;
        gm = GameManager.Instance;

        nm.OnReceivedMessage += OnRecievedData;

        gm.OnNewPlayer += AddNewClient;
        gm.OnRemovePlayer += RemoveClient;

        OrderLastMessageReciveFromClients = new Dictionary<int, Dictionary<MessageType, int>>();
        OrderLastMessageReciveFromServer = new Dictionary<int, Dictionary<MessageType, int>>();
    }

    private void OnRecievedData(byte[] data, IPEndPoint ip)
    {
        MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

        if ((messagePriority & MessagePriority.Sorteable) != 0)
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

    public bool CheckMessageOrderRecievedFromClients(int clientID, MessageType messageType, int messageOrder)
    {
        if (!OrderLastMessageReciveFromClients[clientID].ContainsKey(messageType))
        {
            OrderLastMessageReciveFromClients[clientID].Add(messageType, 0);
        }

        return OrderLastMessageReciveFromClients[clientID][messageType] < messageOrder;
    }

    public bool CheckMessageOrderRecievedFromServer(int clientID, MessageType messageType, int messageOrder)
    {
        if (!OrderLastMessageReciveFromServer[clientID].ContainsKey(messageType))
        {
            OrderLastMessageReciveFromServer[clientID].Add(messageType, 0);
        }

        return OrderLastMessageReciveFromServer[clientID][messageType] < messageOrder;
    }

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