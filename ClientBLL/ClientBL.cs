using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientBLL
{
    public class ClientBL
    {
        private TcpClient tcpClient;
        private const string localIP = "127.0.0.1";
        private const int defaultPort = 5050;
        private NetworkStream stream;
        private string name;

        public Action<string> OnGetMessage;
        public ClientBL(string name)
        {
            this.name = name;
            tcpClient = new TcpClient();
        }
        public  void Connect(string ip = localIP, int port = defaultPort)
        {
            tcpClient.Connect(ip, port);

            if (tcpClient.Connected)
            {
                Console.WriteLine("Подключено.");
            }
            stream = tcpClient.GetStream();
            CheckConnection();
            ReadFromServer();
            SendToServer();
        }

        private void SendToServer()
        {

                while (true)
                {
                    byte[] message = Encoding.UTF8.GetBytes(Console.ReadLine());
                    stream.Write(message, 0, message.Length);
                }
                

        }
        private async void ReadFromServer()
        {
            await Task.Factory.StartNew(() =>
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
                    OnGetMessage.Invoke(response.ToString());
;;                }
            });
        }

        private async void CheckConnection()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!tcpClient.Connected)
                    {
                        tcpClient.Close();
                        Console.WriteLine("Server shutdown.");
                        break;
                    }
                }
            });
        }
    }
}
