using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public Action<int> OnBulletHit;

    public Action<int> OnNewPlayer;
    public Action<int> OnRemovePlayer;

    public Action<bool> OnInitLobbyTimer;
    public Action OnInitGameplayTimer;

    public Action<int> OnChangeLobbyPlayers;

    public Action<int, Vector3> OnInstantiateBullet;

    public TextMeshProUGUI timer;

    [SerializeField] private Transform[] spawnPositions;

    [SerializeField] private GameObject playerPrefab;
    public Dictionary<int, GameObject> playerList = new();

    private NetworkManager nm;
    public bool isGameplay;

    private void Start()
    {
        nm = NetworkManager.Instance;

        OnNewPlayer += SpawnPlayerPefab;
        OnRemovePlayer += RemovePlayer;
        OnInstantiateBullet += InstantiatePlayerBullets;
        OnBulletHit += OnHitRecieved;

        OnInitGameplayTimer += ActivePlayerControllers;
    }

    private void SpawnPlayerPefab(int index)
    {
        if (!playerList.ContainsKey(index))
        {
            playerList.Add(index, Instantiate(playerPrefab, spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position, Quaternion.identity));
            OnChangeLobbyPlayers?.Invoke(index);
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
                Debug.Log(index);
                pc.currentPlayer = true;
            }

            if (!nm.isServer)
            {
                pc.enabled = false;
            }
        }
    }

    private void RemovePlayer(int index)
    {
        if (playerList.ContainsKey(index))
        {
            Destroy(playerList[index]);
            playerList.Remove(index);
        }
    }

    private void InstantiatePlayerBullets(int id, Vector3 bulletDir)
    {
        playerList[id].GetComponent<PlayerController>().ServerShoot(bulletDir);
        playerList[id].GetComponent<AudioSource>().Play();
        playerList[id].GetComponent<Animator>().SetTrigger("Shoot");
    }

    public void UpdatePlayerPosition((int index, Vector3 newPosition) playerData)
    {
        playerList[playerData.index].transform.position = playerData.newPosition;
    }

    private void OnHitRecieved(int playerReciveDamage)
    {
        if (nm.isServer)
        {
            if (playerList.ContainsKey(playerReciveDamage))
            {
                playerList[playerReciveDamage].transform.GetComponent<PlayerController>().OnReciveDamage();
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
        timer.text = "";
    }
}