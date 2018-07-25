using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores
{
    public class ControladoraDB
    {

        public static DbCommand CrearComando(DbConnection connection, string comando)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = comando;
            return command;
        }

        public static DbCommand CrearStoredProcedure(DbConnection connection, string sp)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = sp;
            return command;
        }

        public static void Conectar(DbConnection connection)
        {

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                EjecutarComando(connection, "SET NULL OFF");
                EjecutarComando(connection, "SET DELETED ON");
                EjecutarComando(connection, "SET ENGINEBEHAVIOR 70");
            }
        }

        private static void EjecutarComando(DbConnection connection, string comando)
        {
            var command = CrearComando(connection, comando);
            command.ExecuteNonQuery();
        }        

        public static void Desconectar(DbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
}
