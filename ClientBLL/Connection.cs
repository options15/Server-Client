using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Connection
    {
        private TcpClient tcpClient;
        private string ip;
        private int port;
        private NetworkStream stream;
        private Dictionary<string, Subscription> subscriptions;

        public event Action OnDisconnect = () => { };

        public Connection(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            tcpClient = new TcpClient();
            subscriptions = new Dictionary<string, Subscription>();
        }

        public bool IsConnected() => tcpClient.Connected;


        private async void ReadAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                byte[] data = new byte[256];
                while (true)
                {
                    var response = new StringBuilder();
                    try
                    {
                        do
                        {
                            int bytes = stream.Read(data, 0, data.Length);
                            response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        } while (stream.DataAvailable);
                    }
                    catch
                    {
                        Disconnect();
                        return;
                    }

                    var obj = JsonConvert.DeserializeObject<string[]>(response.ToString());
                    if (obj == null || obj[0] == "Disconnect")
                    {
                        Disconnect();
                        return;
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

        public void Disconnect()
        {
            SendAsync("Disconnect");
            stream.Close();
            tcpClient.Close();
            OnDisconnect.Invoke();
        }

        public Subscription Subscribe(string method)
        {
            var subscription = new Subscription();
            subscriptions.Add(method, subscription);
            return subscription;
        }

        public bool Connect()
        {
            try
            {
                tcpClient.Connect(ip, port);
            }
            catch
            {
                return false;
            }

            stream = tcpClient.GetStream();
            ReadAsync();

            return true;
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
