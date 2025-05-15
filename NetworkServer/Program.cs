using System;
using System.Runtime.InteropServices;

namespace NetworkServer
{
    class Program
    {
        static Server server;

        // Delegate for handler
        private delegate bool ConsoleEventDelegate(int eventType);
        private static ConsoleEventDelegate handler;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        private const int CTRL_C_EVENT = 0;
        private const int CTRL_CLOSE_EVENT = 2;

        static void Main(string[] args)
        {
            int port = 50101;

            if (args.Length > 0 && int.TryParse(args[0], out int parsedPort))
            {
                port = parsedPort;
            }

            DateTime dateTime = DateTime.UtcNow;
            server = new Server(port, dateTime);
            Console.WriteLine($"Server created in port {port} ({dateTime})");

            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            while (true)
            {
                server.Update();
                System.Threading.Thread.Sleep(10);
            }
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == CTRL_CLOSE_EVENT || eventType == CTRL_C_EVENT)
            {
                Console.WriteLine("[Server] Console closing — cleaning up");

                server.OnApplicationQuit();

                System.Threading.Thread.Sleep(200);
            }

            return false;
        }
    }

}
