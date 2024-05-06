using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public Action<int> OnBulletHit;

    public Action<int> OnNewPlayer;
    public Action<int> OnRemovePlayer;

    public Action<int, Vector3> OnInstantiateBullet;

    public TextMeshProUGUI timer;

    [SerializeField] Transform[] spawnPositions;

    [SerializeField] GameObject playerPrefab;
    Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();

    NetworkManager nm;
    void Start()
    {
        nm = NetworkManager.Instance;

        OnNewPlayer += SpawnPlayerPefab;
        OnRemovePlayer += RemovePlayer;
        OnInstantiateBullet += InstantiatePlayerBullets;
        OnBulletHit += OnHitRecieved;
    }

    void SpawnPlayerPefab(int index)
    {
        if (!playerList.ContainsKey(index))
        {
            playerList.Add(index, Instantiate(playerPrefab, spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position, Quaternion.identity));
        }

        if (playerList[index].TryGetComponent(out PlayerController pc))
        {
            pc.clientID = index;

            if (index != nm.actualClientId)
            {
                pc.currentPlayer = false;
            }
            else
            {
                pc.currentPlayer = true;
            }
        }
    }

    void RemovePlayer(int index)
    {
        Destroy(playerList[index]);
        playerList.Remove(index);
    }

    void InstantiatePlayerBullets(int id, Vector3 bulletDir)
    {
        playerList[id].GetComponent<PlayerController>().ServerShoot(bulletDir);
    }

    public void UpdatePlayerPosition((int index, Vector3 newPosition) playerData)
    {
        playerList[playerData.index].transform.position = playerData.newPosition;
    }

    void OnHitRecieved(int playerReciveDamage)
    {
        if (nm.isServer)
        {
            if (playerList.ContainsKey(playerReciveDamage))
            {
                playerList[playerReciveDamage].transform.GetComponent<PlayerController>().OnReciveDamage();
            }
        }
    }
}