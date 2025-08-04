using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;

namespace Net
{
    /// <summary>
    /// Manages UDP network connections for sending and receiving data.
    /// </summary>
    public class UdpConnection
    {
        private struct DataReceived
        {
            public byte[] data;
            public IPEndPoint ipEndPoint;
        }

        private readonly UdpClient connection;
        private IReceiveData receiver = null;
        private Queue<DataReceived> dataReceivedQueue = new Queue<DataReceived>();
        private bool isClosed = false;
        object handler = new object();

        /// <summary>
        /// Initializes a new instance of the UdpConnection class for listening on a specific port.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="receiver">The receiver for incoming data.</param>
        public UdpConnection(int port, IReceiveData receiver = null)
        {
            const int maxRetries = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    connection = new UdpClient(port);
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[UdpConnection] Port bind failed (attempt {attempt}) on port {port}: {ex.Message}");
                    if (attempt == maxRetries) throw;

                    System.Threading.Thread.Sleep(50); // Wait a bit before retry
                }
            }

            this.receiver = receiver;
            connection.BeginReceive(OnReceive, null);
        }

        /// <summary>
        /// Initializes a new instance of the UdpConnection class for connecting to a specific IP and port.
        /// </summary>
        /// <param name="ip">The IP address to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="receiver">The receiver for incoming data.</param>
        public UdpConnection(IPAddress ip, int port, IReceiveData receiver = null)
        {
            connection = new UdpClient();
            connection.Connect(ip, port);

            this.receiver = receiver;

            connection.BeginReceive(OnReceive, null);
        }

        /// <summary>
        /// Closes the UDP connection.
        /// </summary>
        public void Close()
        {
            if (isClosed) return;

            isClosed = true;

            try
            {
                connection.Close();
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Processes all received data in the queue.
        /// </summary>
        public void FlushReceiveData()
        {
            lock (handler)
            {
                while (dataReceivedQueue.Count > 0)
                {
                    DataReceived dataReceived = dataReceivedQueue.Dequeue();
                    if (receiver != null)
                        receiver.OnReceiveData(dataReceived.data, dataReceived.ipEndPoint);
                }
            }
        }

        /// <summary>
        /// Callback for when data is received.
        /// </summary>
        /// <param name="ar">The async result.</param>
        void OnReceive(IAsyncResult ar)
        {
            DataReceived dataReceived = new DataReceived();

            try
            {
                dataReceived.data = connection.EndReceive(ar, ref dataReceived.ipEndPoint);
            }
            catch (SocketException e)
            {
                // This happens when a client disconnects, as we fail to send to that port.
                Console.WriteLine("[UdpConnection] " + e.Message);
            }
            finally
            {
                if (!isClosed)
                {
                    lock (handler)
                    {
                        connection.BeginReceive(OnReceive, null);
                    }
                    dataReceivedQueue.Enqueue(dataReceived);
                }
            }
        }

        /// <summary>
        /// Sends data to the connected endpoint.
        /// </summary>
        /// <param name="data">The data to send.</param>

        public void Send(byte[] data)
        {
            if (isClosed) return;
            connection.Send(data, data.Length);
        }

        /// <summary>
        /// Sends data to a specific endpoint.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="ipEndpoint">The target endpoint.</param>
        public void Send(byte[] data, IPEndPoint ipEndpoint)
        {
            if (isClosed) return;
            connection.Send(data, data.Length, ipEndpoint);
        }

        /// <summary>
        /// Converts an IP address to a long value.
        /// </summary>
        /// <param name="ipAddress">The IP address to convert.</param>
        /// <returns>The long representation of the IP address.</returns>
        public static long IPToLong(IPAddress ipAddress)
        {
            byte[] bytes = ipAddress.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}