using System;

namespace Net
{
    /// <summary>
    /// Abstract base class for ping-pong network activity checking.
    /// </summary>
    public abstract class PingPong
    {
        protected NetworkEntity networkEntity;
        public float deltaTime = 0;
        protected int timeUntilDisconnection = 10;
        protected float sendMessageCounter = 0;
        protected float secondsPerCheck = 1.0f;
        protected DateTime currentDateTime = DateTime.UtcNow;
        protected DateTime lastUpdateTime = DateTime.UtcNow;

        /// <summary>
        /// Initializes a new instance of the PingPong class.
        /// </summary>
        /// <param name="networkEntity">The associated network entity.</param>
        public PingPong(NetworkEntity networkEntity)
        {
            this.networkEntity = networkEntity;
        }

        /// <summary>
        /// Updates the ping-pong activity checking.
        /// </summary>
        public void UpdateCheckActivity()
        {
            DateTime currentTime = DateTime.UtcNow;
            deltaTime = (float)(currentTime - lastUpdateTime).TotalSeconds;
            lastUpdateTime = currentTime;

            sendMessageCounter += deltaTime;

            if (sendMessageCounter > secondsPerCheck) // Envio cada 1 segundo el mensaje
            {
                SendPingMessage();
                sendMessageCounter = 0;
            }

            CheckActivityCounter(deltaTime);
            CheckTimeUntilDisconection();
        }

        /// <summary>
        /// Checks the activity counter for timeouts.
        /// </summary>
        /// <param name="deltaTime">The time since last check.</param>
        protected abstract void CheckActivityCounter(float deltaTime);

        /// <summary>
        /// Checks if the connection should be terminated due to inactivity.
        /// </summary>
        protected abstract void CheckTimeUntilDisconection();

        /// <summary>
        /// Sends a ping message.
        /// </summary>
        protected abstract void SendPingMessage();
    }
}