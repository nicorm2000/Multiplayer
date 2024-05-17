using System;
using System.Collections.Generic;
using UnityEngine;

public class PingPong
{
    private int timeUntilDisconnection = 5;

    private Dictionary<int, float> lastMessageReceivedFromClients = new Dictionary<int, float>(); // Server use
    private float lastMessageReceivedFromServer = 0; // Client use

    private float sendMessageCounter = 0;
    private float secondsPerCheck = 1.0f;


    private Dictionary<int, float> latencyFromClients = new (); // Server use
    private float latencyFromServer = 0;
    DateTime currentDateTime;

    public PingPong()
    {

    }

    public void AddClientForList(int idToAdd)
    {
        lastMessageReceivedFromClients.Add(idToAdd, 0.0f);
    }

    public void RemoveClientForList(int idToRemove)
    {
        lastMessageReceivedFromClients.Remove(idToRemove);
    }

    public void ReciveServerToClientPingMessage()
    {
        lastMessageReceivedFromServer = 0;
    }

    public void RecieveClientToServerPingMessage(int playerID)
    {
        lastMessageReceivedFromClients[playerID] = 0;
    }

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

    private void CheckTimeUntilDisconection()
    {
        if (NetworkManager.Instance.isServer)
        {
            foreach (int clientID in lastMessageReceivedFromClients.Keys)
            {
                if (lastMessageReceivedFromClients[clientID] > timeUntilDisconnection)
                {
                    NetworkManager.Instance.RemoveClient(clientID);

                    NetIDMessage netDisconnection = new (MessagePriority.Default, clientID);
                    NetworkManager.Instance.Broadcast(netDisconnection.Serialize());
                }
            }
        }
        else
        {
            if (lastMessageReceivedFromServer > timeUntilDisconnection)
            {
                NetIDMessage netDisconnection = new (MessagePriority.Default, NetworkManager.Instance.actualClientId);
                NetworkManager.Instance.SendToServer(netDisconnection.Serialize());

                NetworkManager.Instance.DisconectPlayer();
            }
        }
    }

    private void SendPingMessage()
    {
        NetPing netPing = new NetPing();

        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(netPing.Serialize());
        }
        else
        {
            NetworkManager.Instance.SendToServer(netPing.Serialize());
        }

        currentDateTime = DateTime.UtcNow;
    }

    public void CalculateLatencyFromServer()
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        latencyFromServer = (float)newDateTime.Milliseconds;
    }

    public void CalculateLatencyFromClients(int clientID)
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        latencyFromClients[clientID] = (float)newDateTime.TotalMilliseconds;
    }

    public float GetLatencyFormClient(int clientId)
    {
        if (latencyFromClients.ContainsKey(clientId))
        {
            return latencyFromClients[clientId];
        }

        return -1;
    }
    public float GetLatencyFormServer()
    {
        return latencyFromServer;
    }
}