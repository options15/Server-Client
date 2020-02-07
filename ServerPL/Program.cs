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
        static void Main()
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
        static void ShowMessage(object[] obj)
        {
            Console.WriteLine(obj[0].ToString());
        }
    }
}
