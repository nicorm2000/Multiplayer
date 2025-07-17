using UnityEngine;
using System.Net;
using System;
using Net;

enum States { Init, Lobby, Game, Finish };

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>  
{
    public NetworkEntity networkEntity;

    public Action onInitEntity;
    public Action<int, GameObject> onInstanceCreated;

    public int ClientID => networkEntity?.GetNetworkClient() ?? -1;

    public bool isServer
    {
        get { return !(networkEntity is NetworkClient); }
        private set { }
    }

    DateTime appStartTime;

    private void Start()
    {
        appStartTime = DateTime.UtcNow;
    }

    public NetworkClient GetNetworkClient()
    {
        if (isServer)
        {
            Instance.onInitEntity.Invoke();
            return null;
        }

        return (NetworkClient)networkEntity;
    }

    public void StartClient(IPAddress ip, int port, string name)
    {
        networkEntity = new NetworkClient(ip, port, name);
        onInitEntity?.Invoke();
    }

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
        #if CLIENT
        NetworkScreen.Instance.SwitchToMenuScreen();
        #endif
    }

    public void WriteChat(string text)
    {
        #if CLIENT
        ChatScreen.Instance.messages.text += text;
        #endif
    }

    public void ShowErrorPanel(string errorText)
    {
        #if CLIENT
        NetworkScreen.Instance.ShowErrorPanel(errorText);
        #endif
    }
}