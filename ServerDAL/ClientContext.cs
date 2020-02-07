using System;
using System.Data.Entity;
using System.Linq;
using Entities;

namespace ServerDAL
{
    internal class ClientContext : DbContext
    {
        public ClientContext()
            : base("name=ClientContext")
        {
        }

        internal DbSet<Client> Clients { get; set; }
    }


}