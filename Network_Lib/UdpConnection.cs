using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Net
{
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

        object handler = new object();

        public UdpConnection(int port, IReceiveData receiver = null)
        {
            connection = new UdpClient(port);

            this.receiver = receiver;

            connection.BeginReceive(OnReceive, null);
        }

        public UdpConnection(IPAddress ip, int port, IReceiveData receiver = null)
        {
            connection = new UdpClient();
            connection.Connect(ip, port);

            this.receiver = receiver;

            connection.BeginReceive(OnReceive, null);
        }

        public void Close()
        {
            connection.Close();
        }

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

                lock (handler)
                {
                    dataReceivedQueue.Enqueue(dataReceived);
                }

                connection.BeginReceive(OnReceive, null);
            }
        }

        public void Send(byte[] data)
        {
            connection.Send(data, data.Length);
        }

        public void Send(byte[] data, IPEndPoint ipEndpoint)
        {
            connection.Send(data, data.Length, ipEndpoint);
        }

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