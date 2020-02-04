using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerBLL;

namespace ServerPL
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerBL server = new ServerBL();
            server.OnGetMessgeFromClient += ShowMessage;
            server.OnServerEvent += ShowMessage;
            server.Start();
        }

        static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
