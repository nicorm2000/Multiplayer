using System.Net;
using System;

namespace Net
{
    /// <summary>
    /// Abstract base class for network entities that can send and receive data.
    /// </summary>
    public abstract class NetworkEntity : IReceiveData
    {
        public int port
        {
            get; protected set;
        }

        public Action onInitPingPong;
        public Action<int, Vec3> OnInstantiateBullet;
        public Action<int> OnNewPlayer;
        public Action<int> OnRemovePlayer;

        protected UdpConnection connection;
        public Action<byte[], IPEndPoint> OnReceivedMessage;

        public string userName = "Server";
        public int clientID = 0;

        public PingPong checkActivity;

        /// <summary>
        /// Initializes a new instance of the NetworkEntity class.
        /// </summary>
        public NetworkEntity()
        {
            NetObjFactory.SetNetworkEntity(this);
        }

        /// <summary>
        /// Adds a new client to the network entity.
        /// </summary>
        /// <param name="ip">The client's IP endpoint.</param>
        /// <param name="newClientID">The client's ID.</param>
        /// <param name="clientName">The client's name.</param>
        public abstract void AddClient(IPEndPoint ip, int newClientID, string clientName);

        /// <summary>
        /// Removes a client from the network entity.
        /// </summary>
        /// <param name="idToRemove">The ID of the client to remove.</param>
        public abstract void RemoveClient(int idToRemove);

        /// <summary>
        /// Closes the network connection.
        /// </summary>
        public abstract void CloseConnection();

        /// <summary>
        /// Handles received network data.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <param name="ipEndpoint">The sender's IP endpoint.</param>
        public abstract void OnReceiveData(byte[] data, IPEndPoint ipEndpoint);

        /// <summary>
        /// Sends a message to a specific client.
        /// </summary>
        /// <param name="data">The message data.</param>
        /// <param name="id">The client ID.</param>
        public abstract void SendMessage(byte[] data, int id);

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="data">The message data.</param>
        public abstract void SendMessage(byte[] data);

        /// <summary>
        /// Updates the network entity's state.
        /// </summary>
        public virtual void Update()
        {
            if (connection != null)
            {
                connection.FlushReceiveData();
                checkActivity?.UpdateCheckActivity();
            }
        }

        /// <summary>
        /// Updates the chat text with received message data.
        /// </summary>
        /// <param name="data">The message data.</param>
        protected abstract void UpdateChatText(byte[] data);

        /// <summary>
        /// Updates a player's position based on received data.
        /// </summary>
        /// <param name="data">The position data.</param>
        protected abstract void UpdatePlayerPosition(byte[] data);

        /// <summary>
        /// Handles cleanup when the application is about to quit.
        /// </summary>
        public abstract void OnApplicationQuit();

        /// <summary>
        /// Gets the network client ID.
        /// </summary>
        /// <returns>The client ID.</returns>
        public virtual int GetNetworkClient()
        {
            return clientID;
        }
    }
}