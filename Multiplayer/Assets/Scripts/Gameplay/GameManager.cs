using Net;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public static Action<int, int> OnBulletHit;
    public Action<bool> OnInitLobbyTimer;
    public Action OnInitGameplayTimer;

    public Action<int> OnChangeLobbyPlayers;

    [SerializeField] Transform[] spawnPositions;

    [SerializeField] GameObject playerPrefab;
    public Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();

    int spawnCounter = 0;

    NetworkManager nm;
    public bool isGameplay;

    void Start()
    {
        nm = NetworkManager.Instance;

        nm.onInitEntity += InitNetworkEntityActions;
        OnBulletHit += OnHitRecieved;

        nm.onInstanceCreated += CheckForInstanceCreated;

        OnInitGameplayTimer += ActivePlayerControllers;
    }

    void InitNetworkEntityActions()
    {
        nm.networkEntity.OnNewPlayer += SpawnPlayerPefab;
        nm.networkEntity.OnRemovePlayer += RemovePlayer;
        nm.networkEntity.OnInstantiateBullet += InstantiatePlayerBullets;
    }

    void SpawnPlayerPefab(int index)
    {
        if (!playerList.ContainsKey(index))
        {
            if (index == nm.ClientID)
            {
                if (spawnCounter >= spawnPositions.Length)
                {
                    Debug.LogError("No available spawn positions!");
                    return;
                }

                IPrefabService prefabService = ServiceProvider.GetService<IPrefabService>();

                NetObjFactory.NetInstance(prefabService.GetIdByPrefab(playerPrefab),
                                          spawnPositions[spawnCounter].position.x, spawnPositions[spawnCounter].position.y, spawnPositions[spawnCounter].position.z,
                                          Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w,
                                          playerPrefab.transform.localScale.x, playerPrefab.transform.localScale.y, playerPrefab.transform.localScale.z,
                                          -1);
            }

            playerList.Add(index, null);
            OnChangeLobbyPlayers?.Invoke(index);
            spawnCounter++;
            spawnCounter %= spawnPositions.Length;
        }
    }

    void CheckForInstanceCreated(int owner, GameObject gameObject)
    {
        if (playerList.ContainsKey(owner))
        {
            if (gameObject.TryGetComponent(out PlayerController pc)) //Confirmo que el objeto instanciado sea un player
            {
                playerList[owner] = gameObject;
                Debug.Log("Se instancio el Gameobject: " + gameObject.name + " Del Owner " + owner);

                pc.clientID = owner;
                pc.currentPlayer = owner == nm.ClientID;
            }
        }
    }


    void RemovePlayer(int index)
    {
        if (playerList.ContainsKey(index))
        {
            Destroy(playerList[index]);
            playerList.Remove(index);

   
            if (!nm.isServer && index == nm.ClientID)
            {
                spawnCounter = 0;
                RemoveAllPlayers();
            }
        }
    }

    public void RemoveAllPlayers()
    {
        foreach (int id in playerList.Keys)
        {
            Destroy(playerList[id]);
        }

        playerList.Clear();
        spawnCounter = 0;
    }

    void InstantiatePlayerBullets(int id, Vec3 bulletDir)
    {
        playerList[id].GetComponent<AudioSource>().Play();
        playerList[id].GetComponent<Animator>().SetTrigger("Shoot");
    }

    public void UpdatePlayerPosition((int index, Vector3 newPosition) playerData)
    {
        if (playerList.ContainsKey(playerData.index))
        {
            playerList[playerData.index].transform.position = playerData.newPosition;
        }
    }

    void OnHitRecieved(int playerReciveDamage, int otherPlayer)
    {
        if (playerList.ContainsKey(playerReciveDamage))
        {
            playerList[playerReciveDamage].transform.GetComponent<PlayerController>().OnReceiveDamage();

            if (playerList[playerReciveDamage].transform.GetComponent<PlayerController>().health <= 0)
            {
                Debug.Log($"Player: { otherPlayer }, hit Player: {playerReciveDamage}");
                if (playerReciveDamage == nm.networkEntity.clientID)
                {
                    //DisconnectAll disconnectAll = new();
                    //NetDisconnectionMessage netDisconnectionMessage = new(disconnectAll);
                    WinnerInfo winnerInfo = new WinnerInfo(otherPlayer);
                    NetWinnerMessage netWin = new(MessagePriority.Default, winnerInfo);
                    nm.networkEntity.SendMessage(netWin.Serialize());
                }
            }
        }
    }

    public void ActivePlayerControllers()
    {
        foreach (int index in playerList.Keys)
        {
            if (playerList[index].TryGetComponent(out PlayerController pc))
            {
                pc.enabled = true;
            }
        }
    }

    public void EndMatch()
    {
        RemoveAllPlayers();
    }
}
