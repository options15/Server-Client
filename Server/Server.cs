using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Server
    {
        private TcpListener server;

        public Server(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            try
            {
                StartAsync().GetAwaiter().GetResult();
            }
            catch
            {
                Console.WriteLine("Сервер упал.");
            }
        }
        private async Task StartAsync()
        {
            server.Start();
            Console.WriteLine("Сервер запущен.");
            try
            {
                while (true)
                {
                    Console.WriteLine("Ожидание нового подключения.");
                    var client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                    Console.WriteLine("Подключён новый клиент.");
                    var cc = new ConnectClients(client);
                    await cc.ChatAsync();
                }
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Клиент отключился");
            }
        }
    }
}
