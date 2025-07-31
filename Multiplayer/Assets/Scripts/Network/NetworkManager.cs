using UnityEngine;
using System.Net;
using System;
using Net;

/// <summary>
/// Manages the network connection state and provides access to network functionality.
/// Acts as a singleton bridge between game systems and network operations.
/// </summary>
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

    /// <summary>
    /// Initializes the network manager and records the startup time.
    /// </summary>
    private void Start()
    {
        appStartTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the network client instance if running as a client.
    /// </summary>
    /// <returns>The NetworkClient instance, or null if running as server.</returns>
    public NetworkClient GetNetworkClient()
    {
        if (isServer)
        {
            Instance.onInitEntity.Invoke();
            return null;
        }

        return (NetworkClient)networkEntity;
    }

    /// <summary>
    /// Starts a new network client connection.
    /// </summary>
    /// <param name="ip">The server IP address to connect to.</param>
    /// <param name="port">The server port number.</param>
    /// <param name="name">The name of this client.</param>
    public void StartClient(IPAddress ip, int port, string name)
    {
        networkEntity = new NetworkClient(ip, port, name);
        onInitEntity?.Invoke();
    }

    /// <summary>
    /// Performs per-frame updates for network operations.
    /// </summary>
    private void Update()
    {
        networkEntity?.Update();
    }

    /// <summary>
    /// Handles cleanup when the application is quitting.
    /// </summary>
    private void OnApplicationQuit()
    {
        networkEntity.OnApplicationQuit();
    }

    /// <summary>
    /// Switches to the menu screen (client-side only).
    /// </summary>
    public void SwitchToMenuScreen()
    {
        #if CLIENT
        NetworkScreen.Instance.SwitchToMenuScreen();
        #endif
    }

    /// <summary>
    /// Writes text to the chat display (client-side only).
    /// </summary>
    /// <param name="text">The text to display.</param>
    public void WriteChat(string text)
    {
        #if CLIENT
        ChatScreen.Instance.messages.text += text;
        #endif
    }

    /// <summary>
    /// Shows an error panel with the specified message (client-side only).
    /// </summary>
    /// <param name="errorText">The error message to display.</param>
    public void ShowErrorPanel(string errorText)
    {
        #if CLIENT
        NetworkScreen.Instance.ShowErrorPanel(errorText);
        #endif
    }
}