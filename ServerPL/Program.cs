using ServerBLL;
using System;

namespace ServerPL
{
    class Program
    {
        static void Main()
        {
            ServerBL server = new ServerBL();

            server.OnGetDataFromClient += ShowMessage;
            server.OnServerEvent += ShowMessage;
            server.OnClientDisconnect += Server_OnClientDisconnect;

            server.Start();
        }

        private static void Server_OnClientDisconnect(Connection obj)
        {
            Console.WriteLine("Client disconnect");
        }

        static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
        static void ShowMessage(object[] obj)
        {
            foreach (var o in obj)
            {
                Console.Write(o.ToString() + ": ");
            }
            Console.WriteLine();
        }
    }
}
