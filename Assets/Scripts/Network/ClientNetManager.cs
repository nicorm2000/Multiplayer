using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientNetManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
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

    /// <summary>
    /// The port of the network manager.
    /// </summary>
    public int port
    {
        get; private set;
    }

    private UdpConnection connection;

    public Action<byte[], IPEndPoint> OnReceivedMessage;

    private readonly Dictionary<int, Player> players = new();

    public string userName = "Server";
    public int actualClientId = -1;

    public PingPong checkActivity;

    private GameManager gm;
    private SortableMessage sortableMessages;
    private NonDisposableMessage nonDisposablesMessages;

    private void Start()
    {
        gm = GameManager.Instance;
        sortableMessages = new();
        nonDisposablesMessages = new();
    }

    /// <summary>
    /// Starts the client with the specified IP address, port, and name.
    /// </summary>
    /// <param name="ip">The IP address of the server.</param>
    /// <param name="port">The port of the server.</param>
    /// <param name="name">The name of the client.</param>
    public void StartClient(IPAddress ip, int port, string name)
    {
        this.port = port;
        this.ipAddress = ip;
        this.userName = name;

        connection = new UdpConnection(ip, port, this);
        checkActivity = new PingPong();

        ClientToServerNetHandShake handShakeMesage = new(MessagePriority.NonDisposable, (UdpConnection.IPToLong(ip), port, name));
        SendToServer(handShakeMesage.Serialize());
    }

    /// <summary>
    /// Adds a new client to the server.
    /// </summary>
    /// <param name="ip">The IP endpoint of the client.</param>
    /// <param name="newClientID">The ID of the new client.</param>
    /// <param name="clientName">The name of the new client.</param>
    public void AddClient(IPEndPoint ip, int newClientID, string clientName)
    {
        Debug.Log("Adding Client: " + ip.Address);

        checkActivity.AddClientForList(newClientID);
        gm.OnNewPlayer?.Invoke(newClientID);
    }

    /// <summary>
    /// Removes a client from the server.
    /// </summary>
    /// <param name="idToRemove">The ID of the client to remove.</param>
    public void RemoveClient(int idToRemove)
    {
        gm.OnRemovePlayer?.Invoke(idToRemove);

        Debug.Log("Removing client: " + idToRemove);
        checkActivity.RemoveClientForList(idToRemove);
        players.Remove(idToRemove);

        if (actualClientId == idToRemove)
        {
            NetworkScreen.Instance.SwitchToMenuScreen();
        }
    }

    /// <summary>
    /// Handles incoming data received over the network.
    /// </summary>
    /// <param name="data">The data received.</param>
    /// <param name="ip">The IP address of the sender.</param>
    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        // Invoke the event to notify listeners about the received message
        OnReceivedMessage?.Invoke(data, ip);

        switch (MessageChecker.CheckMessageType(data))
        {
            case MessageType.Ping:

                checkActivity.ReceiveServerToClientPingMessage();
                checkActivity.CalculateServerLatency();

                break;

            case MessageType.ServerToClientHandShake:

                ServerToClientHandShake netGetClientID = new(data);
                List<(int clientId, string userName)> playerList = netGetClientID.GetData();

                for (int i = 0; i < playerList.Count; i++) // First verify which client am I
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
                    Player playerToAdd = new(playerList[i].clientId, playerList[i].userName);
                    players.Add(playerList[i].clientId, playerToAdd);
                    gm.OnNewPlayer?.Invoke(playerToAdd.id);
                }

                break;

            case MessageType.Console:

                UpdateChatText(data, ip);

                break;

            case MessageType.Position:

                NetVector3 netVector3 = new(data);

                if (sortableMessages.CheckMessageOrderReceivedFromServer(netVector3.GetData().id, MessageType.Position, netVector3.MessageOrder))
                {
                    UpdatePlayerPosition(data);
                }

                break;

            case MessageType.BulletInstatiate:

                NetVector3 netBullet = new(data);
                gm.OnInstantiateBullet?.Invoke(netBullet.GetData().id, netBullet.GetData().position);

                break;

            case MessageType.Disconnection:

                NetIDMessage netDisconnection = new(data);
                int playerID = netDisconnection.GetData();

                Debug.Log("Remove player " + playerID);
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

    /// <summary>
    /// Sends data to the server.
    /// </summary>
    /// <param name="data">The data to send.</param>
    public void SendToServer(byte[] data)
    {
        nonDisposablesMessages.AddSentMessagesFromClients(data);
        connection.Send(data);
    }

    /// <summary>
    /// Updates the network manager.
    /// </summary>
    void Update()
    {
        // Flush the data in the main thread
        if (connection != null)
        {
            connection.FlushReceiveData();
            checkActivity?.UpdateCheckActivity();
            nonDisposablesMessages?.ResendPackages();
        }
    }

    /// <summary>
    /// Updates the chat text with received message data.
    /// </summary>
    /// <param name="data">The message data received.</param>
    /// <param name="ip">The IP endpoint of the client.</param>
    private void UpdateChatText(byte[] data, IPEndPoint ip)
    {
        string messageText = "";

        NetMessage netMessage = new(data);
        messageText += new string(netMessage.GetData());

        ChatScreen.Instance.messages.text += messageText + System.Environment.NewLine;
    }

    /// <summary>
    /// Handles the cleanup when the application is about to quit.
    /// </summary>
    void OnApplicationQuit()
    {
        // Notify the server about the client's disconnection
        NetIDMessage netDisconnection = new(MessagePriority.Default, actualClientId);
        SendToServer(netDisconnection.Serialize());
    }

    /// <summary>
    /// Disconnects the player from the network.
    /// </summary>
    public void DisconectPlayer()
    {
        connection.Close();
        NetworkScreen.Instance.SwitchToMenuScreen();
    }

    /// <summary>
    /// Updates the position of a player based on received data.
    /// </summary>
    /// <param name="data">The data containing the player's position.</param>
    private void UpdatePlayerPosition(byte[] data)
    {
        NetVector3 netPosition = new(data);
        int clientId = netPosition.GetData().id;

        gm.UpdatePlayerPosition(netPosition.GetData());
    }
}