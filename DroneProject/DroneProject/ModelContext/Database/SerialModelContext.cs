using DronePIProject.ModelContext.Database;
using DroneProject.Models;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;

namespace DroneProject.ModelContext.Database
{
    internal class SerialModelContext : DatabaseConnection, ITableModelContext
    {
        public List<SerialModel> SerialList { get; set; } = new List<SerialModel>();
        public string TABLE_NAME { get; private set; } = "SERIAL";

        public SerialModelContext()
        {
            Initialize();
            SelectAll();
        }
        public void Initialize()
        {
            try
            {
                Conectar();
                if (((ITableModelContext)this).CheckTable(Connection, TABLE_NAME))
                {
                    string createTableCommand = "CREATE TABLE SERIAL (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                        "NOME STRING NOT NULL," +
                        "BAUD_RATE INTEGER NOT NULL," +
                        "PORT_COM STRING NOT NULL);";

                    ((ITableModelContext)this).CreateTable(Connection, createTableCommand, TABLE_NAME);
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao buscar o " + TABLE_NAME + " no BD: " + ex.Message);

            }
        }
        public List<SerialModel> SelectAll()
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"SELECT * FROM " + TABLE_NAME;
                    command.CommandType = CommandType.Text;

                    SerialList.Clear();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SerialModel serialModel = new SerialModel();
                            serialModel.Id = reader.GetInt32("ID");
                            serialModel.Nome = reader.GetString("NOME");
                            serialModel.BaudRate = reader.GetInt32("BAUD_RATE");
                            serialModel.PortCOM = reader.GetString("PORT_COM");

                            SerialList.Add(serialModel);
                        }
                    }
                }
                Desconectar();
                return SerialList;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao buscar o "+ TABLE_NAME + " no BD: " + ex.Message);

            }
        }

        public void Insert(SerialModel serialModel)
        {
            try
            {
                Conectar();
                if (Connection.State == System.Data.ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"INSERT INTO "+ TABLE_NAME + " (NOME, BAUD_RATE, PORT_COM)" +
                        " VALUES (@NOME, @BAUD_RATE, @PORT_COM)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("NOME", serialModel.Nome);
                    command.Parameters.AddWithValue("BAUD_RATE", serialModel.BaudRate);
                    command.Parameters.AddWithValue("PORT_COM", serialModel.PortCOM);

                    int id = (int)command.ExecuteScalar();
                    serialModel.Id = id;
                    SerialList.Add(serialModel);
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao inserir o "+ TABLE_NAME + " no BD: " + ex.Message);
            }
        }
        public void Update(SerialModel serialModel)
        {
            try
            {
                Conectar();
                if (Connection.State == System.Data.ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"UPDATE "+ TABLE_NAME + " SET " +
                        "NOME = @NOME, " +
                        "BAUD_RATE = @BAUD_RATE, " + 
                        "PORT_COM = @PORT_COM" +
                        " WHERE ID = @ID";
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("NOME", serialModel.Nome);
                    command.Parameters.AddWithValue("BAUD_RATE", serialModel.BaudRate);
                    command.Parameters.AddWithValue("PORT_COM", serialModel.PortCOM);

                    command.Parameters.AddWithValue("ID", serialModel.Id);

                    command.ExecuteNonQuery();

                    SelectAll();
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao atualizar o "+ TABLE_NAME + " no BD: " + ex.Message); ;
            }
        }

        public void DeleteById(int id)
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"DELETE "+ TABLE_NAME +
                        " WHERE ID = @ID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("ID", id);

                    command.ExecuteNonQuery();

                    SelectAll();
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao remover o "+ TABLE_NAME + " no BD: " + ex.Message); ;
            }
        }

        public SerialModel GetSerialByName(string name)
        {
            var serial = SerialList.FirstOrDefault((x) => x.Nome == name);
            if (serial == null)
            {
                SelectAll();
                serial = SerialList.FirstOrDefault((x) => x.Nome == name);
                if (serial == null)
                {
                    throw new Exception("O "+ TABLE_NAME + " do "+ name + " não foi localizado!");
                }
            }

            return serial;
        }
    }
}
