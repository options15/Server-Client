using ClientBLL;
using System;

namespace ClientPL
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("If you not register, input 'r', else press enter");

            switch (Console.ReadLine())
            {
                case "r":
                    Registration();
                    break;
                default:
                    Connect();
                    break;
            }
        }

        static void Connect()
        {
            LoginAndPassword(out string login, out string password);
            var client = new ClientBL("127.0.0.1", 5050);
            Subscription takeMessge = client.Subscribe("ChatMessage");
            takeMessge.Data += Write;
            client.Connect(login, password);
            while (true)
            {
                client.SendAsync("ChatMessage", Console.ReadLine());
            }
        }
        private static void Write(params object[] obj)
        {
            Console.WriteLine(obj[0].ToString() + ":" + obj[1].ToString());
        }
        static void Registration()
        {
            LoginAndPassword(out string login, out string password);
            var client = new ClientBL("127.0.0.1", 5050);
            client.Connect(login, password, false);
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
