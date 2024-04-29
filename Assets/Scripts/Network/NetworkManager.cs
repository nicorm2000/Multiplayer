using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public struct Client
{
    public float timeStamp;
    public int id;
    public IPEndPoint ipEndPoint;

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
    }
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    public IPAddress ipAddress
    {
        get; private set;
    }

    public int port
    {
        get; private set;
    }

    public bool isServer
    {
        get; private set;
    }

    public int TimeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;

    private UdpConnection connection;

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

    public int serverClientID = 0; // ID that the server assigns to the clients that enter
    int actualClientID = 0; // The ID of the current client, NOT server

    private MessageChecker messageChecker;

    private void Start()
    {
        messageChecker = new MessageChecker();
    }

    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port)
    {
        isServer = false;

        this.port = port;
        this.ipAddress = ip;

        connection = new UdpConnection(ip, port, this);

        NetHandShake netHandShake = new NetHandShake((UdpConnection.IPToLong(ip), port));
        SendToServer(netHandShake.Serialize());
        //AddClient(new IPEndPoint(ip, port));
    }

    void AddClient(IPEndPoint ip, int newClientID)
    {
        if (!ipToId.ContainsKey(ip) && !clients.ContainsKey(newClientID))
        {
            Debug.Log("Adding client: " + ip.Address);

            ipToId[ip] = newClientID;

            clients.Add(serverClientID, new Client(ip, newClientID, Time.realtimeSinceStartup));

            if (isServer)
            {
                //Send message to notify the other players that a new one has joined
            }
        }
        else
        {
            Debug.Log("This client already exists!");
        }
    }

    void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        switch (messageChecker.CheckMessageType(data))
        {
            case MessageType.CheckActivity:
                break;
            case MessageType.SetClientID:
                NetSetClientID netGetClientID = new NetSetClientID(data);
                actualClientID = netGetClientID.GetData();
                AddClient(ip, actualClientID);
                Debug.Log("Client's number: " + actualClientID);
                break;
            case MessageType.HandShake:
                ConnectToServer(data, ip);
                break;
            case MessageType.Console:
                UpdateChatText(data);
                break;
            case MessageType.Position:
                break;
            case MessageType.Disconnection:
                break;
            default:
                break;
        }

        if (OnReceiveEvent != null)
            OnReceiveEvent.Invoke(data, ip);
    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    public void Broadcast(byte[] data, IPEndPoint ip)
    {
        connection.Send(data, ip);
    }

    public void Broadcast(byte[] data)//Dar rt a los mensajes
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                connection.Send(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }

    void ConnectToServer(byte[]data, IPEndPoint ip)
    {
        NetHandShake netHandShake = new NetHandShake(data);

        if (!clients.ContainsKey(serverClientID))
        {
            //Assigns an ID to a client and then broadcasts it
            NetSetClientID netSetClientID = new NetSetClientID(data);
            Broadcast(netSetClientID.Serialize(), ip);

            AddClient(ip, serverClientID);
            serverClientID++;
        }
    }

    private void UpdateChatText(byte[] data)
    {
        int netMessageSum = 0;
        int sum = 0;
        char[] aux;
        string text = "";

        NetConsole.Deserialize(data, out aux, out netMessageSum);

        for (int i = 0; i < aux.Length; i++)
        {
            sum += aux[i];
        }

        Debug.Log(sum);

        if (sum != netMessageSum)
        {
            //Ask the message again
            Debug.Log("The message got corrupt");
            return;
        }

        if (isServer)
        {
            Broadcast(data);
        }

        for (int i = 0; i < aux.Length; i++)
        {
            text += aux[i];
        }

        Debug.Log("Message is:" + text);

        ChatScreen.Instance.messages.text += text + System.Environment.NewLine;
    }
}