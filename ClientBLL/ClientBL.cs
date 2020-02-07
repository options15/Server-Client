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


        private async void ReadAsync()
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

                    var obj = JsonConvert.DeserializeObject<string[]>(response.ToString());
                    if (obj[0] == "Disconnect")
                    {
                        Disconnect();
                        break;
                    }
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
            SendAsync("Disconnect");
            stream.Close();
            tcpClient.Close();
        }

        public Subscription Subscribe(string method)
        {
            var subscription = new Subscription();
            subscriptions.Add(method, subscription);
            return subscription;
        }

        public void Connect(string login, string password, bool isRegistered = true)
        {
            tcpClient.Connect(ip, port);

            stream = tcpClient.GetStream();
            var method = isRegistered ? "SignIn" : "Registration";
            SendAsync(method, login, password);
            CheckConnection();
            ReadAsync();
        }

        public async void SendAsync(params object[] obj)
        {
            await Task.Factory.StartNew(() =>
            {
                if (obj != null || obj[0].ToString().Length > 0)
                {
                    string output = JsonConvert.SerializeObject(obj);
                    byte[] byteArray = Encoding.UTF8.GetBytes(output);
                    stream.Write(byteArray, 0, byteArray.Length);
                }
            });
        }

    }
}
