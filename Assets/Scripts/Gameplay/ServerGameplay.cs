using UnityEngine;

enum States { Init, Lobby, Game, Finish };

public class ServerGameplay : MonoBehaviour
{

    private int minutesInLobby = 10;
    private float minutesGameplay = 180;
    private float timeUntilCloseServer = 5;

    private int minPlayerToInitCounter = 2;

    private GameManager gm;
    private NetworkManager nm;
    private States currentState;

    bool initLobby = true;
    bool initGameplay = true;
    bool counterInit = false;

    private float counter = 0;

    bool clientLobbyTimer = false;
    bool clientGameplayTimer = false;

    private void Start()
    {
        gm = GameManager.Instance;
        nm = NetworkManager.Instance;

        gm.OnInitLobbyTimer += SetLobbyTimer;
        gm.OnInitGameplayTimer += SetGameplayTimer;
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

    private void UpdateServer()
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
                            NetUpdateTimer netUpdateLobbyTimer = new (MessagePriority.NonDisposable, true);
                            netUpdateLobbyTimer.CurrentMessageType = MessageType.UpdateLobbyTimer;
                            nm.Broadcast(netUpdateLobbyTimer.Serialize());
                            initLobby = false;
                        }

                        counter += Time.deltaTime;
                        gm.timer.text = counter.ToString("F2");

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
                            NetUpdateTimer netUpdateLobbyTimer = new (MessagePriority.NonDisposable, false);
                            netUpdateLobbyTimer.CurrentMessageType = MessageType.UpdateLobbyTimer;
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
                        NetUpdateTimer netUpdateGameplayTimer = new (MessagePriority.NonDisposable, true);
                        netUpdateGameplayTimer.CurrentMessageType = MessageType.UpdateGameplayTimer;
                        nm.Broadcast(netUpdateGameplayTimer.Serialize());

                        initGameplay = false;
                    }

                    counter += Time.deltaTime;
                    gm.timer.text = counter.ToString("F2");

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

    private void UpdateClient()
    {
        if (clientLobbyTimer)
        {
            counter += Time.deltaTime;
            gm.timer.text = counter.ToString("F2");
        }

        if (clientGameplayTimer)
        {
            clientLobbyTimer = false;

            counter += Time.deltaTime;
            gm.timer.text = counter.ToString("F2");
        }
    }

    private void SetGameplayTimer()
    {
        clientGameplayTimer = true;
        counter = 0;
    }

    private void SetLobbyTimer(bool init)
    {
        gm.timer.text = "";
        clientLobbyTimer = init;
    }

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

        NetIDMessage netIDMessage = new (MessagePriority.Default, playerWithMaxHealth.clientID);
        netIDMessage.CurrentMessageType = MessageType.Winner;
        nm.Broadcast(netIDMessage.Serialize());
    }
}