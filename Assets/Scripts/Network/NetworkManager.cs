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

    public Action<byte[], IPEndPoint> OnRecievedMessage;

    public readonly Dictionary<int, Client> clients = new(); // The server has this list
    private readonly Dictionary<int, Player> players = new(); // The client has this list
    public readonly Dictionary<IPEndPoint, int> ipToId = new();

    public string userName = "Server";
    public int serverClientId = 0; // The ID that has the server to assign the clients
    public int actualClientId = -1; // The ID of this client

    public PingPong checkActivity;

    private GameManager gm;
    private SorteableMessage sorteableMessages;
    private NonDisposableMessage nondisponblesMessages;

    private int maxPlayersPerServer = 4;
    public bool matchOnGoing = false;

    private void Start()
    {
        gm = GameManager.Instance;
        sorteableMessages = new();
        nondisponblesMessages = new();
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

        ClientToServerNetHandShake handShakeMesage = new (MessagePriority.NonDisposable, (UdpConnection.IPToLong(ip), port, name));
        SendToServer(handShakeMesage.Serialize());
    }

    public void AddClient(IPEndPoint ip, int newClientID, string clientName)
    {
        if (!ipToId.ContainsKey(ip) && !clients.ContainsKey(newClientID))
        {
            Debug.Log("Adding Client: " + ip.Address);

            ipToId[ip] = newClientID;
            clients.Add(newClientID, new Client(ip, newClientID, Time.realtimeSinceStartup, clientName));
            checkActivity.AddClientForList(newClientID);
            gm.OnNewPlayer?.Invoke(newClientID);

            if (isServer)
            {
                List<(int, string)> playersInServer = new ();

                foreach (int id in clients.Keys)
                {
                    playersInServer.Add((clients[id].id, clients[id].clientName));
                }

                ServerToClientHandShake serverToClient = new (MessagePriority.NonDisposable, playersInServer);
                Broadcast(serverToClient.Serialize());
            }
        }
        else
        {
            Debug.Log("It's a repeated Client");
        }
    }

    public void RemoveClient(int idToRemove)
    {
        gm.OnRemovePlayer?.Invoke(idToRemove);

        if (clients.ContainsKey(idToRemove))
        {
            Debug.Log("Removing client: " + idToRemove);
            checkActivity.RemoveClientForList(idToRemove);
            ipToId.Remove(clients[idToRemove].ipEndPoint);
            players.Remove(idToRemove);
            clients.Remove(idToRemove);
        }
        if (!isServer && actualClientId == idToRemove)
        {
            NetworkScreen.Instance.SwitchToMenuScreen();
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        OnRecievedMessage?.Invoke(data, ip);

        switch (MessageChecker.CheckMessageType(data))
        {
            case MessageType.Ping:

                if (isServer)
                {
                    if (ipToId.ContainsKey(ip))
                    {
                        checkActivity.RecieveClientToServerPingMessage(ipToId[ip]);
                        checkActivity.CalculateLatencyFromClients(ipToId[ip]);
                    }
                    else
                    {
                        Debug.LogError("Fail Client ID");
                    }
                }
                else
                {
                    checkActivity.ReciveServerToClientPingMessage();
                    checkActivity.CalculateLatencyFromServer();
                }

                break;

            case MessageType.ServerToClientHandShake:

                ServerToClientHandShake netGetClientID = new (data);
                List<(int clientId, string userName)> playerList = netGetClientID.GetData();
                for (int i = 0; i < playerList.Count; i++) // First verify which client am i
                {
                    if (playerList[i].userName == userName)
                    {
                        actualClientId = playerList[i].clientId;
                    }
                }
                players.Clear();
                for (int i = 0; i < playerList.Count; i++)
                {
                    Debug.Log(playerList[i].clientId + " - " + playerList[i].userName);
                    Player playerToAdd = new Player(playerList[i].clientId, playerList[i].userName);
                    players.Add(playerList[i].clientId, playerToAdd);
                    gm.OnNewPlayer?.Invoke(playerToAdd.id);
                }

                break;

            case MessageType.ClientToServerHandShake:

                ReciveClientToServerHandShake(data, ip);

                break;
            case MessageType.Console:

                UpdateChatText(data, ip);
                break;

            case MessageType.Position:

                NetVector3 netVector3 = new(data);
                if (isServer)
                {
                    if (ipToId.ContainsKey(ip))
                    {
                        if (sorteableMessages.CheckMessageOrderRecievedFromClients(ipToId[ip], MessageChecker.CheckMessageType(data), netVector3.MessageOrder))
                        {
                            UpdatePlayerPosition(data);
                        }
                    }
                }
                else
                {
                    if (sorteableMessages.CheckMessageOrderRecievedFromServer(netVector3.GetData().id, MessageType.Position, netVector3.MessageOrder))
                    {
                        UpdatePlayerPosition(data);
                    }
                }

                break;

            case MessageType.BulletInstatiate:

                NetVector3 netBullet = new (data);
                gm.OnInstantiateBullet?.Invoke(netBullet.GetData().id, netBullet.GetData().position);
                if (isServer)
                {
                    BroadcastPlayerPosition(netBullet.GetData().id, data);
                }

                break;

            case MessageType.Disconnection:

                NetIDMessage netDisconnection = new (data);
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

            case MessageType.UpdateLobbyTimer:

                if (!isServer)
                {
                    NetUpdateTimer netUpdate = new(data);
                    gm.OnInitLobbyTimer?.Invoke(netUpdate.GetData());
                }

                break;
            case MessageType.UpdateGameplayTimer:

                if (!isServer)
                {
                    gm.OnInitGameplayTimer?.Invoke();
                }

                break;

            case MessageType.Error:

                NetErrorMessage netErrorMessage = new (data);
                NetworkScreen.Instance.SwitchToMenuScreen();
                NetworkScreen.Instance.ShowErrorPanel(netErrorMessage.GetData());
                connection.Close();

                break;

            case MessageType.Winner:

                NetIDMessage netIDMessage = new(data);
                string winText = $"Congratulations! \n {players[netIDMessage.GetData()].name} won the game!";
                NetworkScreen.Instance.SwitchToMenuScreen();
                NetworkScreen.Instance.ShowWinPanel(winText);
                gm.EndMatch();

                break;

            default:

                break;
        }
    }

    public void SendToServer(byte[] data)
    {
        nondisponblesMessages.AddSentMessagesFromClients(data);
        connection.Send(data);
    }

    public void Broadcast(byte[] data, IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            nondisponblesMessages.AddSentMessagesFromServer(data, ipToId[ip]);
        }
        connection.Send(data, ip);
    }

    public void Broadcast(byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                nondisponblesMessages.AddSentMessagesFromServer(data, iterator.Current.Value.id);
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

            if (nondisponblesMessages != null)
            {
                nondisponblesMessages.ResendPackages();
            }

        }
    }


    void ReciveClientToServerHandShake(byte[] data, IPEndPoint ip)
    {
        ClientToServerNetHandShake handShake = new (data);

        if (!MatchOnGoing(ip) && CheckValidUserName(handShake.GetData().Item3, ip) && !ServerIsFull(ip))
        {
            AddClient(ip, serverClientId, handShake.GetData().Item3);
            serverClientId++;
        }
    }

    bool MatchOnGoing(IPEndPoint ip)
    {
        if (matchOnGoing)
        {
            NetErrorMessage netServerIsFull = new ("Match has already started");
            Broadcast(netServerIsFull.Serialize(), ip);
        }

        return matchOnGoing;
    }

    bool ServerIsFull(IPEndPoint ip)
    {
        bool serverIsFull = clients.Count >= maxPlayersPerServer;

        if (serverIsFull)
        {
            NetErrorMessage netServerIsFull = new ("Server is full");
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
                NetErrorMessage netInvalidUserName = new ("Invalid User Name");
                Broadcast(netInvalidUserName.Serialize(), ip);

                return false;
            }
        }

        return true;
    }

    private void UpdateChatText(byte[] data, IPEndPoint ip)
    {
        string messageText = "";

        NetMessage netMessage = new NetMessage(data);
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
            NetIDMessage netDisconnection = new (MessagePriority.Default, actualClientId);
            SendToServer(netDisconnection.Serialize());
        }
        else
        {
            NetErrorMessage netErrorMessage = new ("Lost Connection To Server");
            Broadcast(netErrorMessage.Serialize());
            CloseServer();
        }
    }

    public void CloseServer()
    {
        if (isServer)
        {
            List<int> clientIdsToRemove = new (clients.Keys);
            foreach (int clientId in clientIdsToRemove)
            {
                NetIDMessage netDisconnection = new (MessagePriority.Default, clientId);
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
            NetworkScreen.Instance.SwitchToMenuScreen();
        }
    }

    private void UpdatePlayerPosition(byte[] data)
    {
        NetVector3 netPosition = new (data);
        int clientId = netPosition.GetData().id;

        gm.UpdatePlayerPosition(netPosition.GetData());

        if (isServer)
        {
            BroadcastPlayerPosition(clientId, data);
        }
    }

    private void BroadcastPlayerPosition(int senderClientId, byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                int receiverClientId = iterator.Current.Key;

                // Stops you from sending your own position
                if (receiverClientId != senderClientId)
                {
                    // Checks both IpEndPoints, and if the sender is the same as the receptor, the loop continues without doing the Broadcast
                    if (clients[receiverClientId].ipEndPoint.Equals(clients[senderClientId].ipEndPoint)) continue;
                    Broadcast(data, clients[receiverClientId].ipEndPoint);
                }
            }
        }
    }
}