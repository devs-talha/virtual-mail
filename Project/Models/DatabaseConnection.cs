using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Project.Models
{
    public static class DatabaseConnection
    {
        private const string ConnectionString = "Host=localhost;Username=postgres;Database=EADProject;Password=root12345";

        public static NpgsqlConnection GetConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
