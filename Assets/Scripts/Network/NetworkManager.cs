using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public struct Client
{
    public float timeStamp;
    public int id;
    public IPEndPoint ipEndPoint;
    public string clientName;

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp, string clientName)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
        this.clientName = clientName;
    }
}

public struct Player
{
    public int id;
    public string name;

    public Player(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    public IPAddress ipAddress
    {
        get; private set;
    }

    public int port
    {
        get; private set;
    }

    public bool isServer
    {
        get; private set;
    }

    public int TimeOut = 30;

    private UdpConnection connection;

    public readonly Dictionary<int, Client> clients = new(); // Server has this list
    private readonly Dictionary<int, Player> players = new(); // Clients have this list
    private readonly Dictionary<IPEndPoint, int> ipToId = new();

    public string userName = "Server";
    public int serverClientId = 0; // ID that the server assigns to new clients
    public int actualClientId = 0; // ID of this client, not server

    MessageChecker messageChecker;
    PingPong checkActivity;

    int maxPlayersPerServer = 4;

    private void Start()
    {
        messageChecker = new MessageChecker();
    }

    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);
        checkActivity = new PingPong();
    }

    public void StartClient(IPAddress ip, int port, string name)
    {
        isServer = false;

        this.port = port;
        this.ipAddress = ip;
        this.userName = name;

        connection = new UdpConnection(ip, port, this);
        checkActivity = new PingPong();

        ClientToServerNetHandShake handShakeMesage = new((UdpConnection.IPToLong(ip), port, name));
        SendToServer(handShakeMesage.Serialize());
    }

    public void AddClient(IPEndPoint ip, int newClientID, string clientName)
    {
        if (!ipToId.ContainsKey(ip) && !clients.ContainsKey(newClientID))
        {
            Debug.Log("Adding client: " + ip.Address);

            ipToId[ip] = newClientID;
            clients.Add(newClientID, new Client(ip, newClientID, Time.realtimeSinceStartup, clientName));

            checkActivity.AddClientForList(newClientID);

            if (isServer)
            {
                List<(int, string)> playersInServer = new();

                foreach (int id in clients.Keys)
                {
                    playersInServer.Add((clients[id].id, clients[id].clientName));
                }

                ServerToClientHandShake serverToClient = new(playersInServer);
                Broadcast(serverToClient.Serialize());
            }
        }
        else
        {
            Debug.Log("Es un cliente repetido");
        }
    }

    public void RemoveClient(int idToRemove)
    {
        if (clients.ContainsKey(idToRemove))
        {
            Debug.Log("Removing client: " + idToRemove);

            checkActivity.RemoveClientForList(idToRemove);

            ipToId.Remove(clients[idToRemove].ipEndPoint);
            players.Remove(idToRemove);
            clients.Remove(idToRemove);
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        switch (messageChecker.CheckMessageType(data))
        {
            case MessageType.Ping:

                if (isServer)
                {
                    if (ipToId.ContainsKey(ip))
                    {
                        checkActivity.ReciveClientToServerPingMessage(ipToId[ip]);
                    }
                    else
                    {
                        Debug.LogError("Fail Client ID");
                    }
                }
                else
                {
                    checkActivity.ReciveServerToClientPingMessage();
                }

                break;

            case MessageType.ServerToClientHandShake:

                ServerToClientHandShake netGetClientID = new(data);

                List<(int clientId, string userName)> playerList = netGetClientID.GetData();

                players.Clear();
                for (int i = 0; i < playerList.Count; i++)
                {
                    if (playerList[i].userName == userName)
                    {
                        actualClientId = playerList[i].clientId;
                    }

                    Debug.Log(playerList[i].clientId + " - " + playerList[i].userName);
                    Player playerToAdd = new Player(playerList[i].clientId, playerList[i].userName);
                    players.Add(playerList[i].clientId, playerToAdd);
                }

                break;

            case MessageType.ClientToServerHandShake:

                ReciveClientToServerHandShake(data, ip);

                break;
            case MessageType.Console:

                UpdateChatText(data, ip);
                break;

            case MessageType.Position:
                break;
            case MessageType.NewCustomerNotice:
                break;
            case MessageType.Disconnection:

                NetDisconnection netDisconnection = new(data);

                int playerID = netDisconnection.GetData();
                if (isServer)
                {
                    Broadcast(data);
                    RemoveClient(playerID);
                }
                else
                {
                    Debug.Log("Remove player " + playerID);
                    RemoveClient(playerID);
                }

                break;
            case MessageType.ThereIsNoPlace:
                break;
            case MessageType.RepeatMessage:
                break;
            case MessageType.Error:

                NetErrorMessage netErrorMessage = new NetErrorMessage(data);

                //NetworkScreen.Instance.SwitchToMenuScreen();
                //NetworkScreen.Instance.ShowErrorPanel(netErrorMessage.GetData());
                connection.Close();

                break;
            default:
                break;
        }
    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    public void Broadcast(byte[] data, IPEndPoint ip)
    {
        connection.Send(data, ip);
    }

    public void Broadcast(byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                connection.Send(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    void Update()
    {
        // Flush the data in main thread
        if (connection != null)
        {
            connection.FlushReceiveData();

            if (checkActivity != null)
            {
                checkActivity.UpdateCheckActivity();
            }
        }
    }

    void ReciveClientToServerHandShake(byte[] data, IPEndPoint ip)
    {
        ClientToServerNetHandShake handShake = new(data);

        if (CheckValidUserName(handShake.GetData().Item3, ip) && !ServerIsFull(ip))
        {
            AddClient(ip, serverClientId, handShake.GetData().Item3);
            serverClientId++;
        }

    }

    bool ServerIsFull(IPEndPoint ip)
    {
        bool serverIsFull = clients.Count >= maxPlayersPerServer;

        if (serverIsFull)
        {
            NetErrorMessage netServerIsFull = new("Server is full");
            Broadcast(netServerIsFull.Serialize(), ip);
        }

        return serverIsFull;
    }

    bool CheckValidUserName(string userName, IPEndPoint ip)
    {
        foreach (int clientID in clients.Keys)
        {
            if (userName == clients[clientID].clientName)
            {
                NetErrorMessage netInvalidUserName = new("Invalid User Name");
                Broadcast(netInvalidUserName.Serialize(), ip);

                return false;
            }
        }

        return true;
    }

    private void UpdateChatText(byte[] data, IPEndPoint ip)
    {
        string messageText = "";

        NetMessage netMessage = new(data);
        messageText += new string(netMessage.GetData());

        if (isServer)
        {
            Broadcast(data);
        }

        ChatScreen.Instance.messages.text += messageText + System.Environment.NewLine;
    }

    void OnApplicationQuit()
    {
        if (!isServer)
        {
            NetDisconnection netDisconnection = new(actualClientId);
            SendToServer(netDisconnection.Serialize());
        }
        else
        {
            NetErrorMessage netErrorMessage = new("Lost Connection To Server");
            CloseServer();
        }
    }

    public void CloseServer()
    {
        if (isServer)
        {
            foreach (int clientId in clients.Keys)
            {
                NetDisconnection netDisconnection = new(clientId);
                Broadcast(netDisconnection.Serialize());
                RemoveClient(clientId);
            }
        }
    }

    public void DisconectPlayer()
    {
        if (!isServer)
        {
            connection.Close();
        }
    }
}