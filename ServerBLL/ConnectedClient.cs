using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public class ConnectedClient
    {
        private TcpClient client;
        private NetworkStream stream;

        internal ConnectedClient(TcpClient client)
        {
            this.client = client;
        }

        public event Action<string, ConnectedClient> OnGetMessage = (s, cc) => { };

        internal void ConnectAsync()
        {
            stream = client.GetStream();

            while (true)
            {
                byte[] data = new byte[256];
                StringBuilder response = new StringBuilder(); 

                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (stream.DataAvailable);

                OnGetMessage.Invoke(response.ToString(), this);
            }
        }

        public async void SendMessage(byte[] message)
        {
            await Task.Factory.StartNew(() =>
            {
                stream.WriteAsync(message, 0, message.Length).ConfigureAwait(false);
            });
        }


        internal void Disconnect()
        {
            stream.Close();
            client.Close();
        }
    }
}
