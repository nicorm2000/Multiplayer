using UnityEngine;
using System.Net;
using System;
using Net;
using NetworkServer;

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>  
{
    public NetworkEntity networkEntity;
    public bool isServerMode = false;

    public Action onInitEntity;
    public Action<int, GameObject> onInstanceCreated;

    public int ClientID => networkEntity?.clientID ?? -1;

    //remove
    public bool isServer
    {
        get { return !(networkEntity is NetworkClient); }
        private set { }
    }

    //public bool isServer => isServerMode || (networkEntity is Server);

    DateTime appStartTime;

    private void Start()
    {
        appStartTime = DateTime.UtcNow;
    }

    public NetworkClient GetNetworkClient()
    {
        if (isServer)
        {
            return null;
        }

        return (NetworkClient)networkEntity;
    }

    //public void StartServer(int port)
    //{
    //    if (networkEntity != null) return;
    //
    //    isServerMode = true;
    //    networkEntity = ServerManager.Instance.server;
    //    onInitEntity?.Invoke();
    //}

    public void StartClient(IPAddress ip, int port, string name)
    {
        networkEntity = new NetworkClient(ip, port, name);
        onInitEntity?.Invoke();
    }

    //public void StartClient(IPAddress ip, int port, string name)
    //{
    //    if (networkEntity != null) return;
    //
    //    isServerMode = false;
    //    networkEntity = new NetworkClient(ip, port, name);
    //    onInitEntity?.Invoke();
    //}

    private void Update()
    {
        networkEntity?.Update();
    }

    private void OnApplicationQuit()
    {
        networkEntity.OnApplicationQuit();
    }

    public void SwitchToMenuScreen()
    {
        NetworkScreen.Instance.SwitchToMenuScreen();
    }

    public void WriteChat(string text)
    {
        ChatScreen.Instance.messages.text += text;
    }

    public void ShowErrorPanel(string errorText)
    {
        NetworkScreen.Instance.ShowErrorPanel(errorText);
    }

    public void UpdatePlayerPosition((int index, Vector3 newPosition) data, NetVector3 netVector3)
    {
        GameManager.Instance.UpdatePlayerPosition(data);
    }
}