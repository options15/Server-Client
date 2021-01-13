using System.Collections.Generic;
using System.Linq;

namespace ServerBLL
{
    public static class ConnectionExtension
    {
        public static void Invoke(this IEnumerable<Connection> сonnections, params object[] obj)
        {
            foreach (var conn in сonnections)
            {
                conn.SendAsync(obj);
            }
        }
    }
}
