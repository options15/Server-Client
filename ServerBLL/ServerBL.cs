using ServerDAL;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerBLL
{
    public class ServerBL
    {
        private readonly TcpListener server;
        internal List<Connection> connections;

        public ServerBL(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
            connections = new List<Connection>();
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

        public async void ConnectClientAsync(TcpClient tcpClient)
        {
            await Task.Factory.StartNew(() =>
            {
                var cc = new Connection(tcpClient);

                OnServerEvent.Invoke("New client connected.");

                cc.OnGetData += GettingData;
                cc.OnDisconnect += DisconnectClient;
                cc.Connect();
                OnClientConnect.Invoke(cc);
            });
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



        public void SendAllClient(string message, Connection sender)
        {
            foreach (var client in connections)
            {
                if (client != sender)
                {
                    client.SendAsync(message);
                }
            }
        }

        public void DisconnectClient(Connection client)
        {
            if (client.IsConnected())
            {
                client.Disconnect();
            }

            client.OnGetData -= GettingData;
            client.OnDisconnect -= DisconnectClient;

            connections.Remove(client);
            OnClientDisconnect.Invoke(client);
        }


        private void GettingData(object[] data, Connection sender)
        {
            OnGetDataFromClient.Invoke(data);
        }
    }
}
