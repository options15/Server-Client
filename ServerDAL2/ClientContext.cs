namespace ServerDAL2
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Entities;

    public class ClientContext : DbContext
    {
        public ClientContext()
            : base("ClientContext")
        {
        }
        public DbSet<Client> Clients { get; set; }
    }
}