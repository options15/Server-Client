using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServerBLL
{
    public class ConnectedClient
    {
        private TcpClient connection;
        private NetworkStream stream;
        internal Client clientInfo;

        internal ConnectedClient(TcpClient tcpClient, Client clientInfo)
        {
            this.connection = tcpClient;
            this.clientInfo = clientInfo;
        }

        public event Action<string, ConnectedClient> OnGetMessage = (s, cc) => { };

        internal void Connect()
        {
            Task.Factory.StartNew(() =>
            {
                stream = connection.GetStream();
                while (true)
                {
                    byte[] data = new byte[256];
                    var response = new StringBuilder();

                    do
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        if (bytes == 0)
                        {
                            stream.Close();
                            OnGetMessage.Invoke("Client is down", this);
                            return;
                        }
                        response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    OnGetMessage.Invoke(response.ToString(), this);
                }
            });
        }

        internal async void SendMessage(byte[] message)
        {
            await stream.WriteAsync(message, 0, message.Length).ConfigureAwait(false);
        }

        internal void Disconnect()
        {
            stream.Close();
            connection.Close();
        }
    }
}
