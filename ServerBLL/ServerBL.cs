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

        public event Action<string> OnGetMessgeFromClient = (s)=> { };
        public event Action<string> OnServerEvent = (s) => { };

        public void Start()
        {
            server.Start();
            OnServerEvent.Invoke("Server started.");
            while (true)
            {
                var client = server.AcceptTcpClient();
                var cc = new ConnectedClient(client);
                OnServerEvent.Invoke("New client connected.");
                clients.Add(cc);
                cc.OnGetMessage += GetMessgeFromClient;
                Task.Factory.StartNew(()=> cc.ConnectAsync());
            }
        }

        public void SendMessageToAllClient(string message, ConnectedClient sender)
        {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            foreach (var client in clients)
            {
                if(client != sender)
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
