using Net;
using System;
using System.Collections.Generic;
using System.Net;
using static Server;

public class Server : NetworkEntity
{
    public struct Client
    {
        public float timeStamp; // Time stamp of when the client connected
        public int id; // Unique identifier of the client
        public IPEndPoint ipEndPoint; // IP endpoint of the client
        public string clientName; // Name of the client

        /// <summary>
        /// Initializes a new instance of the Client struct with the specified IP endpoint, ID, time stamp, and client name.
        /// </summary>
        /// <param name="ipEndPoint">The IP endpoint of the client.</param>
        /// <param name="id">The unique identifier of the client.</param>
        /// <param name="timeStamp">The time stamp of when the client connected.</param>
        /// <param name="clientName">The name of the client.</param>
        public Client(IPEndPoint ipEndPoint, int id, float timeStamp, string clientName)
        {
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
            this.clientName = clientName;
        }
    }

    public readonly Dictionary<int, Client> clients = new();

    public readonly Dictionary<IPEndPoint, int> ipToId = new();

    private List<string> activePlayerNames = new();

    DateTime appStartTime;

    private int maxPlayersPerServer = 4;
    public bool matchOnGoing = false;

    ServerPingPong pingPong;
    ServerSortableMessage sortableMessage;
    ServerNondisponsableMessage nondisponsableMessage;

    IPEndPoint matchmMakerIp;

    int instancesIdCount = 0;

    /// <summary>
    /// Starts the server on the specified port.
    /// </summary>
    /// <param name="port">The port to listen on.</param>
    public Server(int port, DateTime appStartTime) : base()
    {
        this.port = port;

        connection = new UdpConnection(port, this);

        pingPong = new ServerPingPong(this);
        checkActivity = pingPong;

        onInitPingPong?.Invoke();

        sortableMessage = new ServerSortableMessage(this);
        nondisponsableMessage = new ServerNondisponsableMessage(this);

        this.appStartTime = appStartTime;
    }

    /// <summary>
    /// Adds a new client to the server.
    /// </summary>
    /// <param name="ip">The IP endpoint of the client.</param>
    /// <param name="newClientID">The ID of the new client.</param>
    /// <param name="clientName">The name of the new client.</param>
    public override void AddClient(IPEndPoint ip, int newClientID, string clientName)
    {
        if (!ipToId.ContainsKey(ip) && !clients.ContainsKey(newClientID))
        {
            Console.WriteLine("Adding Client: " + ip.Address);

            ipToId[ip] = newClientID;
            clients.Add(newClientID, new Client(ip, newClientID, Convert.ToSingle((DateTime.UtcNow - appStartTime).TotalSeconds), clientName));
            if (!activePlayerNames.Contains(clientName))
                activePlayerNames.Add(clientName);
            BroadcastPlayerListToMatchMaker();
            pingPong.AddClientForList(newClientID);
            OnNewPlayer?.Invoke(newClientID);

            List<(int, string)> playersInServer = new();

            foreach (int id in clients.Keys)
            {
                playersInServer.Add((clients[id].id, clients[id].clientName));
            }

            ServerToClientHandShake serverToClient = new(MessagePriority.NonDisposable, playersInServer);
            Broadcast(serverToClient.Serialize());
        }
        else
        {
            Console.WriteLine("It's a repeated Client");
        }
    }

    /// <summary>
    /// Removes a client from the server.
    /// </summary>
    /// <param name="idToRemove">The ID of the client to remove.</param>
    public override void RemoveClient(int idToRemove)
    {
        OnRemovePlayer?.Invoke(idToRemove);

        if (clients.ContainsKey(idToRemove))
        {
            Console.WriteLine("Removing client: " + idToRemove);
            pingPong.RemoveClientForList(idToRemove);
            ipToId.Remove(clients[idToRemove].ipEndPoint);

            string removedName = clients[idToRemove].clientName;
            clients.Remove(idToRemove);
            activePlayerNames.Remove(removedName);

            BroadcastPlayerListToMatchMaker();
        }
    }

    /// <summary>
    /// Handles incoming data received over the network.
    /// </summary>
    /// <param name="data">The data received.</param>
    /// <param name="ip">The IP address of the sender.</param>
    public override void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        // Invoke the event to notify listeners about the received message
        OnReceivedMessage?.Invoke(data, ip);

        OnReceivedMessagePriority(data, ip);

        MessageType messageType = MessageChecker.CheckMessageType(data);

        switch (messageType)
        {
            case MessageType.Bool:
            case MessageType.Byte:
            case MessageType.Char:
            case MessageType.Decimal:
            case MessageType.Double:
            case MessageType.Float:
            case MessageType.Int:
            case MessageType.Long:
            case MessageType.Sbyte:
            case MessageType.Short:
            case MessageType.String:
            case MessageType.Uint:
            case MessageType.Ulong:
            case MessageType.Remove:
            case MessageType.Empty:
            case MessageType.Null:
            case MessageType.Method:
            case MessageType.Event:
            case MessageType.Enum:
            case MessageType.TRS:
            case MessageType.Ushort:

                BroadcastPlayerPosition(ipToId[ip], data);

                if (MessageType.TRS == messageType)
                {
                    NetTRSMessage netTRSMessage = new NetTRSMessage(data);
                    TRS tRS = netTRSMessage.GetData();
                    NetObjTracker.UpdateNetObj(tRS, ipToId[ip], netTRSMessage.GetMessageRoute()[0].route);
                }

                break;
            case MessageType.Ping:

                if (ipToId.ContainsKey(ip))
                {
                    pingPong.ReciveClientToServerPingMessage(ipToId[ip]);
                    pingPong.CalculateLatencyFromClients(ipToId[ip]);
                }

                break;

            case MessageType.ClientToServerHandShake:

                ReceiveClientToServerHandShake(data, ip);

                break;

            case MessageType.Console:

                UpdateChatText(data);

                break;

            case MessageType.InstanceRequest:

                InstanceRequestPayload instanceRequest = new InstanceRequestMenssage(data).GetData();

                InstancePayload instancePayload = new(instancesIdCount, ipToId[ip], instanceRequest.objectId,
                                                                       instanceRequest.positionX, instanceRequest.positionY, instanceRequest.positionZ,
                                                                       instanceRequest.rotationX, instanceRequest.rotationY, instanceRequest.rotationZ, instanceRequest.rotationW,
                                                                       instanceRequest.scaleX, instanceRequest.scaleY, instanceRequest.scaleZ, instanceRequest.parentInstanceID);

                NetObjTracker.AddNetObj(instancePayload);
                Console.WriteLine("Added Net Obj to tracker.");

                InstanceMessage instanceMessage = new(MessagePriority.NonDisposable, instancePayload);

                Console.WriteLine("Send Instance Message");
                SendMessage(instanceMessage.Serialize());

                instancesIdCount++;

                break;

            case MessageType.Position:

                NetVector3 netVector3 = new(data);

                if (ipToId.ContainsKey(ip))
                {
                    if (sortableMessage.CheckMessageOrderRecievedFromClients(ipToId[ip], MessageChecker.CheckMessageType(data), netVector3.MessageOrder))
                    {
                        UpdatePlayerPosition(data);
                    }
                }

                break;

            case MessageType.BulletInstatiate:

                NetVector3 netBullet = new(data);
                OnInstantiateBullet?.Invoke(netBullet.GetData().id, netBullet.GetData().position);

                BroadcastPlayerPosition(netBullet.GetData().id, data);

                break;

            case MessageType.Disconnection:

                NetIDMessage netDisconnection = new(data);
                int playerID = netDisconnection.GetData();

                Broadcast(data);
                RemoveClient(playerID);

                break;

            case MessageType.DisconnectAll:

                Console.WriteLine("Disconnect Message arrived");
                NetDisconnectionMessage netDisconnectionMessage = new(data);
                Broadcast(data);
                CloseConnection();

                break;

            case MessageType.Winner:

                NetWinnerMessage netWinnerMessage = new(data);
                Console.WriteLine("Winner Message arrived");
                Broadcast(data);

                break;

            case MessageType.Error:

                NetErrorMessage netErrorMessage = new(data);
                CloseConnection();

                break;

            case MessageType.MatchMakerIp:

                matchmMakerIp = ip;
                Console.WriteLine("[Server] Registered MatchMaker IP: " + ip);

                break;

            case MessageType.DestroyNetObj:

                NetDestroyGO netDestroyGO = new(data);
                int playerId = netDestroyGO.GetData().Item1;
                int instanceId = netDestroyGO.GetData().Item2;

                NetObjTracker.RemoveNetObj(playerId, instanceId);

                Broadcast(data);

                Console.WriteLine("Removed Net Obj to tracker.");

                break;

            case MessageType.Confirm:

                Console.WriteLine("Message reception confirmed");

                break;

            default:
                Console.WriteLine("Not known: " + messageType);
                break;
        }

    }

    void OnReceivedMessagePriority(byte[] data, IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            if (MessageChecker.IsSorteableMessage(data))
            {
                sortableMessage?.OnRecievedData(data, ipToId[ip]);
            }
            if (MessageChecker.IsNondisponsableMessage(data))
            {
                nondisponsableMessage?.OnReceivedData(data, ipToId[ip]);
            }
        }
    }

    /// <summary>
    /// Broadcasts data to a specific endpoint.
    /// </summary>
    /// <param name="data">The data to broadcast.</param>
    /// <param name="ip">The IP endpoint to broadcast to.</param>
    public void Broadcast(byte[] data, IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            nondisponsableMessage?.AddSentMessages(data, ipToId[ip]);
        }

        //  string s = "SEND FROM SERVER = " + MessageChecker.CheckMessageType(data) + " - " + MessageChecker.CheckMessagePriority(data) + "[" + DateTime.UtcNow + "]";
        //  Console.WriteLine(s);

        connection.Send(data, ip);
    }

    public void BroadcastToMatchMaker(byte[] data)
    {
        connection.Send(data, matchmMakerIp);
    }

    /// <summary>
    /// Broadcasts data to all connected clients.
    /// </summary>
    /// <param name="data">The data to broadcast.</param>
    public void Broadcast(byte[] data)
    {
        using (Dictionary<int, Client>.Enumerator iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                Broadcast(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    /// <summary>
    /// Receives a handshake message from a client to the server.
    /// </summary>
    /// <param name="data">The data received.</param>
    /// <param name="ip">The IP endpoint of the client.</param>
    void ReceiveClientToServerHandShake(byte[] data, IPEndPoint ip)
    {
        ClientToServerNetHandShake handShake = new(data);

        if (!MatchOnGoing(ip) && CheckValidUserName(handShake.GetData().Item3, ip) && !ServerIsFull(ip))
        {
            AddClient(ip, clientID, handShake.GetData().Item3);
            clientID++;
        }
    }

    /// <summary>
    /// Checks if a match is ongoing.
    /// </summary>
    /// <param name="ip">The IP endpoint of the client.</param>
    /// <returns>True if a match is ongoing, false otherwise.</returns>
    bool MatchOnGoing(IPEndPoint ip)
    {
        if (matchOnGoing)
        {
            NetErrorMessage netServerIsFull = new("Match has already started");
            Broadcast(netServerIsFull.Serialize(), ip);
        }

        return matchOnGoing;
    }

    /// <summary>
    /// Checks if the server is full.
    /// </summary>
    /// <param name="ip">The IP endpoint of the client.</param>
    /// <returns>True if the server is full, false otherwise.</returns>
    bool ServerIsFull(IPEndPoint ip)
    {
        // Check if the number of connected clients exceeds the maximum allowed
        bool serverIsFull = clients.Count >= maxPlayersPerServer;

        if (serverIsFull)
        {
            NetErrorMessage netServerIsFull = new("Server is full");
            Broadcast(netServerIsFull.Serialize(), ip);
        }

        return serverIsFull;
    }

    /// <summary>
    /// Checks if a given username is valid.
    /// </summary>
    /// <param name="userName">The username to check.</param>
    /// <param name="ip">The IP endpoint of the client.</param>
    /// <returns>True if the username is valid, false otherwise.</returns>
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

    /// <summary>
    /// Updates the chat text with received message data.
    /// </summary>
    /// <param name="data">The message data received.</param>
    /// <param name="ip">The IP endpoint of the client.</param>
    protected override void UpdateChatText(byte[] data)
    {
        string messageText = "";

        NetMessage netMessage = new(data);
        messageText += new string(netMessage.GetData());

        Broadcast(data);
    }

    /// <summary>
    /// Handles the cleanup when the application is about to quit.
    /// </summary>
    public override void OnApplicationQuit()
    {
        Console.WriteLine("[Server] Application quitting � notifying MatchMaker");

        BroadcastPlayerListToMatchMaker(); // Will be empty if all clients disconnected properly

        NetErrorMessage netErrorMessage = new("Lost Connection To Server");
        Broadcast(netErrorMessage.Serialize());
        CloseConnection();
    }

    /// <summary>
    /// Closes the server and removes all connected clients.
    /// </summary>
    public override void CloseConnection()
    {
        List<byte[]> disconnectMessages = new();
        List<int> clientIdsToRemove = new(clients.Keys);

        foreach (int clientId in clientIdsToRemove)
        {
            NetIDMessage netDisconnection = new(MessagePriority.Default, clientId);
            disconnectMessages.Add(netDisconnection.Serialize());
        }

        foreach (int clientId in clientIdsToRemove)
        {
            RemoveClient(clientId);
        }

        foreach (byte[] message in disconnectMessages)
        {
            Broadcast(message);
        }

        BroadcastPlayerListToMatchMaker();
        System.Threading.Thread.Sleep(100);
        connection.Close();
    }


    /// <summary>
    /// Updates the position of a player based on received data.
    /// </summary>
    /// <param name="data">The data containing the player's position.</param>
    protected override void UpdatePlayerPosition(byte[] data)
    {
        NetVector3 netPosition = new(data);
        int clientId = netPosition.GetData().id;

        // Broadcast the player's position to all clients except the sender
        BroadcastPlayerPosition(clientId, data);
    }

    /// <summary>
    /// Broadcasts a player's position to all clients except the sender.
    /// </summary>
    /// <param name="senderClientId">The ID of the player sending the position.</param>
    /// <param name="data">The data containing the player's position.</param>
    private void BroadcastPlayerPosition(int senderClientId, byte[] data)
    {
        using (Dictionary<int, Client>.Enumerator iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                int receiverClientId = iterator.Current.Key;

                // Avoids sending the position back to the sender
                if (receiverClientId != senderClientId)
                {
                    // Check if the sender and receiver have the same IP endpoint
                    if (clients[receiverClientId].ipEndPoint.Equals(clients[senderClientId].ipEndPoint)) continue;
                    Broadcast(data, clients[receiverClientId].ipEndPoint);
                }
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if (connection != null)
        {
            nondisponsableMessage?.ResendPackages();
        }
    }

    public override void SendMessage(byte[] data)
    {
        Broadcast(data);
    }

    public override void SendMessage(byte[] data, int id)
    {
        if (clients.ContainsKey(id))
        {
            Broadcast(data, clients[id].ipEndPoint);
        }
    }

    private void BroadcastPlayerListToMatchMaker()
    {
        if (matchmMakerIp == null) return;

        List<string> names = new();
        foreach (Client client in clients.Values)
        {
            names.Add(client.clientName);
        }

        if (names.Count == 0)
            Console.WriteLine("[Server] Sent empty player list to MatchMaker");

        MatchMakerPlayerListUpdateMessage msg = new MatchMakerPlayerListUpdateMessage(MessagePriority.Default, names);
        connection.Send(msg.Serialize(), matchmMakerIp);
        Console.WriteLine("[Server] Sent player list to MatchMaker: " + string.Join(", ", names));
    }
}

public static class NetObjTracker
{
    static Dictionary<(int owner, int instance), InstancePayload> instancePayloadsDictionary = new Dictionary<(int, int), InstancePayload>();

    public static void AddNetObj(InstancePayload payload)
    {
        instancePayloadsDictionary.TryAdd((payload.ownerId, payload.instanceId), payload);
    }

    public static void RemoveNetObj(int owner, int instance)
    {
        instancePayloadsDictionary.Remove((owner, instance));
    }

    public static void UpdateNetObj(TRS tRS, int owner, int instance)
    {
        InstancePayload aux = instancePayloadsDictionary[(owner, instance)];
        aux.positionX = tRS.position.Item1;
        aux.positionY = tRS.position.Item2;
        aux.positionZ = tRS.position.Item3;
        aux.rotationX = tRS.rotation.Item1;
        aux.rotationY = tRS.rotation.Item2;
        aux.rotationZ = tRS.rotation.Item3;
        aux.rotationW = tRS.rotation.Item4;
        aux.scaleX = tRS.scale.Item1;
        aux.scaleY = tRS.scale.Item2;
        aux.scaleZ = tRS.scale.Item3;
        instancePayloadsDictionary[(owner, instance)] = aux;
    }
}