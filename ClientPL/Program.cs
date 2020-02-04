using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientView
{
    class Program
    {
        private static TcpClient tcpClient = new TcpClient();
        private static string ip = "127.0.0.1";
        private static int port = 5050;
        private static string name;
        private static NetworkStream stream;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите своё имя для подключения: ");
                name = Console.ReadLine();
                if (name.Length != 0)
                {
                    break;
                }
                Console.Write("Для подключения требуется имя.");
            }
            Connect();
        }

        private static void Connect()
        {
            tcpClient.Connect(ip, port);

            if (tcpClient.Connected)
            {
                Console.WriteLine("Подключено.");
            }
            stream = tcpClient.GetStream();
            ReadServer();
            while (true)
            {
                byte[] message = Encoding.UTF8.GetBytes(Console.ReadLine());
                stream.Write(message, 0 ,message.Length);
            }          
        }
        private static async void ReadServer()
        {
            await Task.Factory.StartNew(()=>
            {
                while (true)
                {
                    byte[] data = new byte[256];
                    StringBuilder response = new StringBuilder();

                    do
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    if (response.ToString() == "exit")
                    {
                        break;
                    }
                    Console.WriteLine(response.ToString());
                }
            });
        }
    }
}
