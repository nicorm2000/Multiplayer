using System.Collections.Generic;
using NetworkServer;
using UnityEngine;
using System;
using Net;

/// <summary>
/// Manages core game logic, player spawning, and network synchronization.
/// Handles player instances, game state, and match progression.
/// </summary>
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public static Action<int, int> OnBulletHit;
    public Action<bool> OnInitLobbyTimer;
    public Action OnInitGameplayTimer;
    public Action<byte[], int> OnPlayerInstanceCreated;
    public Action<int> OnChangeLobbyPlayers;

    [SerializeField] Transform[] spawnPositions;

    [SerializeField] GameObject playerPrefab;
    public Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();

    int spawnCounter = 0;
    bool isFirstTime = true;
    NetworkManager nm;
    public bool isGameplay;

    /// <summary>
    /// Initializes the GameManager and sets up event subscriptions.
    /// </summary>
    void Start()
    {
        nm = NetworkManager.Instance;

        nm.onInitEntity += InitNetworkEntityActions;
        OnBulletHit += OnHitRecieved;

        nm.onInstanceCreated += CheckForInstanceCreated;

        OnInitGameplayTimer += ActivePlayerControllers;
    }

    /// <summary>
    /// Initializes network-related actions and event handlers.
    /// </summary>
    void InitNetworkEntityActions()
    {
        nm.networkEntity.OnNewPlayer += SpawnPlayerPefab;
        nm.networkEntity.OnRemovePlayer += RemovePlayer;
        nm.networkEntity.OnInstantiateBullet += InstantiatePlayerBullets;
    }

    /// <summary>
    /// Spawns a player prefab at the next available spawn position.
    /// </summary>
    /// <param name="index">The ID of the player to spawn.</param>
    public void SpawnPlayerPefab(int index)
    {
        if (!playerList.ContainsKey(index))
        {
            //Debug.Log("Entered spawn player prefab");
            if (index == nm.ClientID)
            {
                if (spawnCounter >= spawnPositions.Length)
                {
                    Debug.LogError("No available spawn positions!");
                    return;
                }
            }

#if SERVER
            if (isFirstTime)
            {
                NetworkManager.Instance.onInitEntity.Invoke();
                ((Server)nm.networkEntity).OnReflectionMsg += ReflectionSystem.Instance.reflection.reflectionMessageHandler.OnReceivedReflectionMessage;
                isFirstTime = !isFirstTime;
            }
            IPrefabService prefabService = ServiceProvider.GetService<IPrefabService>();

            InstancePayload instancePayload = new InstancePayload(NetObjFactory.NetObjectsCount, index, prefabService.GetIdByPrefab(playerPrefab),
                                              spawnPositions[spawnCounter].position.x, spawnPositions[spawnCounter].position.y, spawnPositions[spawnCounter].position.z,
                                              Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w,
                                              playerPrefab.transform.localScale.x, playerPrefab.transform.localScale.y, playerPrefab.transform.localScale.z,
                                              -1);

            NetObjFactory.NetInstance(prefabService.GetIdByPrefab(playerPrefab),
                                          spawnPositions[spawnCounter].position.x, spawnPositions[spawnCounter].position.y, spawnPositions[spawnCounter].position.z,
                                          Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w,
                                          playerPrefab.transform.localScale.x, playerPrefab.transform.localScale.y, playerPrefab.transform.localScale.z,
                                          -1, index);

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

                if (instance.TryGetComponent(out PlayerController pc)) //Confirmo que el objeto instanciado sea un player
                {
                    playerList[obj.GetOwnerID()] = instance;

                    pc.clientID = obj.GetOwnerID();
                    pc.currentPlayer = obj.GetOwnerID() == nm.ClientID;
                }
            }
            playerList.TryAdd(index, instance);
#endif
#if CLIENT
            playerList.TryAdd(index, null);
#endif
            OnChangeLobbyPlayers?.Invoke(index);
            spawnCounter++;
            spawnCounter %= spawnPositions.Length;
        }
    }

    /// <summary>
    /// Handles the creation of a network-synchronized game object instance.
    /// </summary>
    /// <param name="owner">The owner ID of the instance.</param>
    /// <param name="gameObject">The game object that was created.</param>
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

    /// <summary>
    /// Removes a player from the game by their ID.
    /// </summary>
    /// <param name="index">The ID of the player to remove.</param>
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

    /// <summary>
    /// Removes all players from the game and resets spawn positions.
    /// </summary>
    public void RemoveAllPlayers()
    {
        foreach (int id in playerList.Keys)
        {
            Destroy(playerList[id]);
        }

        playerList.Clear();
        spawnCounter = 0;
    }

    /// <summary>
    /// Instantiates bullet effects for a player's shooting action.
    /// </summary>
    /// <param name="id">The ID of the shooting player.</param>
    /// <param name="bulletDir">The direction of the bullet.</param>
    void InstantiatePlayerBullets(int id, Vec3 bulletDir)
    {
        playerList[id].GetComponent<AudioSource>().Play();
        playerList[id].GetComponent<Animator>().SetTrigger("Shoot");
    }

    /// <summary>
    /// Updates a player's position in the game world.
    /// </summary>
    /// <param name="playerData">Tuple containing player ID and new position.</param>
    public void UpdatePlayerPosition((int index, Vector3 newPosition) playerData)
    {
        if (playerList.ContainsKey(playerData.index))
        {
            playerList[playerData.index].transform.position = playerData.newPosition;
        }
    }

    /// <summary>
    /// Handles damage received by a player from another player.
    /// </summary>
    /// <param name="playerReciveDamage">ID of the player receiving damage.</param>
    /// <param name="otherPlayer">ID of the player causing damage.</param>
    void OnHitRecieved(int playerReciveDamage, int otherPlayer)
    {
#if SERVER
        if (playerList.ContainsKey(playerReciveDamage))
        {
            playerList[playerReciveDamage].transform.GetComponent<PlayerController>().OnReceiveDamage();

            if (playerList[playerReciveDamage].transform.GetComponent<PlayerController>().health <= 0)
            {
                Debug.Log($"Player: { otherPlayer }, hit Player: {playerReciveDamage}");
                Debug.Log("Send Win msg");
                WinnerInfo winnerInfo = new WinnerInfo(otherPlayer);
                NetWinnerMessage netWin = new(MessagePriority.Default, winnerInfo);
                nm.networkEntity.SendMessage(netWin.Serialize());

                DisconnectAll disconnectAll = new();
                NetDisconnectionMessage netDisconnectionMessage = new(disconnectAll);
                nm.networkEntity.SendMessage(netDisconnectionMessage.Serialize());
            }
        }
#endif
    }

    /// <summary>
    /// Activates all player controllers when gameplay begins.
    /// </summary>
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

    /// <summary>
    /// Cleans up and resets the game state when a match ends.
    /// </summary>
    public void EndMatch()
    {
        RemoveAllPlayers();
    }
}
