using System;
using System.Collections.Generic;
using UnityEngine;

public class PingPong
{
    int timeUntilDisconnection = 5;

    private Dictionary<int, float> lastMessageReceivedFromClients = new Dictionary<int, float>(); //Lo usa el Server
    float lastMessageReceivedFromServer = 0; //Lo usan los clientes

    float sendMessageCounter = 0;
    float secondsPerCheck = 1.0f;

    private Dictionary<int, float> latencyFromClients = new Dictionary<int, float>(); //Lo usa el Server
    float latencyFromServer = 0;
    DateTime currentDateTime;

    /// <summary>
    /// Initializes a new instance of the PingPong class.
    /// </summary>
    public PingPong()
    {
    }

    /// <summary>
    /// Adds a client to the list for tracking received messages.
    /// </summary>
    /// <param name="idToAdd">The ID of the client to add.</param>
    public void AddClientForList(int idToAdd)
    {
        lastMessageReceivedFromClients.Add(idToAdd, 0.0f);
    }

    /// <summary>
    /// Removes a client from the list for tracking received messages.
    /// </summary>
    /// <param name="idToRemove">The ID of the client to remove.</param>
    public void RemoveClientForList(int idToRemove)
    {
        lastMessageReceivedFromClients.Remove(idToRemove);
    }

    /// <summary>
    /// Handles the reception of a ping message from the server to the client.
    /// </summary>
    public void ReceiveServerToClientPingMessage()
    {
        lastMessageReceivedFromServer = 0;
    }

    /// <summary>
    /// Handles the reception of a ping message from a client to the server.
    /// </summary>
    /// <param name="playerID">The ID of the client sending the ping.</param>
    public void ReceiveClientToServerPingMessage(int playerID)
    {
        lastMessageReceivedFromClients[playerID] = 0;
    }


    /// <summary>
    /// Updates the activity checks, sending ping messages and checking disconnection times.
    /// </summary>
    public void UpdateCheckActivity()
    {
        sendMessageCounter += Time.deltaTime;

        if (sendMessageCounter > secondsPerCheck) //Envio cada 1 segundo el mensaje
        {
            SendPingMessage();
            sendMessageCounter = 0;
        }

        CheckActivityCounter();
        CheckTimeUntilDisconection();
    }

    /// <summary>
    /// Increments the activity counter for each client or the server.
    /// </summary>
    private void CheckActivityCounter()
    {
        if (NetworkManager.Instance.isServer)
        {
            var keys = new List<int>(lastMessageReceivedFromClients.Keys);

            foreach (var key in keys)
            {
                lastMessageReceivedFromClients[key] += Time.deltaTime;
            }
        }
        else
        {
            lastMessageReceivedFromServer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks if any clients or the server have exceeded the disconnection time limit.
    /// </summary>
    private void CheckTimeUntilDisconection()
    {
        if (NetworkManager.Instance.isServer)
        {
            foreach (int clientID in lastMessageReceivedFromClients.Keys)
            {
                if (lastMessageReceivedFromClients[clientID] > timeUntilDisconnection)
                {
                    NetworkManager.Instance.networkEntity.RemoveClient(clientID);

                    NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
                    NetworkManager.Instance.GetNetworkServer().Broadcast(netDisconnection.Serialize());
                }
            }
        }
        else
        {
            if (lastMessageReceivedFromServer > timeUntilDisconnection)
            {
                NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, NetworkManager.Instance.ClientID);
                NetworkManager.Instance.GetNetworkClient().SendToServer(netDisconnection.Serialize());

                NetworkManager.Instance.GetNetworkClient().DisconectPlayer();
            }
        }
    }

    /// <summary>
    /// Sends a ping message to check for connectivity.
    /// </summary>
    private void SendPingMessage()
    {
        NetPing netPing = new NetPing();

        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.GetNetworkServer().Broadcast(netPing.Serialize());
        }
        else
        {
            NetworkManager.Instance.GetNetworkClient().SendToServer(netPing.Serialize());
        }

        currentDateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the latency from the server to the client.
    /// </summary>
    public void CalculateServerLatency()
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        latencyFromServer = (float)newDateTime.Milliseconds;
        //Debug.Log("Latency from Server " + latencyFromServer / 1000);
    }

    /// <summary>
    /// Calculates the latency from a specific client to the server.
    /// </summary>
    /// <param name="clientID">The ID of the client to calculate latency for.</param>
    public void CalculateClientLatency(int clientID)
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        latencyFromClients[clientID] = (float)newDateTime.TotalMilliseconds;
        //Debug.Log("Latency from client " + clientID + " - " + latencyFromClients[clientID] /1000);
    }

    /// <summary>
    /// Gets the latency from a specific client.
    /// </summary>
    /// <param name="clientId">The ID of the client to get latency for.</param>
    /// <returns>The latency value in milliseconds, or -1 if the client ID is not found.</returns>
    public float GetClientLatency(int clientId)
    {
        if (latencyFromClients.ContainsKey(clientId))
        {
            return latencyFromClients[clientId];
        }

        return -1;
    }

    /// <summary>
    /// Gets the latency from the server.
    /// </summary>
    /// <returns>The latency value in milliseconds.</returns>
    public float GetServerLatency()
    {
        return latencyFromServer;
    }
}