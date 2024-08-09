using DronePIProject.ModelContext.Database;
using DroneProject.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneProject.ModelContext.Database
{
    public class ErrorLogModelContext : DatabaseConnection, ITableModelContext
    {
        public string TABLE_NAME { get; private set; } = "ERROR_LOG";
        public const int MAX_REGISTER = 1000;
        public List<ErrorLogModel> ErrorLogList { get; set; } = new List<ErrorLogModel>();
        public ErrorLogModelContext()
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
                    string createTableCommand = "CREATE TABLE ERROR_LOG(" +
                        "ID    INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                        "ORIGEM    STRING  NOT NULL," +
                        "TIPO    STRING  NOT NULL," +
                        "DESCRICAO STRING  NOT NULL);";

                    ((ITableModelContext)this).CreateTable(Connection, createTableCommand, TABLE_NAME);
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao buscar o " + TABLE_NAME + " no BD: " + ex.Message);

            }
        }
        public List<ErrorLogModel> SelectAll()
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"SELECT * FROM ERROR_LOG";
                    command.CommandType = CommandType.Text;

                    ErrorLogList.Clear();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ErrorLogModel serialModel = new ErrorLogModel();
                            serialModel.Id = reader.GetInt32("ID");
                            serialModel.Origem = reader.GetString("ORIGEM");
                            serialModel.Tipo = reader.GetString("TIPO");
                            serialModel.Descricao = reader.GetString("DESCRICAO");

                            ErrorLogList.Add(serialModel);
                        }
                    }
                }
                Desconectar();
                return ErrorLogList;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao buscar o LOG no BD: " + ex.Message);

            }
        }
        public void Insert(ErrorLogModel errorLog)
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"INSERT INTO ERROR_LOG (TIPO, ORIGEM, DESCRICAO)" +
                        " VALUES (@TIPO, @ORIGEM, @DESCRICAO)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("TIPO", errorLog.Tipo);
                    command.Parameters.AddWithValue("ORIGEM", errorLog.Origem);
                    command.Parameters.AddWithValue("DESCRICAO", errorLog.Descricao);

                    int id = (int)command.ExecuteNonQuery();
                    errorLog.Id = id;
                    ErrorLogList.Add(errorLog);
                }
                Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um problema ao inserir o LOG no BD: " + ex.Message);
            }
            CleanUpTable(MAX_REGISTER);
        }
        public void DeleteById(int id)
        {
            try
            {
                Conectar();
                if (Connection.State == ConnectionState.Open)
                {
                    SqliteCommand command = Connection.CreateCommand();
                    command.CommandText = @"DELETE ERROR_LOG " +
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
                throw new Exception("Houve um problema ao remover o ERROR_LOG no BD: " + ex.Message); ;
            }
        }

        public void CleanUpTable(int maxNumOfRegisters)
        {
            if (ErrorLogList.Count > maxNumOfRegisters)
            {
                var listToRemove = ErrorLogList.OrderByDescending((x) => x.Id).Skip(maxNumOfRegisters).ToList();

                foreach (var item in listToRemove)
                {
                    DeleteById(item.Id);
                }
            }
        }

    }
}
