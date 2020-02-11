using Entities;
using System.Collections.Generic;
using System.Linq;

namespace ServerDAL
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
            return clientContext.Clients.FirstOrDefault(x => x.Login == login && x.Password == password);
        }

        public bool Add(string login, string password)
        {
            if (clientContext.Clients.FirstOrDefault(x => x.Login == login) != null)
            {
                return false;
            }

            var client = new Client(login, password);
            clientContext.Clients.Add(client);
            clientContext.SaveChanges();

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
