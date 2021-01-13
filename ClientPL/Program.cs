using Client;
using System;

namespace ClientPL
{
    class Program
    {
        static void Main()
        {
            Connect();
        }

        static void Connect()
        {
            var client = new Connection("127.0.0.1", 5050);
            Subscription takeMessge = client.Subscribe("SendMessage");
            takeMessge.Data += Write;
            client.Connect();
            while (true)
            {
                var message = Console.ReadLine();
                if (client.IsConnected())
                {
                    client.SendAsync("MyHub", "SendMessage", message);
                }
                else
                {
                    break;
                }
            }
        }
        private static void Write(params object[] obj)
        {
            Console.WriteLine(obj[0].ToString() + ":" + obj[1].ToString());
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
