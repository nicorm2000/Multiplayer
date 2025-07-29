using NetworkServer;
using Network_Lib;
using UnityEngine;
using System;
using Net;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }

    public Server server { get; private set; }
    public bool isServerRunning = false;
    public GameManager gm;
    private DateTime appStartTime;

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

    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();

        int port = 52002;
        foreach (var arg in args)
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

    private void OnDestroy()
    {
#if SERVER
        server.OnPlayerID = null;
        gm.OnPlayerInstanceCreated = null;
#endif
    }

    public void StartServer(int port)
    {
        if (isServerRunning) return;

        appStartTime = DateTime.UtcNow;
        server = new Server(port, appStartTime);
        isServerRunning = true;

        Debug.Log($"Server started on port {port}");
    }

    public void StopServer()
    {
        if (!isServerRunning) return;

        server.CloseConnection();
        isServerRunning = false;

        Debug.Log("Server stopped");
    }

    private void FixedUpdate()
    {
        if (isServerRunning)
        {
            server.Update();
        }
    }

    private void OnApplicationQuit()
    {
        if (isServerRunning)
        {
            server.OnApplicationQuit();
        }
    }
}