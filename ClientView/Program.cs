using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
            byte[] data = new byte[256];
            StringBuilder response = new StringBuilder();

            if (tcpClient.Connected)
            {
                Console.WriteLine("Подключено.");
            }
            stream = tcpClient.GetStream();
            Thread thread = new Thread(()=> Reader(stream));
            thread.Start();
            while (true)
            {
                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);

                Console.WriteLine(response.ToString());
                if (response.ToString() == "exit")
                {
                    thread.Abort();
                    break;
                }
            }
        }

        private static void Reader(NetworkStream stream)
        {
            while (true)
            {
                string data = Console.ReadLine();

                stream.Write(Encoding.UTF8.GetBytes(data));
                stream.Flush();
            }
        }
    }
}
