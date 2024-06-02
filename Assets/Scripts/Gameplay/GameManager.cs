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

    [SerializeField] Transform[] spawnPositions;

    [SerializeField] GameObject playerPrefab;
    public Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();

    int spawnCounter = 0;

    NetworkManager nm;
    public bool isGameplay;

    /// <summary>
    /// Initializes the GameManager and sets up event listeners.
    /// </summary>
    private void Start()
    {
        nm = NetworkManager.Instance;

        OnNewPlayer += SpawnPlayerPefab;
        OnRemovePlayer += RemovePlayer;
        OnInstantiateBullet += InstantiatePlayerBullets;
        OnBulletHit += OnHitRecieved;

        OnInitGameplayTimer += ActivePlayerControllers;
    }

    /// <summary>
    /// Spawns a player prefab at a random spawn position.
    /// </summary>
    /// <param name="index">The player ID.</param>
    private void SpawnPlayerPefab(int index)
    {
        if (!playerList.ContainsKey(index))
        {
            playerList.Add(index, Instantiate(playerPrefab, spawnPositions[spawnCounter].position, Quaternion.identity));
            OnChangeLobbyPlayers?.Invoke(index);
            spawnCounter++;
        }

        if (playerList[index].TryGetComponent(out PlayerController pc))
        {
            pc.clientID = index;

            if (index != nm.ClientID)
            {
                pc.currentPlayer = false;
            }
            else
            {
                pc.currentPlayer = true;
            }

            if (!nm.isServer)
            {
                pc.enabled = false;
            }
        }
    }

    /// <summary>
    /// Removes a player from the game.
    /// </summary>
    /// <param name="index">The player ID.</param>
    private void RemovePlayer(int index)
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
    /// Removes all players from the game.
    /// </summary>
    public void RemoveAllPlayers()
    {
        foreach (int id in playerList.Keys)
        {
            Destroy(playerList[id]);
        }

        playerList.Clear();
    }

    /// <summary>
    /// Instantiates bullets for a player and plays the shoot animation and sound.
    /// </summary>
    /// <param name="id">The player ID.</param>
    /// <param name="bulletDir">The direction of the bullet.</param>
    private void InstantiatePlayerBullets(int id, Vector3 bulletDir)
    {
        playerList[id].GetComponent<PlayerController>().ServerShoot(bulletDir);
        playerList[id].GetComponent<AudioSource>().Play();
        playerList[id].GetComponent<Animator>().SetTrigger("Shoot");
    }

    /// <summary>
    /// Updates the position of a player.
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
    /// Handles receiving a hit on a player.
    /// </summary>
    /// <param name="playerReciveDamage">The player ID receiving damage.</param>
    private void OnHitRecieved(int playerReciveDamage)
    {
        if (nm.isServer)
        {
            if (playerList.ContainsKey(playerReciveDamage))
            {
                playerList[playerReciveDamage].transform.GetComponent<PlayerController>().OnReceiveDamage();
            }
        }
    }

    /// <summary>
    /// Activates player controllers for all players.
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
    /// Ends the match and clears the timer text.
    /// </summary>
    public void EndMatch()
    {
        timer.text = "";
        RemoveAllPlayers();
    }
}