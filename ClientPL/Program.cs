using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientBLL;

namespace ClientPL
{
    class Program
    {
        private static string name;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите своё имя для подключения: ");
                name = Console.ReadLine();
                if (name.Length != 0)
                {
                    break;
                }
                Console.Write("Для подключения требуется имя.");
            }
            var client = new ClientBL(name);
            client.Connect();
            Console.WriteLine("connection close");
            Console.ReadLine();
        }       
    }
}
