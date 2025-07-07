using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using System;
using Net;

public class NetworkClient : NetworkEntity
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public struct Player
    {
        public int id;
        public string name;

        /// <summary>
        /// Initializes a new instance of the Player struct with the specified ID and name.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="name">The name of the player.</param>
        public Player(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    /// <summary>
    /// The IP address of the network manager.
    /// </summary>
    public IPAddress ipAddress
    {
        get; private set;
    }

    private readonly Dictionary<int, Player> players = new();

    protected GameManager gm;

    ClientPingPong pingPong;
    ClientSortableMessage sortableMessage;
    ClientNondisponsableMessage nondisposablesMessages;

    /// <summary>
    /// Starts the client with the specified IP address, port, and name.
    /// </summary>
    /// <param name="ip">The IP address of the server.</param>
    /// <param name="port">The port of the server.</param>
    /// <param name="name">The name of the client.</param>
    public NetworkClient(IPAddress ip, int port, string name) : base()
    {
        this.port = port;
        this.ipAddress = ip;
        this.userName = name;

        InitializeClient();
    }

    private async void InitializeClientWithDelay(int secondsToDelay)
    {
        await Task.Delay(secondsToDelay * 1000);
        InitializeClient();
    }

    void InitializeClient()
    {
        gm = GameManager.Instance;

        Debug.Log($"Initialize Client = IP: {ipAddress} - Port: {port} - DataReceive {this}");

        connection = new UdpConnection(ipAddress, port, this);

        onInitPingPong += () => nondisposablesMessages = new(this);
        onInitPingPong += () => sortableMessage = new(this);

        ClientToServerNetHandShake handShakeMesage = new(MessagePriority.NonDisposable, (UdpConnection.IPToLong(ipAddress), port, userName));
        SendToServer(handShakeMesage.Serialize());
        NetworkScreen.Instance.SwitchToChatScreen();
    }

    /// <summary>
    /// Adds a new client to the server.
    /// </summary>
    /// <param name="ip">The IP endpoint of the client.</param>
    /// <param name="newClientID">The ID of the new client.</param>
    /// <param name="clientName">The name of the new client.</param>
    public override void AddClient(IPEndPoint ip, int newClientID, string clientName)
    {
        Console.WriteLine("Adding Client: " + ip.Address);

        OnNewPlayer?.Invoke(newClientID);
    }

    /// <summary>
    /// Removes a client from the server.
    /// </summary>
    /// <param name="idToRemove">The ID of the client to remove.</param>
    public override void RemoveClient(int idToRemove)
    {
        OnRemovePlayer?.Invoke(idToRemove);

        Debug.Log("Removing client: " + idToRemove);
        players.Remove(idToRemove);

        if (clientID == idToRemove)
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Handles incoming data received over the network.
    /// </summary>
    /// <param name="data">The data received.</param>
    /// <param name="ip">The IP address of the sender.</param>
    public override void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        if (data == null || ip == null)
        {
            Debug.Log("On Receive IP or Data NULL");
            return;
        }

        OnReceivedMessage?.Invoke(data, ip);


        OnReceivedMessagePriority(data);

        switch (MessageChecker.CheckMessageType(data))
        {
            case MessageType.Ping:

                pingPong.ReciveServerToClientPingMessage();
                pingPong.CalculateLatencyFromServer();

                break;

            case MessageType.AssignServer:

                NetIDMessage netDisconection = new(MessagePriority.Default, clientID);
                netDisconection.CurrentMessageType = MessageType.Disconnection;
                SendToServer(netDisconection.Serialize());

                port = new NetAssignServerMessage(data).GetData();

                InitializeClientWithDelay(2);

                break;

            case MessageType.MatchMakerToClientHandShake:

                clientID = new NetIDMessage(data).GetData();
                if (checkActivity == null)
                {
                    pingPong = new ClientPingPong(this);
                    checkActivity = pingPong;
                    onInitPingPong?.Invoke();
                }

                if (NetworkScreen.Instance.isInMenu)
                {
                    NetworkScreen.Instance.SwitchToChatScreen();
                }

                OnNewPlayer?.Invoke(new NetIDMessage(data).GetData());

                break;

            case MessageType.ServerToClientHandShake:

                ServerToClientHandShake netGetClientID = new(data);
                List<(int clientId, string userName)> playerList = netGetClientID.GetData();

                if (checkActivity == null)
                {
                    pingPong = new ClientPingPong(this);
                    checkActivity = pingPong;
                    onInitPingPong?.Invoke();
                }

                for (int i = 0; i < playerList.Count; i++) // First verify which client am I
                {
                    if (playerList[i].userName == userName)
                    {
                        if (NetworkScreen.Instance.isInMenu)
                        {
                            NetworkScreen.Instance.SwitchToChatScreen();
                        }

                        clientID = playerList[i].clientId;
                    }
                }

                players.Clear();

                for (int i = 0; i < playerList.Count; i++)
                {
                    Player playerToAdd = new(playerList[i].clientId, playerList[i].userName);
                    players.Add(playerList[i].clientId, playerToAdd);
                    OnNewPlayer?.Invoke(playerToAdd.id);
                }

                break;

            case MessageType.Instance:

                InstancePayload instancePayload = new InstanceMessage(data).GetData();

                // Obtengo los prefabs ID del objeto y el padre;
                IPrefabService prefabService = ServiceProvider.GetService<IPrefabService>();
                GameObject prefab = prefabService.GetPrefabById(instancePayload.objectId);
                INetObj parentObj = NetObjFactory.GetINetObject(instancePayload.parentInstanceID);


                GameObject instance = MonoBehaviour.Instantiate(prefab, new Vector3(instancePayload.positionX, instancePayload.positionY, instancePayload.positionZ),
                                                                       new Quaternion(instancePayload.rotationX, instancePayload.rotationY, instancePayload.rotationZ, instancePayload.rotationW));

                if (parentObj != null)
                {
                    instance.transform.SetParent(((GameObject)(parentObj as object)).transform);
                }

                instance.transform.localScale = new Vector3(instancePayload.scaleX, instancePayload.scaleY, instancePayload.scaleZ);


                if (instance.TryGetComponent(out INetObj obj))
                {
                    obj.GetNetObj().SetValues(instancePayload.instanceId, instancePayload.ownerId);

                    NetObjFactory.AddINetObject(obj.GetID(), obj);

                    NetworkManager.Instance.onInstanceCreated?.Invoke(obj.GetOwnerID(), instance); //Enivo un evento con el objeto instanciado y su owner
                }

                break;

            case MessageType.Console:

                UpdateChatText(data);

                break;

            case MessageType.Position:

                NetVector3 netVector3 = new(data);

                if (sortableMessage.CheckMessageOrderRecievedFromServer(netVector3.GetData().id, MessageType.Position, netVector3.MessageOrder))
                {
                    UpdatePlayerPosition(data);
                }

                break;

            case MessageType.BulletInstatiate:

                NetVector3 netBullet = new(data);
                OnInstantiateBullet?.Invoke(netBullet.GetData().id, netBullet.GetData().position);

                break;

            case MessageType.Disconnection:

                NetIDMessage netDisconnection = new(data);
                int playerID = netDisconnection.GetData();

                Console.WriteLine("Remove player " + playerID);
                RemoveClient(playerID);

                break;

            case MessageType.UpdateLobbyTimer:

                NetUpdateTimer netUpdate = new(data);
                gm.OnInitLobbyTimer?.Invoke(netUpdate.GetData());

                break;

            case MessageType.UpdateGameplayTimer:

                gm.OnInitGameplayTimer?.Invoke();

                break;

            case MessageType.Error:

                NetErrorMessage netErrorMessage = new(data);
                NetworkScreen.Instance.SwitchToMenuScreen();
                NetworkScreen.Instance.ShowErrorPanel(netErrorMessage.GetData());

                CloseConnection();

                break;

            case MessageType.Winner:

                Debug.Log("Winner");
                NetWinnerMessage netWin = new(data);
                Debug.Log("Winner number: " + netWin.GetData().winner);
                string winText = $"Congratulations! \n {players[netWin.GetData().winner].name} won the game!";
                NetworkScreen.Instance.SwitchToMenuScreen();
                NetworkScreen.Instance.ShowWinPanel(winText);

                DisconnectAll disconnectAll = new();
                NetDisconnectionMessage netDisconnectionMessage = new(disconnectAll);
                SendMessage(netDisconnectionMessage.Serialize());

                break;

            case MessageType.DisconnectAll:

                Debug.Log("Disconnect All");
                NetObjFactory.RemoveAllINetObject();
                for (int i = 0; i < players.Count; i++)
                {
                    RemoveClient(i);
                }
                CloseConnection();

                break;

            case MessageType.DestroyNetObj:

                Debug.Log("Net Destroy enter");
                NetDestroyGO netDestroyGO = new NetDestroyGO(data);
                foreach (INetObj netObjAux in NetObjFactory.NetObjects())
                {
                    if (netObjAux.GetID() == netDestroyGO.GetData().Item1)
                    {
                        NetObjFactory.RemoveINetObject(netDestroyGO.GetData().Item1);
                        GameObject.Destroy(((MonoBehaviour)netObjAux).gameObject);
                        Debug.Log("Object Destroyed");
                    }
                }

                break;

            default:
                break;
        }

    }

    void OnReceivedMessagePriority(byte[] data)
    {
        if (MessageChecker.IsSorteableMessage(data))
        {
            sortableMessage?.OnRecievedData(data, -1);
        }
        if (MessageChecker.IsNondisponsableMessage(data))
        {
            nondisposablesMessages?.OnReceivedData(data, -1);
        }
    }

    /// <summary>
    /// Sends data to the server.
    /// </summary>
    /// <param name="data">The data to send.</param>
    public void SendToServer(byte[] data)
    {
        if (connection == null) return;

        nondisposablesMessages?.AddSentMessages(data);
        connection.Send(data);
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

        ChatScreen.Instance.messages.text += messageText + System.Environment.NewLine;
    }

    /// <summary>
    /// Handles the cleanup when the application is about to quit.
    /// </summary>
    public override void OnApplicationQuit()
    {
        // Notify the server about the client's disconnection
        NetIDMessage netDisconnection = new(MessagePriority.Default, clientID);
        SendToServer(netDisconnection.Serialize());
    }

    /// <summary>
    /// Disconnects the player from the network.
    /// </summary>
    public override void CloseConnection()
    {
        connection.Close();

        gm.RemoveAllPlayers();

        checkActivity = null;
        pingPong = null;
        nondisposablesMessages = null;
        sortableMessage = null;

        NetworkScreen.Instance.SwitchToMenuScreen();
    }

    /// <summary>
    /// Updates the position of a player based on received data.
    /// </summary>
    /// <param name="data">The data containing the player's position.</param>
    protected override void UpdatePlayerPosition(byte[] data)
    {
        NetVector3 netPosition = new(data);
        int clientId = netPosition.GetData().id;

        Vec3 position = netPosition.GetData().position;
        gm.UpdatePlayerPosition((clientId, new UnityEngine.Vector3(position.x, position.y, position.z)));
    }

    public override void Update()
    {
        base.Update();

        if (connection != null)
        {
            nondisposablesMessages?.ResendPackages();
        }
    }

    public override void SendMessage(byte[] data)
    {
        SendToServer(data);
    }

    public override void SendMessage(byte[] data, int id = -1) //Es una sobrecarga que solo usa el SERVER
    {
        SendToServer(data);
    }
}
