using ServerDAL;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;

namespace ServerBLL
{
    public class ServerBL
    {
        private readonly TcpListener server;
        private Dictionary<int, Connection> connections;

        internal static Dictionary<string, Method> methods;
        internal static Dictionary<string, Connection[]> Groups;

        private int lastId = 0;
        private int SetId => lastId < 0 ? 0 : lastId++; 

        static ServerBL()
        {
            methods = new Dictionary<string, Method>();
            Groups = new Dictionary<string, Connection[]>();
        }

        public ServerBL(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
            connections = new Dictionary<int, Connection>();
        }

        public event Action<object[]> OnGetDataFromClient = (s) => { };
        public event Action<string> OnServerEvent = (s) => { };
        public event Action<Connection> OnClientConnect = (c) => { };
        public event Action<Connection> OnClientDisconnect = (c) => { };


        public IReadOnlyDictionary<int, Connection> Clients => connections;

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
                connections.Add(SetId, cc);
                OnClientConnect.Invoke(cc);
            });
        }

        public void Stop()
        {
            foreach (var client in connections.Values)
            {
                DisconnectClient(client);
            }
            connections.Clear();
            server.Stop();
            OnServerEvent.Invoke("Server stopped.");
        }

        public void Listener(string method)
        { 
        
        }

        public void SendAllClient(object[] message, Connection sender)
        {
            foreach (var client in connections.Values)
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

            connections.Remove(connections.FirstOrDefault(x => x.Value == client).Key); ;
            OnClientDisconnect.Invoke(client);
        }


        private void GettingData(object[] data, Connection sender)
        {
            OnGetDataFromClient.Invoke(data);
            SendAllClient(data, sender);
        }
    }
}
