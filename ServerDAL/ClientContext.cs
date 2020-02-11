﻿using Entities;
using System.Data.Entity;
using System.Data.SqlClient;

namespace ServerDAL
{
    public class ClientContext : DbContext
    {
        public ClientContext()
            : base("ClientContext")
        {
        }

        public DbSet<Client> Clients { get; set; }
    }


}