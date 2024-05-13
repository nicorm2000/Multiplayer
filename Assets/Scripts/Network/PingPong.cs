using System;
using System.Collections.Generic;
using UnityEngine;

public class PingPong
{
    private int timeUntilDisconnection = 5;

    private Dictionary<int, float> lastMessageReceivedFromClients = new Dictionary<int, float>(); //Server use
    float lastMessageReceivedFromServer = 0; //Client use

    private float sendMessageCounter = 0;
    private float secondsPerCheck = 1.0f;

    private DateTime currentDateTime = DateTime.UtcNow;

    public PingPong() //Calculate latency/ms
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

    public void ReciveClientToServerPingMessage(int playerID)
    {
        lastMessageReceivedFromClients[playerID] = 0;
    }

    public void UpdateCheckActivity()
    {
        sendMessageCounter += Time.deltaTime;

        if (sendMessageCounter > secondsPerCheck) //Every 1 second I send a message
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

                    NetIDMessage netDisconnection = new (clientID);
                    NetworkManager.Instance.Broadcast(netDisconnection.Serialize());
                }
            }
        }
        else
        {
            if (lastMessageReceivedFromServer > timeUntilDisconnection)
            {
                NetIDMessage netDisconnection = new (NetworkManager.Instance.actualClientId);
                NetworkManager.Instance.SendToServer(netDisconnection.Serialize());
    
                NetworkManager.Instance.DisconectPlayer();
            }
        }
    }
    
    private void SendPingMessage()
    {
        NetPing netPing = new();
    
        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(netPing.Serialize());
        }
        else
        {
            NetworkManager.Instance.SendToServer(netPing.Serialize());
        }
    }
}