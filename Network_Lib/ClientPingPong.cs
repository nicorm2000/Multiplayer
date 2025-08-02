using System;

namespace Net
{
    public class ClientPingPong : PingPong
    {
        private float lastMessageReceivedFromServer = 0;
        private float latencyFromServer = 0;

        /// <summary>
        /// Initializes a new instance of the ClientPingPong class with the specified network entity.
        /// </summary>
        /// <param name="networkEntity">The network entity associated with this ping handler.</param>
        public ClientPingPong(NetworkEntity networkEntity) : base(networkEntity) { }

        /// <summary>
        /// Records the receipt of a ping message from the server.
        /// </summary>
        public void ReciveServerToClientPingMessage()
        {
            lastMessageReceivedFromServer = 0;
        }

        /// <summary>
        /// Checks the activity counter to detect timeouts.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last check.</param>
        protected override void CheckActivityCounter(float deltaTime)
        {
            lastMessageReceivedFromServer += deltaTime;
        }

        /// <summary>
        /// Checks if the connection should be terminated due to inactivity.
        /// </summary>
        protected override void CheckTimeUntilDisconection()
        {
            if (lastMessageReceivedFromServer > timeUntilDisconnection)
            {
                NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, networkEntity.clientID);
                networkEntity.SendMessage(netDisconnection.Serialize());
                networkEntity.CloseConnection();
            }
        }

        /// <summary>
        /// Sends a ping message to the server.
        /// </summary>
        protected override void SendPingMessage()
        {
            NetPing netPing = new NetPing();
            networkEntity.SendMessage(netPing.Serialize());
            currentDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Calculates the latency from the server based on ping times.
        /// </summary>
        public void CalculateLatencyFromServer()
        {
            TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
            latencyFromServer = (float)(newDateTime.TotalMilliseconds / 1000);
        }

        /// <summary>
        /// Gets the current latency from the server.
        /// </summary>
        /// <returns>The latency in seconds.</returns>
        public float GetLatencyFormServer()
        {
            return latencyFromServer;
        }
    }
}