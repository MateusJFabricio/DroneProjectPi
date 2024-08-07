using Microsoft.Data.Sqlite;
using System.Data;

namespace DronePIProject.ModelContext.Database
{
    public interface ITableModelContext
    {
        public bool CheckTable(SqliteConnection connection, string tableName)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND name='@TABLE_NAME'";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("TABLE_NAME", tableName);

            return command.ExecuteScalar() != null;
        }
        public void CreateTable(SqliteConnection connection, string createTabelCommand, string tableName)
        {
            try
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = createTabelCommand;
                command.CommandType = CommandType.Text;
                command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao criar a tabela " + tableName + ". Erro: " + ex.Message);
            }
        }
    }
}
