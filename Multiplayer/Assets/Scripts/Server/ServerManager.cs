using NetworkServer;
using UnityEngine;
using System;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }

    public Server server { get; private set; }
    public bool isServerRunning = false;

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
            Debug.Log($"arg is: {arg}");
        }
        if (args.Length > 0 && int.TryParse(args[0], out int parsedPort))
        {
            port = parsedPort;
        }

        StartServer(port);
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

    private void Update()
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