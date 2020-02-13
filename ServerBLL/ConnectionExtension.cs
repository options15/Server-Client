using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public static class ConnectionExtension
    {
        public static IEnumerable<Connection> All(this IEnumerable<Connection> connections)
        {
            return connections;
        }

        public static IEnumerable<Connection> AllExcept(this IEnumerable<Connection> connections, params Connection[] except)
        {
            var temp = new List<Connection>();
            foreach (var con in connections)
            {
                if (!except.Contains(con))
                {
                    temp.Add(con);
                }
            }
            return temp;
        }

        public static IEnumerable<Connection> Group(this IEnumerable<Connection> connections, string group)
        {
            return ServerBL.Groups.FirstOrDefault(x => x.Key ==group).Value;
        }

        public static IEnumerable<Connection> Invoke(this IEnumerable<Connection> connections, string method)
        {
            return ServerBL.Groups.FirstOrDefault(x => x.Key == method).Value;
        }
    }
}
