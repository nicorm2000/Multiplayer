using System.Net;
using UnityEngine;

enum States { Init, Lobby, Game, Finish };

public class ServerGameplay : MonoBehaviour
{
    private int minutesInLobby = 120;
    private float minutesGameplay = 180;
    private float timeUntilCloseServer = 5;

    private int minPlayerToInitCounter = 2;

    private GameManager gm;
    private NetworkManager nm;
    private States currentState;

    private bool counterInit = false;

    private bool initLobby = true;
    private bool initGameplay = true;

    private float counter = 0;

    private bool clientLobbyTimer = false;
    private bool clientGameplayTimer = false;


    private void Start()
    {
        gm = GameManager.Instance;
        nm = NetworkManager.Instance;

        nm.OnRecievedMessage += OnRecievedData;

        gm.OnInitLobbyTimer += SetLobbyTimer;
        gm.OnInitGameplayTimer += SetGameplayTimer;

        gm.OnChangeLobbyPlayers += CheckForAddNewPlayer;
    }

    void CheckForAddNewPlayer(int clientID)
    {
        if (nm.isServer && currentState == States.Lobby && counterInit)
        {
            NetUpdateNewPlayersTimer timer = new (MessagePriority.Default, counter);
            nm.Broadcast(timer.Serialize(), nm.clients[clientID].ipEndPoint);
        }
    }

    void OnRecievedData(byte[] data, IPEndPoint ip)
    {
        if (MessageChecker.CheckMessageType(data) == MessageType.UpdateLobbyTimerForNewPlayers)
        {
            Debug.Log("Timer message recieved");
            NetUpdateNewPlayersTimer timer = new (data);

            counter = timer.GetData();
            clientLobbyTimer = true;
        }
    }

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

    void UpdateServer()
    {
        if (nm != null && nm.isServer)
        {
            switch (currentState)
            {
                case States.Init:

                    currentState = States.Lobby;

                    break;
                case States.Lobby:

                    if (nm.clients.Count >= minPlayerToInitCounter)
                    {
                        counterInit = true;

                        if (initLobby)
                        {
                            NetUpdateTimer netUpdateLobbyTimer = new(MessagePriority.NonDisposable, true)
                            {
                                CurrentMessageType = MessageType.UpdateLobbyTimer
                            };
                            nm.Broadcast(netUpdateLobbyTimer.Serialize());
                            initLobby = false;
                        }

                        counter += Time.deltaTime;
                        gm.timer.text = counter.ToString("F2") + "s";

                        if (counter >= minutesInLobby)
                        {
                            counter = 0;
                            gm.timer.text = "";
                            nm.matchOnGoing = true;
                            currentState = States.Game;
                        }
                    }
                    else
                    {
                        if (counterInit)
                        {
                            NetUpdateTimer netUpdateLobbyTimer = new (MessagePriority.NonDisposable, false)
                            {
                                CurrentMessageType = MessageType.UpdateLobbyTimer
                            };
                            nm.Broadcast(netUpdateLobbyTimer.Serialize());

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
                        NetUpdateTimer netUpdateGameplayTimer = new(MessagePriority.NonDisposable, true)
                        {
                            CurrentMessageType = MessageType.UpdateGameplayTimer
                        };
                        nm.Broadcast(netUpdateGameplayTimer.Serialize());

                        initGameplay = false;
                    }

                    counter += Time.deltaTime;
                    gm.timer.text = counter.ToString("F2") + "s";

                    if (counter >= minutesGameplay)
                    {
                        SendMatchWinner();

                        gm.timer.text = "";
                        currentState = States.Finish;
                    }
                    break;

                case States.Finish:

                    timeUntilCloseServer -= Time.deltaTime;

                    if (timeUntilCloseServer <= 0)
                    {
                        nm.CloseServer();
                    }

                    break;

                default:
                    break;
            }
        }
    }

    void UpdateClient()
    {
        if (clientLobbyTimer)
        {
            counter += Time.deltaTime;
            gm.timer.text = counter.ToString("F2") + "s";
        }

        if (clientGameplayTimer && !NetworkScreen.Instance.gameObject.activeInHierarchy)
        {
            clientLobbyTimer = false;

            counter += Time.deltaTime;
            gm.timer.text = counter.ToString("F2") + "s";
        }
    }

    void SetGameplayTimer()
    {
        clientGameplayTimer = true;
        counter = 0;
    }

    void SetLobbyTimer(bool init)
    {
        gm.timer.text = "";
        clientLobbyTimer = init;
    }

    void SendMatchWinner()
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

        NetIDMessage netIDMessage = new(MessagePriority.Default, playerWithMaxHealth.clientID)
        {
            CurrentMessageType = MessageType.Winner
        };
        nm.Broadcast(netIDMessage.Serialize());
    }
}