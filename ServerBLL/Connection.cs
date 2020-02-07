using Entities;
using Newtonsoft.Json;
using ServerDAL;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public class Connection
    {
        private TcpClient connection;
        public NetworkStream stream;

        internal Connection(TcpClient tcpClient)
        {
            this.connection = tcpClient;
        }

        /// <summary>
        /// The event invoke when a message is received from the client.
        /// </summary>
        public event Action<object[], Connection> OnGetData = (s, cc) => { };

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
        }

        /// <summary>
        /// Read stream and convert to array of objects.
        /// </summary>
        /// <returns> array of objects </returns>
        private object[] Read()
        {
            byte[] data = new byte[256];
            var json = new StringBuilder();

            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                json.Append(Encoding.UTF8.GetString(data, 0, bytes));
            } while (stream.DataAvailable);

            return JsonConvert.DeserializeObject<object[]>(json.ToString());
        }
    }
}
