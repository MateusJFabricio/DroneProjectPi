using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

namespace DroneProject.ModelContext.Database
{
    public class DatabaseConnection
    {
        private string _connectionString;
        public SqliteConnection Connection { get; set; }

        public DatabaseConnection(string conn)
        {
            _connectionString = conn;
            _connectionString = "Data Source=Drone.db";
        }

        public void Conectar()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();   
        }

        public void Desconectar()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }
    }
}
