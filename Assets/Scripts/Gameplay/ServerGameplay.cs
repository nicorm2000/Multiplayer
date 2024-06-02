using System.Net;
using UnityEngine;

enum States { Init, Lobby, Game, Finish };

public class ServerGameplay : MonoBehaviour
{
    int minutesInLobby = 15;
    float minutesGameplay = 30;
    float timeUntilCloseServer = 5;

    int minPlayerToInitCounter = 2;

    GameManager gm;
    NetworkManager nm;
    States currentState;

    bool counterInit = false;

    bool initLobby = true;
    bool initGameplay = true;

    float counter = 0;

    bool clientLobbyTimer = false;
    bool clientGameplayTimer = false;

    NetworkServer server;

    /// <summary>
    /// Initializes the GameManager and NetworkManager instances, and subscribes to necessary events.
    /// </summary>
    private void Start()
    {
        gm = GameManager.Instance;
        nm = NetworkManager.Instance;

        nm.onInitEntity += () => nm.networkEntity.OnReceivedMessage += OnReceivedData;

        gm.OnInitLobbyTimer += SetLobbyTimer;
        gm.OnInitGameplayTimer += SetGameplayTimer;

        gm.OnChangeLobbyPlayers += CheckForAddNewPlayer;
    }

    /// <summary>
    /// Checks if a new player has been added during the lobby state and updates the lobby timer for new players.
    /// </summary>
    /// <param name="clientID">The ID of the newly added client.</param>
    private void CheckForAddNewPlayer(int clientID)
    {
        if (nm.isServer && currentState == States.Lobby && counterInit)
        {
            Debug.Log("Mando el tiempo a nuevos clientes en el lobby");
            NetUpdateNewPlayersTimer timer = new NetUpdateNewPlayersTimer(MessagePriority.NonDisposable, counter);
            server.Broadcast(timer.Serialize(), server.clients[clientID].ipEndPoint);
        }
    }

    /// <summary>
    /// Handles received network data and updates the lobby timer if the message type is UpdateLobbyTimerForNewPlayers.
    /// </summary>
    /// <param name="data">The received data as a byte array.</param>
    /// <param name="ip">The IP endpoint from which the data was received.</param>
    private void OnReceivedData(byte[] data, IPEndPoint ip)
    {
        if (MessageChecker.CheckMessageType(data) == MessageType.UpdateLobbyTimerForNewPlayers)
        {
            Debug.Log("Llego mensaje de update timer");
            NetUpdateNewPlayersTimer timer = new NetUpdateNewPlayersTimer(data);

            counter = timer.GetData();
            clientLobbyTimer = true;
        }
    }

    /// <summary>
    /// Updates the server or client state each frame based on whether the instance is a server or client.
    /// </summary>
    private void Update()
    {
        if (nm.isServer)
        {
            UpdateServer();
        }
        else
        {
            UpdateClient();
        }
    }

    /// <summary>
    /// Manages the state transitions and game logic for the server.
    /// </summary>
    private void UpdateServer()
    {
        if (nm != null && nm.isServer)
        {
            if (server == null)
            {
                server = nm.GetNetworkServer();
            }

            switch (currentState)
            {
                case States.Init:

                    currentState = States.Lobby;

                    break;
                case States.Lobby:

                    if (server.clients.Count >= minPlayerToInitCounter)
                    {
                        counterInit = true;

                        if (initLobby)
                        {
                            NetUpdateTimer netUpdateLobbyTimer = new NetUpdateTimer(MessagePriority.NonDisposable, true);
                            netUpdateLobbyTimer.CurrentMessageType = MessageType.UpdateLobbyTimer;
                            server.Broadcast(netUpdateLobbyTimer.Serialize());
                            initLobby = false;
                        }

                        counter += Time.deltaTime;
                        gm.timer.text = counter.ToString("F0");

                        if (counter >= minutesInLobby)
                        {
                            counter = 0;
                            gm.timer.text = "";
                            server.matchOnGoing = true;
                            currentState = States.Game;
                        }
                    }
                    else
                    {
                        if (counterInit)
                        {
                            NetUpdateTimer netUpdateLobbyTimer = new NetUpdateTimer(MessagePriority.NonDisposable, false);
                            netUpdateLobbyTimer.CurrentMessageType = MessageType.UpdateLobbyTimer;
                            server.Broadcast(netUpdateLobbyTimer.Serialize());

                            counterInit = false;
                            initLobby = true;

                            counter = 0;
                            gm.timer.text = "";

                            currentState = States.Init;

                        }
                    }

                    break;
                case States.Game:

                    if (initGameplay)
                    {
                        NetUpdateTimer netUpdateGameplayTimer = new NetUpdateTimer(MessagePriority.NonDisposable, true);
                        netUpdateGameplayTimer.CurrentMessageType = MessageType.UpdateGameplayTimer;
                        server.Broadcast(netUpdateGameplayTimer.Serialize());

                        initGameplay = false;
                    }

                    counter += Time.deltaTime;
                    gm.timer.text = counter.ToString("F0");

                    if (counter >= minutesGameplay || gm.playerList.Count <= 1)
                    {
                        SendMatchWinner();

                        counter = 0;
                        gm.timer.text = "";
                        currentState = States.Finish;
                    }
                    break;

                case States.Finish:

                    timeUntilCloseServer -= Time.deltaTime;

                    if (timeUntilCloseServer <= 0)
                    {
                        server.CloseServer();
                    }

                    break;


                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Manages the state updates and timer display for the client.
    /// </summary>
    private void UpdateClient()
    {
        if (clientLobbyTimer)
        {
            counter += Time.deltaTime;
            gm.timer.text = counter.ToString("F0");
        }

        if (clientGameplayTimer && !NetworkScreen.Instance.gameObject.activeInHierarchy)
        {
            clientLobbyTimer = false;

            counter += Time.deltaTime;
            gm.timer.text = counter.ToString("F0");
        }
    }

    /// <summary>
    /// Initializes the gameplay timer for the client.
    /// </summary>
    private void SetGameplayTimer()
    {
        clientGameplayTimer = true;
        counter = 0;
    }

    /// <summary>
    /// Sets the lobby timer for the client based on the given initialization state.
    /// </summary>
    /// <param name="init">A boolean indicating whether to initialize the lobby timer.</param>
    private void SetLobbyTimer(bool init)
    {
        counter = 0;
        gm.timer.text = "";
        clientLobbyTimer = init;
    }

    /// <summary>
    /// Determines the player with the maximum health and sends a message to all clients indicating the match winner.
    /// </summary>
    private void SendMatchWinner()
    {
        PlayerController playerWithMaxHealth = null;
        int maxHealth = int.MinValue;

        foreach (int index in gm.playerList.Keys)
        {
            if (gm.playerList[index].TryGetComponent(out PlayerController pc))
            {
                if (pc.health > maxHealth)
                {
                    maxHealth = pc.health;
                    playerWithMaxHealth = pc;
                }
            }
        }

        NetIDMessage netIDMessage = new NetIDMessage(MessagePriority.Default, playerWithMaxHealth.clientID);
        netIDMessage.CurrentMessageType = MessageType.Winner;
        nm.GetNetworkServer().Broadcast(netIDMessage.Serialize());
    }
}