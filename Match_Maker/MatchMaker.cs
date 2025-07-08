using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Net;

namespace Match_Maker
{
    class MatchMaker : NetworkEntity
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
        private Dictionary<int, HashSet<string>> activeNamesByServer = new();
        private HashSet<int> usedPorts = new();
        public readonly Dictionary<IPEndPoint, int> ipToId = new();

        DateTime appStartTime;

        ServerPingPong pingPong;
        ServerSortableMessage sortableMessage;
        ServerNondisponsableMessage nondisponsableMessage;
        Dictionary<int, IPEndPoint> serversIps = new Dictionary<int, IPEndPoint>();
        List<Process> serversApplicationRunnnig = new();

        int minPlayerToStartGame = 2;
        int serverPort = 0;

        //int serverNumber = 0;
        //int playerCounterTotalServers = 0;

        /// <summary>
        /// Starts the server on the specified port.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public MatchMaker(int port, DateTime appStartTime) : base()
        {
            this.port = port;
            serverPort = port;

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
                Client clientAux = new(ip, newClientID, Convert.ToSingle((DateTime.UtcNow - appStartTime).TotalSeconds), clientName);
                clients.Add(newClientID, clientAux);
                pingPong.AddClientForList(newClientID);
                //OnNewPlayer?.Invoke(newClientID);  El Lobby no instanca a los players

                NetIDMessage netIDMessage = new(MessagePriority.NonDisposable, newClientID);
                netIDMessage.CurrentMessageType = MessageType.MatchMakerToClientHandShake;
                Broadcast(netIDMessage.Serialize(), ip);

                CheckPlayerInLobby();
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

                string clientName = clients[idToRemove].clientName;

                pingPong.RemoveClientForList(idToRemove);
                ipToId.Remove(clients[idToRemove].ipEndPoint);
                clients.Remove(idToRemove);

                foreach (HashSet<string> nameSet in activeNamesByServer.Values)
                {
                    nameSet.Remove(clientName);
                }

                CheckPlayerInLobby();
            }
        }

        /// <summary>
        /// Handles incoming data received over the network.
        /// </summary>
        /// <param name="data">The data received.</param>
        /// <param name="ip">The IP address of the sender.</param>
        public override void OnReceiveData(byte[] data, IPEndPoint ip) 
        {
            OnReceivedMessage?.Invoke(data, ip);

            if (data != null && MessageChecker.CheckMessageType(data) != MessageType.Ping && ipToId.ContainsKey(ip))
            {
                Console.WriteLine("RECEIVE (" + ipToId[ip] + ") = " + MessageChecker.CheckMessageType(data) + " - " + MessageChecker.CheckMessagePriority(data));
            }

            OnReceivedMessagePriority(data, ip);

            switch (MessageChecker.CheckMessageType(data))
            {
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

                case MessageType.Disconnection:

                    NetIDMessage netDisconnection = new(data);
                    int playerID = netDisconnection.GetData();

                    Broadcast(data);
                    RemoveClient(playerID);

                    break;

                case MessageType.Error:

                    CloseConnection();

                    break;

                case MessageType.MatchMakerPlayerListUpdate:
                    {
                        MatchMakerPlayerListUpdateMessage updateMsg = new(data);
                        int senderPort = ip.Port;

                        Console.WriteLine("[MatchMaker] Received player list update from port " + senderPort + ":");
                        foreach (string name in updateMsg.GetData())
                        {
                            Console.WriteLine(" - " + name);
                        }

                        if (!activeNamesByServer.ContainsKey(senderPort))
                            activeNamesByServer[senderPort] = new HashSet<string>();

                        HashSet<string> serverNameSet = activeNamesByServer[senderPort];

                        serverNameSet.Clear();

                        foreach (var name in updateMsg.GetData())
                        {
                            serverNameSet.Add(name);
                        }

                        Console.WriteLine("[MatchMaker] Current names in port " + senderPort + ":");
                        foreach (string name in activeNamesByServer[senderPort])
                        {
                            Console.WriteLine(" -> " + name);
                        }

                        break;
                    }

                default:
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

            connection.Send(data, ip);
        }

        /// <summary>
        /// Broadcasts data to all connected clients.
        /// </summary>
        /// <param name="data">The data to broadcast.</param>
        public void Broadcast(byte[] data)
        {
            using (var iterator = clients.GetEnumerator())
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

            if (CheckValidUserName(handShake.GetData().Item3, ip))
            {
                AddClient(ip, clientID, handShake.GetData().Item3);
                clientID++;
            }
        }

        /// <summary>
        /// Checks if a given username is valid.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <param name="ip">The IP endpoint of the client.</param>
        /// <returns>True if the username is valid, false otherwise.</returns>
        bool CheckValidUserName(string userName, IPEndPoint ip)
        {
            Console.WriteLine("[MatchMaker] Checking name: " + userName);

            bool hasUpperCase = false;
            bool hasLowerCase = false;

            foreach (char c in userName)
            {
                if (char.IsUpper(c))
                    hasUpperCase = true;
                if (char.IsLower(c))
                    hasLowerCase = true;

                if (hasUpperCase && hasLowerCase)
                {
                    NetErrorMessage netInvalidUserName = new("Invalid username letter case");
                    Broadcast(netInvalidUserName.Serialize(), ip);
                    return false;
                }
            }

            if (!IsToLower(userName) && !IsToUpper(userName))
            {
                NetErrorMessage netInvalidUserName = new("Invalid username letter case");

                Broadcast(netInvalidUserName.Serialize(), ip);

                return false;
            }

            foreach (int clientID in clients.Keys)
            {
                // Chequea que sea o TODO minuscula o TODO mayuscula, dsp chequea qe no sea un nombre que este en uso
                if (userName == clients[clientID].clientName)
                {
                    NetErrorMessage netInvalidUserName = new("Invalid User Name");
                    Broadcast(netInvalidUserName.Serialize(), ip);
                    return false;
                }
            }

            foreach (HashSet<string> nameSet in activeNamesByServer.Values)
            {
                if (nameSet.Contains(userName))
                {
                    NetErrorMessage netInvalidUserName = new("Username already in use in another match.");
                    Broadcast(netInvalidUserName.Serialize(), ip);
                    Console.WriteLine("[MatchMaker] Name is already taken in another match: " + userName);
                    return false;
                }
            }

            return true;
        }

        bool IsToUpper(string userName)
        {
            return userName.All(c => !char.IsLetter(c) || char.IsUpper(c));
        }

        bool IsToLower(string userName)
        {
            return userName.All(c => !char.IsLetter(c) || char.IsLower(c));
        }

        /// <summary>
        /// Updates the chat text with received message data.
        /// </summary>
        /// <param name="data">The message data received.</param>
        /// <param name="ip">The IP endpoint of the client.</param>
        protected override void UpdateChatText(byte[] data)
        {
            Broadcast(data);
        }

        /// <summary>
        /// Handles the cleanup when the application is about to quit.
        /// </summary>
        public override void OnApplicationQuit()
        {
            // Notify all clients about the server's disconnection and close the server
            CloseConnection();
        }

        /// <summary>
        /// Closes the server and removes all connected clients.
        /// </summary>
        public override void CloseConnection()
        {
            // Notify all clients about their disconnection and remove them
            NetErrorMessage netErrorMessage = new("Lost Connection To Match Maker");
            Broadcast(netErrorMessage.Serialize());

            List<int> clientIdsToRemove = new(clients.Keys);

            foreach (int clientId in clientIdsToRemove)
            {
                NetIDMessage netDisconnection = new(MessagePriority.Default, clientId);
                Broadcast(netDisconnection.Serialize());
                RemoveClient(clientId);
            }

            foreach (Process process in serversApplicationRunnnig)
            {
                process.Close();
            }

            connection.Close();
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

        void CheckPlayerInLobby()
        {
            Console.WriteLine("Clients count: " + clients.Count);

            if (clients.Count < minPlayerToStartGame)
            {
                return;
            }

            Func<string, bool> IsToUpper = wordUpper => wordUpper == wordUpper.ToUpper();
            Func<string, bool> IsToLower = wordLower => wordLower == wordLower.ToLower();

            int upperUserNamesCount = clients.Count(c => IsToUpper(c.Value.clientName));
            int lowerUserNamesCount = clients.Count(c => IsToLower(c.Value.clientName));

            if (upperUserNamesCount < minPlayerToStartGame && lowerUserNamesCount < minPlayerToStartGame)
            {
                return;
            }

            int newServerPort = CreateNewServer();

            System.Threading.Thread.Sleep(1000);

            if (upperUserNamesCount >= minPlayerToStartGame)
            {
                List<Client> toUpperClients = clients.Where(c => IsToUpper(c.Value.clientName)).Take(minPlayerToStartGame).Select(pair => pair.Value).ToList();

                foreach (Client client in toUpperClients)
                {
                    //Mando un NetAssignMessage
                    NetAssignServerMessage netAssignServerMessage = new(MessagePriority.NonDisposable, newServerPort);
                    Console.WriteLine($"Re direct client {client.id} - {client.clientName} To new server in port {newServerPort}");
                    Broadcast(netAssignServerMessage.Serialize(), client.ipEndPoint);
                }
                return;
            }

            if (lowerUserNamesCount >= minPlayerToStartGame)
            {
                List<Client> toLowerClients = clients.Where(c => IsToLower(c.Value.clientName)).Take(minPlayerToStartGame).Select(pair => pair.Value).ToList();

                foreach (Client client in toLowerClients)
                {
                    //Mando un NetAssignMessage
                    Console.WriteLine($"Re direct client {client.id} - {client.clientName} To new server in port {newServerPort} ");
                    NetAssignServerMessage netAssignServerMessage = new(MessagePriority.NonDisposable, newServerPort);
                    Broadcast(netAssignServerMessage.Serialize(), client.ipEndPoint);
                }
                return;
            }
        }

        int CreateNewServer()
        {
            do
            {
                serverPort++;
            } while (usedPorts.Contains(serverPort)); // find an unused port

            usedPorts.Add(serverPort);

            Process proc = CreateServerProcess(serverPort);
            serversApplicationRunnnig.Add(proc);

            // Give the server some time to initialize before sending the handshake
            System.Threading.Thread.Sleep(200);

            // Register server's IP and port
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint serverEndPoint = new(ipAddress, serverPort);
            serversIps[serverPort] = serverEndPoint;

            MatchMakerIpMessage msg = new MatchMakerIpMessage(MessagePriority.Default, ipAddress.ToString());
            Broadcast(msg.Serialize(), serverEndPoint);

            Console.WriteLine("[MatchMaker] Sent MatchMakerIp message to server at port " + serverPort);

            return serverPort;
        }

        Process CreateServerProcess(int numberPort)
        {
            Process currentServer;
            ProcessStartInfo startInfo = new ProcessStartInfo();

            string serverPath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName + "\\Build\\Multiplayer.exe";

            startInfo.FileName = serverPath;
            startInfo.Arguments = numberPort.ToString();

            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;

            currentServer = Process.Start(startInfo);
            Console.WriteLine($"MM succesfully create Server id{currentServer.Id} in port {numberPort} ({DateTime.UtcNow})");

            return currentServer;
        }

        protected override void UpdatePlayerPosition(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
