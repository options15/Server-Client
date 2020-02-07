using Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServerDAL;
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
        internal static Dictionary<Client, Connection> connections;

        public ServerBL(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
            connections = new Dictionary<Client, Connection>();
        }

        public event Action<object[]> OnGetMessgeFromClient = (s) => { };
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

                cc.OnGetData += GettingMessage;
                cc.Connect();
                OnClientConnect.Invoke(cc);
            });
        }

        public void Stop()
        {
            foreach (var client in connections)
            {
                DisconnectClient(client.Value);
            }
            connections.Clear();
            server.Stop();
            OnServerEvent.Invoke("Server stopped.");
        }

        private void ValidateLoginAndPassword(string login, string password, Connection connection)
        {
            var clientRepository = new ClientRepository();
            var client = clientRepository.GetByLoginAndPass(login, password);
            if (client == null)
            {
                connection.Disconnect();
            }
        }

        private void RegistrationNewClient(string login, string password)
        {
            var clientRepository = new ClientRepository();
            if (clientRepository.Add(login, password))
            {

            }
        }

        public void SendMessageToAllClient(string message, Connection sender)
        {
            foreach (var client in connections.Values)
            {
                if (client != sender)
                {
                    SendAsync(client, "ChatMessage", message);
                }
            }
        }

        private bool IsConnected(string login)
        {
            foreach (var con in connections.Keys)
            {
                if (con.Login == login)
                {
                    return true;
                }
            }
            return false;
        }

        internal async void SendAsync(Connection connection, params object[] obj)
        {
            await Task.Factory.StartNew(() =>
            {
                var json = JsonConvert.SerializeObject(obj);
                var message = Encoding.UTF8.GetBytes(json);
                connection.stream.WriteAsync(message, 0, message.Length).ConfigureAwait(false);
            });
        }

        public void DisconnectClient(Connection client)
        {
            client.Disconnect();
            client.OnGetData -= GettingMessage;
            OnClientDisconnect.Invoke(client);
        }

        private void GettingMessage(object[] message, Connection sender)
        {
            OnGetMessgeFromClient.Invoke(message);
        }       
    }
}
