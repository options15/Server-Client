using Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public class ServerBL
    {
        private TcpListener server;
        private List<ConnectedClient> clients;

        public ServerBL(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
            clients = new List<ConnectedClient>();
        }

        public event Action<string> OnGetMessgeFromClient = (s) => { };
        public event Action<string> OnServerEvent = (s) => { };

        public void Start()
        {
            server.Start();
            OnServerEvent.Invoke("Server started.");
            while (true)
            {
                var client = server.AcceptTcpClientAsync().Result;
                ConnectedNewClient(client);
            }
        }

        public void ConnectedNewClient(TcpClient tcpClient)
        {
            Task.Factory.StartNew(() =>
            {
                //var name = SearchClientInTheList(tcpClient);
                //if (!IsCanConnect(name))
                //{
                //    tcpClient.Close();
                //    return;
                //}
                var cc = new ConnectedClient(tcpClient, new Client());
                OnServerEvent.Invoke("New client connected.");
                clients.Add(cc);
                cc.OnGetMessage += GetMessgeFromClient;
                cc.Connect();
            });
        }

        private bool IsCanConnect(string name)
        {
            if (name == null)
            {
                return false;
            }
            foreach (var cl in clients)
            {
                if (cl.clientInfo.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        public string SearchClientInTheList(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            byte[] data = new byte[256];
            var response = new StringBuilder();

            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                if (bytes == 0)
                {
                    stream.Close();
                    return null;
                }
                response.Append(Encoding.UTF8.GetString(data, 0, bytes));
            } while (stream.DataAvailable);
            return response.ToString();
        }

        public void SendMessageToAllClient(string message, ConnectedClient sender)
        {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            foreach (var client in clients)
            {
                if (client != sender)
                    client.SendMessage(byteMessage);
            }
        }

        public void Stop()
        {
            foreach (var client in clients)
            {
                DisconnectClient(client);
            }
            clients.Clear();
            server.Stop();
            OnServerEvent.Invoke("Server stopped.");
        }

        public void DisconnectClient(ConnectedClient client)
        {
            client.Disconnect();
            client.OnGetMessage -= GetMessgeFromClient;

        }

        private void GetMessgeFromClient(string message, ConnectedClient sender)
        {
            OnGetMessgeFromClient.Invoke(message);
            SendMessageToAllClient(message, sender);
        }
    }
}
