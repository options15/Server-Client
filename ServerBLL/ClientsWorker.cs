using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public sealed class ClientsWorker
    {
        public IEnumerable<Connection> All => ServerBL.connections;

        public IEnumerable<Connection> AllExcept(params int[] except)
        {
            var temp = new List<Connection>();

            foreach (var con in ServerBL.connections)
            {
                if (!except.Contains(con.Id))
                {
                    temp.Add(con);
                }
            }
            return temp;
        }

        public IEnumerable<Connection> Group(string group)
        {
            return ServerBL.Groups.FirstOrDefault(x => x.Key == group).Value;
        }

    }
}
