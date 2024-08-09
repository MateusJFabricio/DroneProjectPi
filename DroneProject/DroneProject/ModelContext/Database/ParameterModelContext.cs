using DronePIProject.ModelContext.Database;
using DroneProject.Models;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Net.Http.Headers;
using System.Text;

namespace DroneProject.ModelContext.Database
{
    internal class ParameterModelContext : DatabaseConnection, ITableModelContext
    {
        public string TABLE_NAME { get; private set; } = "PARAMETER";
        public List<ParametroModel> ParameterlList { get; set; } = new List<ParametroModel>();
        public ParameterModelContext()
        {
            Initialize();
            SelectAll();
        }
        public void Initialize()
        {
            try
            {
                Conectar();
                if (!((ITableModelContext)this).CheckTable(Connection, TABLE_NAME))
                {
                    string createTableCommand = "CREATE TABLE PARAMETER(" +
                        "ID    INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                        "KEY   STRING  NOT NULL," +
                        "VALUE STRING  NOT NULL);";

                    ((ITableModelContext)this).CreateTable(Connection, createTableCommand, TABLE_NAME);
                    Insert(new ParametroModel { Key = "DRONE_NOME", Value = "Inferno" });
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao buscar o " + TABLE_NAME + " no BD: " + ex.Message);

            }
        }
        public List<ParametroModel> SelectAll()
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"SELECT * FROM PARAMETER";
                    command.CommandType = CommandType.Text;

                    ParameterlList.Clear();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ParametroModel parameterModel = new()
                            {
                                Id = reader.GetInt32("ID"),
                                Key = reader.GetString("KEY"),
                                Value = reader.GetString("VALUE")
                            };

                            ParameterlList.Add(parameterModel);
                        }
                    }
                }
                Desconectar();
                return ParameterlList;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao buscar o PARAMETER no BD: " + ex.Message);

            }
        }
        public void Insert(ParametroModel parameterModel)
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"INSERT INTO PARAMETER (KEY, VALUE)" +
                        " VALUES (@KEY, @VALUE)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("KEY", parameterModel.Key);
                    command.Parameters.AddWithValue("VALUE", parameterModel.Value);

                    int id = (int)command.ExecuteNonQuery();
                    parameterModel.Id = id;
                    ParameterlList.Add(parameterModel);
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao inserir o PARAMETER no BD: " + ex.Message);
            }
        }
        public void Update(ParametroModel parameterModel)
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"UPDATE PARAMETER SET " +
                        "KEY = @KEY, " +
                        "VALUE = @VALUE" +
                        " WHERE ID = @ID";
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("KEY", parameterModel.Key);
                    command.Parameters.AddWithValue("VALUE", parameterModel.Value);

                    command.Parameters.AddWithValue("ID", parameterModel.Id);

                    command.ExecuteNonQuery();

                    SelectAll();
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao atualizar o PARAMETER no BD: " + ex.Message); ;
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
                    command.CommandText = @"DELETE PARAMETER " +
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
                throw new Exception("Houve um problema ao remover o PARAMETER no BD: " + ex.Message); ;
            }
        }
        public ParametroModel GetParameterByKey(string key)
        {
            var parameter = ParameterlList.FirstOrDefault((x) => x.Key == key);
            if (parameter == null)
            {
                SelectAll();
                parameter = ParameterlList.FirstOrDefault((x) => x.Key == key);
                if (parameter == null)
                {
                    throw new Exception("O PARAMETER do "+ key + " não foi localizado!");
                }
            }

            return parameter;
        }
    }
}
