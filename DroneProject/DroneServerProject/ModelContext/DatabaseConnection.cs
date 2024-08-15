using Microsoft.Data.Sqlite;

namespace DroneProject.ModelContext.Database
{
    public class DatabaseConnection
    {
        public static string ConnectionString { get; set; } = "Data Source=Drone.db";
        public SqliteConnection Connection { get; set; } = new SqliteConnection(ConnectionString);

        public void Conectar()
        {
            if (Connection == null)
            {
                Connection = new SqliteConnection(ConnectionString);
            }

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public void Desconectar()
        {
            if (Connection != null)
            {
                Connection.Close();
            }

            Connection = null;
        }
    }
}
