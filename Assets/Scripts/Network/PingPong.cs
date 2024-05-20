using System;
using System.Collections.Generic;
using UnityEngine;

public class PingPong
{
    private int timeUntilDisconnection = 5;
    private Dictionary<int, float> clientLastMessageReceived = new (); // Server use
    private float serverLastMessageReceived = 0; // Client use
    private float sendMessageCounter = 0;
    private float secondsPerCheck = 1.0f;
    private Dictionary<int, float> clientLatency = new (); // Server use
    private float serverLatency = 0;
    private DateTime currentDateTime;

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
        clientLastMessageReceived.Add(idToAdd, 0.0f);
    }

    /// <summary>
    /// Removes a client from the list for tracking received messages.
    /// </summary>
    /// <param name="idToRemove">The ID of the client to remove.</param>
    public void RemoveClientForList(int idToRemove)
    {
        clientLastMessageReceived.Remove(idToRemove);
    }

    /// <summary>
    /// Handles the reception of a ping message from the server to the client.
    /// </summary>
    public void ReceiveServerToClientPingMessage()
    {
        serverLastMessageReceived = 0;
    }

    /// <summary>
    /// Handles the reception of a ping message from a client to the server.
    /// </summary>
    /// <param name="playerID">The ID of the client sending the ping.</param>
    public void ReceiveClientToServerPingMessage(int playerID)
    {
        clientLastMessageReceived[playerID] = 0;
    }

    /// <summary>
    /// Updates the activity checks, sending ping messages and checking disconnection times.
    /// </summary>
    public void UpdateCheckActivity()
    {
        sendMessageCounter += Time.deltaTime;

        if (sendMessageCounter > secondsPerCheck) // Send a message every 1 second
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
            // Increment the time since the last message was received for each client
            var keys = new List<int>(clientLastMessageReceived.Keys);

            foreach (var key in keys)
            {
                clientLastMessageReceived[key] += Time.deltaTime;
            }
        }
        else
        {
            // Increment the time since the last message was received from the server
            serverLastMessageReceived += Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks if any clients or the server have exceeded the disconnection time limit.
    /// </summary>
    private void CheckTimeUntilDisconection()
    {
        if (NetworkManager.Instance.isServer)
        {
            // Check if any clients need to be disconnected due to inactivity
            foreach (int clientID in clientLastMessageReceived.Keys)
            {
                if (clientLastMessageReceived[clientID] > timeUntilDisconnection)
                {
                    NetworkManager.Instance.RemoveClient(clientID);
                    // Notify other clients about the disconnection
                    NetIDMessage netDisconnection = new (MessagePriority.Default, clientID);
                    NetworkManager.Instance.Broadcast(netDisconnection.Serialize());
                }
            }
        }
        else
        {
            // Check if the server needs to be disconnected due to inactivity
            if (serverLastMessageReceived > timeUntilDisconnection)
            {
                NetIDMessage netDisconnection = new (MessagePriority.Default, NetworkManager.Instance.actualClientId);
                NetworkManager.Instance.SendToServer(netDisconnection.Serialize());

                NetworkManager.Instance.DisconectPlayer();
            }
        }
    }

    /// <summary>
    /// Sends a ping message to check for connectivity.
    /// </summary>
    private void SendPingMessage()
    {
        NetPing netPing = new ();

        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(netPing.Serialize());
        }
        else
        {
            NetworkManager.Instance.SendToServer(netPing.Serialize());
        }

        // Record the current time for latency calculation
        currentDateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the latency from the server to the client.
    /// </summary>
    public void CalculateServerLatency()
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        serverLatency = (float)newDateTime.Milliseconds;
    }

    /// <summary>
    /// Calculates the latency from a specific client to the server.
    /// </summary>
    /// <param name="clientID">The ID of the client to calculate latency for.</param>
    public void CalculateClientLatency(int clientID)
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        clientLatency[clientID] = (float)newDateTime.TotalMilliseconds;
    }

    /// <summary>
    /// Gets the latency from a specific client.
    /// </summary>
    /// <param name="clientId">The ID of the client to get latency for.</param>
    /// <returns>The latency value in milliseconds, or -1 if the client ID is not found.</returns>
    public float GetClientLatency(int clientId)
    {
        if (clientLatency.ContainsKey(clientId))
        {
            return clientLatency[clientId];
        }

        return -1;
    }

    /// <summary>
    /// Gets the latency from the server.
    /// </summary>
    /// <returns>The latency value in milliseconds.</returns>
    public float GetServerLatency()
    {
        return serverLatency;
    }
}