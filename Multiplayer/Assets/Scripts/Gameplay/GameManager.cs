using System.Collections.Generic;
using NetworkServer;
using UnityEngine;
using System;
using Net;

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

    public void SpawnPlayerPefab(int index)
    {
        if (!playerList.ContainsKey(index))
        {
            Debug.Log("Entered spawn player prefab");
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
                ((Server)nm.networkEntity).OnReflectionMsg += ReflectionSystem.Instance.reflection.OnReceivedReflectionMessage;
                isFirstTime = !isFirstTime;
            }
            IPrefabService prefabService = ServiceProvider.GetService<IPrefabService>();

            NetObjFactory.NetInstance(prefabService.GetIdByPrefab(playerPrefab),
                                          spawnPositions[spawnCounter].position.x, spawnPositions[spawnCounter].position.y, spawnPositions[spawnCounter].position.z,
                                          Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w,
                                          playerPrefab.transform.localScale.x, playerPrefab.transform.localScale.y, playerPrefab.transform.localScale.z,
                                          -1, index);

            InstancePayload instancePayload = new InstancePayload(NetObjFactory.NetObjectsCount, index, prefabService.GetIdByPrefab(playerPrefab),
                                              spawnPositions[spawnCounter].position.x, spawnPositions[spawnCounter].position.y, spawnPositions[spawnCounter].position.z,
                                              Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w,
                                              playerPrefab.transform.localScale.x, playerPrefab.transform.localScale.y, playerPrefab.transform.localScale.z,
                                              -1);

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

    void CheckForInstanceCreated(int owner, GameObject gameObject)
    {
        foreach (KeyValuePair<int, GameObject> item in playerList)
        {
            Debug.Log(item.Key);
        }
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
            }
        }
#endif
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
