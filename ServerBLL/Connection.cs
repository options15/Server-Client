using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public sealed class Connection
    {
        private static int lastId = 0;
        private readonly TcpClient connection;
        private NetworkStream stream;

        public int Id { get; private set; } 
        internal bool IsConnected { get; private set; } = false;

        internal Connection(TcpClient tcpClient)
        {
            connection = tcpClient;
            Id = lastId++;
        }

        /// <summary>
        /// The event invoke when a message is received from the client.
        /// </summary>
        internal event Action<object[], Connection> OnGetData = (s, c) => { };
        internal event Action<Connection> OnDisconnect = (c) => { };

        /// <summary>
        /// Server create new async connection for new client
        /// and listen stream in endless cycle. when receiving data,
        /// it triggers an event OnGetMessage on the server.
        /// </summary>
        /// 
        internal void Connect()
        {
            stream = connection.GetStream();
            IsConnected = true;
            Read();
        }

        /// <summary>
        /// Close stream and disconnect client from the server
        /// </summary>
        internal void Disconnect()
        {
            IsConnected = false;
            stream.Close();
            connection.Close();
            OnDisconnect.Invoke(this);
        }

        /// <summary>
        /// Deserialize objects and async send to client.
        /// </summary>
        /// <param name="obj"></param>
        internal async void SendAsync(params object[] obj)
        {

            await Task.Factory.StartNew(() =>
            {
                if (IsConnected)
                {
                    var json = JsonConvert.SerializeObject(obj);
                    var message = Encoding.UTF8.GetBytes(json);
                    try
                    {
                        stream.Write(message, 0, message.Length);
                    }
                    catch
                    {
                        Disconnect();
                    }
                }
            });
        }

        /// <summary>
        /// Read stream and convert to array of objects.
        /// </summary>
        /// <returns> array of objects </returns>
        private void Read()
        {
            byte[] data = new byte[256];
            var input = new StringBuilder();
            while (IsConnected)
            {
                input.Clear();
                try
                {
                    do
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        input.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);
                }
                catch
                {
                    Disconnect();
                    return;
                }

                var message = JsonConvert.DeserializeObject<object[]>(input.ToString());
                OnGetData.Invoke(message, this);
            }
        }
    }
}
