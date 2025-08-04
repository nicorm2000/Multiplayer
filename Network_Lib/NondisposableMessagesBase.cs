using System.Collections.Generic;

namespace Net
{
    /// <summary>
    /// Abstract base class for handling non-disposable messages.
    /// </summary>
    public abstract class NondisponsableMessageBase
    {
        protected NetworkEntity networkEntity;
        protected Dictionary<byte[], float> MessagesHistory = new Dictionary<byte[], float>();
        protected int secondsToDeleteMessageHistory = 15;
        protected PingPong pingPong;

        /// <summary>
        /// Initializes a new instance of the NondisponsableMessageBase class.
        /// </summary>
        /// <param name="networkEntity">The associated network entity.</param>
        public NondisponsableMessageBase(NetworkEntity networkEntity)
        {
            this.networkEntity = networkEntity;
            networkEntity.onInitPingPong += () => pingPong = networkEntity.checkActivity;
        }

        /// <summary>
        /// Processes received data and handles confirmations.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <param name="id">The client ID.</param>
        public void OnReceivedData(byte[] data, int id)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);
            MessageType messageType = MessageChecker.CheckMessageType(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                SendConfirmationMessage(messageType, id);
            }

            if (messageType == MessageType.Confirm)
            {
                NetConfirmMessage netConfirm = new NetConfirmMessage(data);

                HandleConfirmationMessage(netConfirm, id);
            }
        }

        /// <summary>
        /// Handles a confirmation message.
        /// </summary>
        /// <param name="netConfirm">The confirmation message.</param>
        /// <param name="id">The client ID.</param>
        protected abstract void HandleConfirmationMessage(NetConfirmMessage netConfirm, int id);

        /// <summary>
        /// Adds a sent message to the tracking queue.
        /// </summary>
        /// <param name="data">The message data.</param>
        /// <param name="clientId">The client ID.</param>
        public abstract void AddSentMessages(byte[] data, int clientId = -1);

        /// <summary>
        /// Sends a confirmation message for a specific message type.
        /// </summary>
        /// <param name="messageType">The message type to confirm.</param>
        /// <param name="id">The client ID.</param>
        public abstract void SendConfirmationMessage(MessageType messageType, int id = -1);

        /// <summary>
        /// Resends unconfirmed packages.
        /// </summary>
        public abstract void ResendPackages();
    }
}