using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class ConnectClients
    {
        private TcpClient client;

        public ConnectClients(TcpClient client)
        {
            this.client = client;
        }

        public async Task ChatAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var stream = client.GetStream())
                    {
                        using (var sr = new StreamReader(stream))
                        using (var sw = new StreamWriter(stream))
                        {
                            string message = "";
                            while (true)
                            {
                                message = sr.ReadToEnd();
                                sw.Write(message);
                                sw.Flush();
                            }
                        }

                    }
                }
                finally
                {
                    if (client != null)
                    {
                        (client as IDisposable).Dispose();
                        client = null;
                    }
                }
            }
            );
        }

    }
}

