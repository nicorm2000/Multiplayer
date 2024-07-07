using System;

namespace NetworkServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 50001;

            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out int parsedPort))
                {
                    port = parsedPort;
                }
                else
                {
                    Console.WriteLine("El argumento proporcionado no es un número válido. Usando el valor predeterminado.");
                }
            }

            DateTime dateTime = DateTime.UtcNow;
            Server server = new Server(port, dateTime);

            Console.WriteLine($"Server created in port {port} ({dateTime})");

            while (true)
            {
                server.Update();
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
