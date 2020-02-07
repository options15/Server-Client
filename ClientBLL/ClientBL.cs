using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientBLL
{
    public class ClientBL
    {
        private TcpClient tcpClient;
        private string ip;
        private int port;
        private NetworkStream stream;
        private Dictionary<string, Subscription> subscriptions;

        public ClientBL(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            tcpClient = new TcpClient();
            subscriptions = new Dictionary<string, Subscription>();
        }
        public void Connect(string login, string password, bool isRegistered = true)
        {
            tcpClient.Connect(ip, port);

            stream = tcpClient.GetStream();
            var method = isRegistered ? "SignIn" : "Registration";
            Send(method, "Server", login, password);
            CheckConnection();
            Read();
        }

        public void Send(params object[] obj)
        {
            if (obj != null || obj[0].ToString().Length > 0)
            {
                string output = JsonConvert.SerializeObject(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(output);
                stream.Write(byteArray, 0, byteArray.Length);
            }
        }

        private async void Read()
        {
            await Task.Factory.StartNew(() =>
            {
                byte[] data = new byte[256];
                while (true)
                {
                    var response = new StringBuilder();

                    do
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    var message = response.ToString();
                    if (message == "Disconnect")
                    {
                        Disconnect();
                        break;
                    }
                    var obj = JsonConvert.DeserializeObject<string[]>(message);
                    InvokeSubscribe(obj);
                }
            });
        }

        private void InvokeSubscribe(params string[] str)
        {
            foreach (var sub in subscriptions)
            {
                if (sub.Key == str[0])
                {
                    sub.Value.Invoke(str);
                }
            }
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

        public void Disconnect()
        {
            Send("Di");
            stream.Close();
            tcpClient.Close();
        }

        public Subscription Subscribe(string method)
        {
            var subscription = new Subscription();
            subscriptions.Add(method, subscription);
            return subscription;
        }
    }
}
