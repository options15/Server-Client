using Entities;
using Newtonsoft.Json;
using ServerDAL;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ServerBLL
{
    public class ServerBL
    {
        private readonly TcpListener server;
        private readonly ClientRepository clientRepository;
        internal  List<Connection> connections;

        public ServerBL(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
            connections = new List<Connection>();
            clientRepository = new ClientRepository();
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
                    client.SendAsync("ChatMessage", message);
                }
            }
        }

        private bool IsConnected(string login)
        {
            return connections.Exists(x => x.ClientInfo.Login == login);
        }

        public void DisconnectClient(Connection client)
        {
            client.Disconnect();
            client.OnGetData -= GettingData;
            OnClientDisconnect.Invoke(client);
        }

        private void GettingData(object[] data, Connection sender)
        {
            OnGetDataFromClient.Invoke(data);
        }   
    }
}
