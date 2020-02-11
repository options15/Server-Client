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
            server.Start();
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
