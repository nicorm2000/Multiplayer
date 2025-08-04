using NetworkServer;
using UnityEngine;
using System;
using Net;

/// <summary>
/// Manages the server lifecycle and network operations for the application.
/// </summary>
public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }

    public Server server { get; private set; }
    public bool isServerRunning = false;
    public GameManager gm;
    private DateTime appStartTime;

    /// <summary>
    /// Initializes the singleton instance and ensures persistence across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Starts the server with command line arguments for port configuration.
    /// </summary>
    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();

        int port = 52002;
        foreach (string arg in args)
        {
            if (int.TryParse(arg, out int parsedPort))
            {
                port = parsedPort;
            }
        }
        StartServer(port);

#if SERVER
        server.OnPlayerID += gm.SpawnPlayerPefab;
        NetObjFactory.OnDataSend += server.HandleInstanceRequest;
        NetworkManager.Instance.networkEntity = server;
        server.OnApplicationClose += Application.Quit;
#endif
    }

    /// <summary>
    /// Cleans up event subscriptions when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
#if SERVER
        server.OnPlayerID = null;
        gm.OnPlayerInstanceCreated = null;
#endif
    }

    /// <summary>
    /// Starts the server on the specified port.
    /// </summary>
    /// <param name="port">The port number to start the server on.</param>
    public void StartServer(int port)
    {
        if (isServerRunning) return;

        appStartTime = DateTime.UtcNow;
        server = new Server(port, appStartTime);
        isServerRunning = true;

        Debug.Log($"Server started on port {port}");
    }

    /// <summary>
    /// Stops the server if it is currently running.
    /// </summary>
    public void StopServer()
    {
        if (!isServerRunning) return;

        server.CloseConnection();
        isServerRunning = false;

        Debug.Log("Server stopped");
    }

    /// <summary>
    /// Updates the server state during each physics frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (isServerRunning)
        {
            server.Update();
        }
    }

    /// <summary>
    /// Handles cleanup when the application is quitting.
    /// </summary>
    private void OnApplicationQuit()
    {
        if (isServerRunning)
        {
            Debug.Log("Server Closing");
            server.OnApplicationQuit();
        }
    }
}