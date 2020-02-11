using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServerDAL2
{
    public class ClientRepository
    {
        private readonly ClientContext clientContext;
        public ClientRepository()
        {
            clientContext = new ClientContext();
        }

        public IEnumerable<Client> GetAll => clientContext.Clients;

        public Client GetById(int id) => clientContext.Clients.FirstOrDefault(x => x.Id == id);

        public Client GetByLoginAndPass(string login, string password)
        {
            var client = clientContext.Clients.FirstOrDefault(x => x.Login == login);
            if (client.Password == password)
            {
                return client;
            }
            return null;
        }

        public bool Add(string login, string password)
        {
            if (clientContext.Clients.FirstOrDefault(x => x.Login == login) != null)
            {
                return false;
            }

            var client = new Client(login, password);
            clientContext.Clients.Add(client);

            return true;
        }

        public bool DeleteById(int id)
        {
            var client = clientContext.Clients.FirstOrDefault(x => x.Id == id);
            if (client != null)
            {
                clientContext.Clients.Remove(client);
                return true;
            }
            return false;
        }
    }
}
