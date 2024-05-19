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

    [Header("Config")]
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private GameObject playerPrefab;
    public Dictionary<int, GameObject> playerList = new();

    private NetworkManager nm;
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
        }
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
        playerList[playerData.index].transform.position = playerData.newPosition;
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
    }
}