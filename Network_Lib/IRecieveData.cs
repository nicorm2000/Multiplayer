using System.Net;

namespace Net
{
    /// <summary>
    /// Defines a method for receiving network data.
    /// </summary>
    public interface IReceiveData
    {
        /// <summary>
        /// Handles received network data.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <param name="ipEndpoint">The IP endpoint of the sender.</param>
        void OnReceiveData(byte[] data, IPEndPoint ipEndpoint);
    }
}