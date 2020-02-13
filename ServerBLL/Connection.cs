using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public sealed class Connection
    {
        private readonly TcpClient connection;
        private NetworkStream stream;

        internal Connection(TcpClient tcpClient)
        {
            connection = tcpClient;
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
        internal async void Connect()
        {
            await Task.Factory.StartNew(() =>
            {
                stream = connection.GetStream();
                while (true)
                {
                    var message = Read();
                    if (message == null || message.Length == 0)
                    {
                        Disconnect();
                        OnDisconnect.Invoke(this);
                        return;
                    }
                    OnGetData.Invoke(message, this);
                }
            });
        }

        /// <summary>
        /// Close stream and disconnect client from the server
        /// </summary>
        internal void Disconnect()
        {
            stream.Close();
            connection.Close();
            OnDisconnect.Invoke(this);
        }

        /// <summary>
        /// Show client is connected.
        /// </summary>
        /// <returns></returns>
        internal bool IsConnected() => connection.Connected;

        /// <summary>
        /// Deserialize objects and async send to client.
        /// </summary>
        /// <param name="obj"></param>
        internal async void SendAsync(params object[] obj)
        {
            if (connection.Connected)
            {
                await Task.Factory.StartNew(() =>
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
                });
            }
        }

        /// <summary>
        /// Read stream and convert to array of objects.
        /// </summary>
        /// <returns> array of objects </returns>
        private object[] Read()
        {
            byte[] data = new byte[256];
            var json = new StringBuilder();
            try
            {
                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    json.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (stream.DataAvailable);
            }
            catch
            {
                Disconnect();
            }
            return JsonConvert.DeserializeObject<object[]>(json.ToString());
        }
    }
}
