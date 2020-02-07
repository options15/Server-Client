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
        static void Main()
        {
            string login = "";
            string password = "";
            LoginAndPassword(out login, out password);

            Console.WriteLine("");

            var client = new ClientBL("127.0.0.1", 5050);
            Subscription takeMessge = client.Subscribe("ChatMessage");
            takeMessge.Data += Write;
            client.Connect(login, password);
            while (true)
            {
                client.Send("ChatMessage", Console.ReadLine());
            }

        }
        private static void Write(params object[] obj)
        {
            Console.WriteLine(obj[0].ToString() + ":" + obj[1].ToString());
        }
        static void Registration()
        {

        }

        static void LoginAndPassword(out string login, out string password)
        {
            while (true)
            {
                Console.Write("Введите логин для подключения: ");
                login = Console.ReadLine();
                if (login.Length >= 6)
                {
                    break;
                }
                Console.Write("Логин не может быть меньше 6 символов");
            }
            while (true)
            {
                Console.Write("Введите пароль: ");
                password = Console.ReadLine();
                if (password.Length >= 6)
                {
                    break;
                }
                Console.Write("Пароль не может быть меньше 6 символов");
            }
        }
    }
}
