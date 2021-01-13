using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBLL
{
    public class Hub
    {
        public ClientsWorker Clients => new ClientsWorker();
    }
}
