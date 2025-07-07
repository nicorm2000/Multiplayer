using UnityEngine;

enum States { Init, Lobby, Game, Finish };

public class ServerGameplay : MonoBehaviour
{
    //private GameManager gm;
    //private NetworkManager nm;
    //private States currentState;
    //
    //private int minutesInLobby = 15;
    //private float minutesGameplay = 30;
    //private float timeUntilCloseServer = 5;
    //private int minAmountOfPlayersToInitCounter = 2;
    //
    //private bool initLobby = true;
    //private bool initGameplay = true;
    //private bool counterInit = false;
    //
    //private float counter = 0;
    //
    //private void Start()
    //{
    //    gm = GameManager.Instance;
    //    nm = NetworkManager.Instance;
    //
    //    nm.OnReceivedMessage += OnReceivedData;
    //}
    //
    //private void OnReceivedData(byte[] data, IPEndPoint ip)
    //{
    //    // Handle incoming commands from clients
    //    if (MessageChecker.CheckMessageType(data) == MessageType.Position)
    //    {
    //        // Validate movement before broadcasting
    //        NetVector3 netPosition = new(data);
    //        int clientId = netPosition.GetData().id;
    //
    //        if (gm.playerList.ContainsKey(clientId))
    //        {
    //            // Only broadcast if valid
    //            nm.networkEntity.SendMessage(data);
    //        }
    //    }
    //    else if (MessageChecker.CheckMessageType(data) == MessageType.BulletInstatiate)
    //    {
    //        // Validate shooting before broadcasting
    //        NetVector3 netBullet = new(data);
    //        int clientId = netBullet.GetData().id;
    //
    //        if (gm.playerList.ContainsKey(clientId))
    //        {
    //            nm.networkEntity.SendMessage(data);
    //        }
    //    }
    //}
    //
    //private void Update()
    //{
    //    if (!nm.isServer) return;
    //
    //    switch (currentState)
    //    {
    //        case States.Init:
    //            if (nm.networkEntity.clients.Count >= minAmountOfPlayersToInitCounter)
    //            {
    //                currentState = States.Lobby;
    //            }
    //            break;
    //
    //        case States.Lobby:
    //            HandleLobbyState();
    //            break;
    //
    //        case States.Game:
    //            HandleGameState();
    //            break;
    //
    //        case States.Finish:
    //            HandleFinishState();
    //            break;
    //    }
    //}
    //
    //private void HandleLobbyState()
    //{
    //    if (nm.networkEntity.clients.Count >= minAmountOfPlayersToInitCounter)
    //    {
    //        if (!counterInit)
    //        {
    //            counterInit = true;
    //            counter = 0;
    //        }
    //
    //        counter += Time.deltaTime;
    //        gm.timer.text = counter.ToString("F0");
    //
    //        if (counter >= minutesInLobby)
    //        {
    //            counter = 0;
    //            gm.timer.text = "";
    //            currentState = States.Game;
    //        }
    //    }
    //    else if (counterInit)
    //    {
    //        counterInit = false;
    //        counter = 0;
    //        gm.timer.text = "";
    //        currentState = States.Init;
    //    }
    //}
    //
    //private void HandleGameState()
    //{
    //    counter += Time.deltaTime;
    //    gm.timer.text = counter.ToString("F0");
    //
    //    if (counter >= minutesGameplay || gm.playerList.Count <= 1)
    //    {
    //        SendMatchWinner();
    //        counter = 0;
    //        gm.timer.text = "";
    //        currentState = States.Finish;
    //    }
    //}
    //
    //private void HandleFinishState()
    //{
    //    timeUntilCloseServer -= Time.deltaTime;
    //    if (timeUntilCloseServer <= 0)
    //    {
    //        nm.networkEntity.CloseConnection();
    //    }
    //}
    //
    //private void SendMatchWinner()
    //{
    //    PlayerController playerWithMaxHealth = null;
    //    int maxHealth = int.MinValue;
    //
    //    foreach (var player in gm.playerList)
    //    {
    //        if (player.Value.TryGetComponent(out PlayerController pc))
    //        {
    //            if (pc.health > maxHealth)
    //            {
    //                maxHealth = pc.health;
    //                playerWithMaxHealth = pc;
    //            }
    //        }
    //    }
    //
    //    if (playerWithMaxHealth != null)
    //    {
    //        NetIDMessage netIDMessage = new(MessagePriority.Default, playerWithMaxHealth.clientID)
    //        {
    //            CurrentMessageType = MessageType.Winner
    //        };
    //        nm.networkEntity.SendMessage(netIDMessage.Serialize());
    //    }
    //}
}