using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Handles ping-pong communication for the server to monitor client connections.
    /// </summary>
    public class ServerPingPong : PingPong
    {
        private Dictionary<int, float> lastMessageReceivedFromClients = new Dictionary<int, float>();
        private Dictionary<int, float> latencyFromClients = new Dictionary<int, float>();

        /// <summary>
        /// Initializes a new instance of the ServerPingPong class.
        /// </summary>
        /// <param name="networkEntity">The network entity to associate with.</param>
        public ServerPingPong(NetworkEntity networkEntity) : base(networkEntity) { }

        /// <summary>
        /// Adds a client to the ping-pong monitoring system.
        /// </summary>
        /// <param name="idToAdd">The client ID to add.</param>
        public void AddClientForList(int idToAdd)
        {
            lastMessageReceivedFromClients.Add(idToAdd, 0.0f);
        }

        /// <summary>
        /// Removes a client from the ping-pong monitoring system.
        /// </summary>
        /// <param name="idToRemove">The client ID to remove.</param>
        public void RemoveClientForList(int idToRemove)
        {
            lastMessageReceivedFromClients.Remove(idToRemove);
        }

        /// <summary>
        /// Removes a client from the ping-pong monitoring system.
        /// </summary>
        /// <param name="idToRemove">The client ID to remove.</param>
        public void ReciveClientToServerPingMessage(int playerID)
        {
            lastMessageReceivedFromClients[playerID] = 0;
        }

        /// <summary>
        /// Checks the activity counter for all clients.
        /// </summary>
        /// <param name="deltaTime">The time since last check.</param>
        protected override void CheckActivityCounter(float deltaTime)
        {
            var keys = new List<int>(lastMessageReceivedFromClients.Keys);

            foreach (var key in keys)
            {
                lastMessageReceivedFromClients[key] += deltaTime;
            }
        }

        /// <summary>
        /// Checks for inactive clients and disconnects them.
        /// </summary>
        protected override void CheckTimeUntilDisconection()
        {
            foreach (int clientID in lastMessageReceivedFromClients.Keys)
            {
                if (lastMessageReceivedFromClients[clientID] > timeUntilDisconnection)
                {
                    networkEntity.RemoveClient(clientID);

                    NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
                    networkEntity.SendMessage(netDisconnection.Serialize());
                }
            }
        }

        /// <summary>
        /// Sends a ping message to all clients.
        /// </summary>
        protected override void SendPingMessage()
        {
            NetPing netPing = new NetPing();
            networkEntity.SendMessage(netPing.Serialize());
            currentDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Calculates the latency from a specific client.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        public void CalculateLatencyFromClients(int clientID)
        {
            TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
            latencyFromClients[clientID] = (float)(newDateTime.TotalMilliseconds / 1000);
        }

        /// <summary>
        /// Gets the latency from a specific client.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <returns>The latency in seconds, or -1 if not found.</returns>
        public float GetLatencyFormClient(int clientId)
        {
            if (latencyFromClients.ContainsKey(clientId))
            {
                return latencyFromClients[clientId];
            }

            return -1;
        }
    }
}