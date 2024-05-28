using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class UdpConnection
{
    /// <summary>
    /// Represents the data received along with the sender's IP endpoint.
    /// </summary>
    private struct DataReceived
    {
        public byte[] data;
        public IPEndPoint ipEndPoint;
    }

    private readonly UdpClient connection;
    private IReceiveData receiver = null;
    private Queue<DataReceived> dataReceivedQueue = new ();

    // Used for thread-safe locking
    private object handler = new ();

    /// <summary>
    /// Initializes a new instance of the UdpConnection class for a server with a specified port and optional receiver.
    /// </summary>
    /// <param name="port">The port to bind the UDP client to.</param>
    /// <param name="receiver">An optional receiver to handle received data.</param>
    public UdpConnection(int port, IReceiveData receiver = null)
    {
        connection = new UdpClient(port);

        this.receiver = receiver;

        connection.BeginReceive(OnReceive, null);
    }

    /// <summary>
    /// Initializes a new instance of the UdpConnection class for a client with a specified IP address, port, and optional receiver.
    /// </summary>
    /// <param name="ip">The IP address to connect to.</param>
    /// <param name="port">The port to connect to.</param>
    /// <param name="receiver">An optional receiver to handle received data.</param>
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
        connection.Close();
    }

    /// <summary>
    /// Processes all received data in the queue by passing it to the receiver.
    /// </summary>
    public void FlushReceiveData()
    {
        lock (handler)
        {
            // Dequeue and process all received data
            while (dataReceivedQueue.Count > 0)
            {
                DataReceived dataReceived = dataReceivedQueue.Dequeue();
                receiver?.OnReceiveData(dataReceived.data, dataReceived.ipEndPoint);
            }
        }
    }

    /// <summary>
    /// Callback method for handling received data asynchronously.
    /// </summary>
    /// <param name="ar">The result of the asynchronous operation.</param>
    private void OnReceive(IAsyncResult ar)
    {
            DataReceived dataReceived = new();
        try
        {
            // Receive data and store the sender's endpoint
            dataReceived.data = connection.EndReceive(ar, ref dataReceived.ipEndPoint);
        }
        catch (SocketException e)
        {
            // Handle socket exceptions (e.g., client disconnects)
            UnityEngine.Debug.LogError("[UdpConnection] " + e.Message);
        }
        finally
        {
            lock (handler)
            {
                dataReceivedQueue.Enqueue(dataReceived);
            }
        }
        connection.BeginReceive(OnReceive, null);
    }

    /// <summary>
    /// Sends data to the connected endpoint.
    /// </summary>
    /// <param name="data">The data to send.</param>
    public void Send(byte[] data)
    {
        connection.Send(data, data.Length);
    }

    /// <summary>
    /// Sends data to a specified IP endpoint.
    /// </summary>
    /// <param name="data">The data to send.</param>
    /// <param name="ipEndpoint">The IP endpoint to send the data to.</param>
    public void Send(byte[] data, IPEndPoint ipEndpoint)
    {
        connection.Send(data, data.Length, ipEndpoint);
    }

    /// <summary>
    /// Converts an IP address to its long representation.
    /// </summary>
    /// <param name="ipAddress">The IP address to convert.</param>
    /// <returns>A long representation of the IP address.</returns>
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