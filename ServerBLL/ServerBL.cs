using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerBLL
{
    public sealed class ServerBL
    {
        private readonly TcpListener server;

        internal static List<Connection> connections;
        internal static Dictionary<string, Connection[]> Groups;

        static ServerBL()
        {
            Groups = new Dictionary<string, Connection[]>();
            connections = new List<Connection>();
        }

        public ServerBL(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
        }

        public event Action<object[]> OnGetDataFromClient = (s) => { };
        public event Action<string> OnServerEvent = (s) => { };
        public event Action<Connection> OnClientConnect = (c) => { };
        public event Action<Connection> OnClientDisconnect = (c) => { };


        public void Start()
        {
            server.Start();
            OnServerEvent.Invoke("Server started.");
            while (true)
            {
                var tcpClient = server.AcceptTcpClientAsync().Result;
                ConnectClientAsync(tcpClient);
            }
        }

        public void Stop()
        {
            foreach (var client in connections)
            {
                DisconnectClient(client);
            }
            connections.Clear();
            server.Stop();
            OnServerEvent.Invoke("Server stopped.");
        }

        public async void ConnectClientAsync(TcpClient tcpClient)
        {
            await Task.Factory.StartNew(() =>
            {
                var client = new Connection(tcpClient);

                SubscribeOnClient(client);
                client.Connect();
                connections.Add(client);
                OnClientConnect.Invoke(client);
            });
        }

        public void SendAllClient(object[] message, Connection sender)
        {
            foreach (var client in connections)
            {
                if (client != sender)
                {
                    client.SendAsync(message);
                }
            }
        }

        public void SendToClient(Connection client, object[] message)
        {
              client.SendAsync(message);
        }

        public void DisconnectClient(Connection client)
        {
            if (client.IsConnected)
            {
                client.Disconnect();
            }

            UnsubscribeOnClient(client);

            connections.Remove(connections.FirstOrDefault(x => x == client)); ;
            OnClientDisconnect.Invoke(client);
        }

        public void DisconnectClient(int id)
        {
            var client = connections.FirstOrDefault(x => x.Id == id);
            if (client != null)
            {
                DisconnectClient(client);
            }
        }

        private void SubscribeOnClient(Connection client)
        {
            client.OnGetData += GettingData;
            client.OnDisconnect += DisconnectClient;
        }

        private void UnsubscribeOnClient(Connection client)
        {
            client.OnGetData -= GettingData;
            client.OnDisconnect -= DisconnectClient;
        }

        private void GettingData(object[] data, Connection sender)
        {
            OnGetDataFromClient.Invoke(data);
            var invoker = new HubInvoker();
            if (!invoker.TryInvoke(data))
            {
                OnServerEvent.Invoke("Hub or method not found");
            }
        }
    }
}
