using System;
using ServerCore;
using System.Threading.Tasks;

namespace ServerView
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }
    }
}
