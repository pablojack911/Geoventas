using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.DAO
{
    public class AccesoDB
    {
        public const string FoxPreventaReal = "Provider=VFPOLEDB.1;Data Source=////192.168.1.95//Work//preventa//datos//truesoft.dbc"; //real
        public const string SqlPreventa = @"Data Source=tcp:192.168.1.96,1433;Initial Catalog=inteldev_mobile;User ID=sa;Password=hergo2017";
        public const string SqlLogistica = @"Data Source=tcp:192.168.1.96,1433;Initial Catalog=Logistica;User ID=sa;Password=hergo2017";


        public static DbCommand CrearComando(DbConnection connection, string comando, CommandType tipoComando, params DbParameter[] parameters) 
        {
            var command = connection.CreateCommand();
            command.CommandType = tipoComando;
            command.CommandText = comando;
            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);
            return command;
        }

        public static void ComandosFox(DbConnection connection)
        {
            EjecutarComando(connection, "SET NULL OFF");
            EjecutarComando(connection, "SET DELETED ON");
            EjecutarComando(connection, "SET ENGINEBEHAVIOR 70");
            EjecutarComando(connection, "SET multilocks ON");
            EjecutarComando(connection, "SET REPROCES TO AUTOMATIC");
        }

        private static void EjecutarComando(DbConnection connection, string comando)
        {
            var command = CrearComando(connection, comando, CommandType.Text);
            command.ExecuteNonQuery();
        }
    }
}
