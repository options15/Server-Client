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
        internal  List<Connection> connections;

        public ServerBL(int port = 5050)
        {
            server = new TcpListener(IPAddress.Any, port);
            connections = new List<Connection>();
        }

        public event Action<object[]> OnGetMessageFromClient = (s) => { };
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
                DisconnectClient(client);
            }
            connections.Clear();
            server.Stop();
            OnServerEvent.Invoke("Server stopped.");
        }

        private void ValidateLoginAndPassword(string login, string password, Connection connection)
        {
            if (IsConnected(login))
            {
                var connect = connections.FirstOrDefault(x => x.ClientInfo.Login == login);
                connection.SetClient(connect.ClientInfo);
                connect.SendAsync("You were forcibly disabled.");
                connect.Disconnect();
            }
            var clientRepository = new ClientRepository();
            var client = clientRepository.GetByLoginAndPass(login, password);
            if (client == null)
            {
                connection.Disconnect();
            }
        }

        private void RegistrationNewClient(string login, string password, Connection sender)
        {
            var clientRepository = new ClientRepository();
            if (clientRepository.Add(login, password))
            {
                sender.SendAsync("Registration", "You have successfully registered, now you can sign in.");
            }
            else
            {
                sender.SendAsync("Registration", "Failure, this login is used.");
            }
            sender.Disconnect();
        }

        public void SendMessageToAllClient(string message, Connection sender)
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
            client.OnGetData -= GettingMessage;
            OnClientDisconnect.Invoke(client);
        }

        private void GettingMessage(object[] data, Connection sender)
        {
            SearchMethod(data, sender);
            OnGetMessageFromClient.Invoke(data);
        }

        private void SearchMethod(object[]  data, Connection sender)
        {
            switch (data[0])
            { 
                case "SignIn" :
                    RegistrationNewClient(data[1].ToString(), data[2].ToString(), sender);
                    break;
                case "Registration" :
                    ValidateLoginAndPassword(data[1].ToString(), data[2].ToString(), sender);
                    break;
                default:
                    break;
            }
        }
    }
}
