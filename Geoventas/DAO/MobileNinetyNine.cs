using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.DAO
{
    public sealed class MobileNinetyNine
    {
        //const string url = @"Data Source = SERVER-XEON\SQLEXPRESS; Initial Catalog = inteldev_mobile; Integrated Security = true;";
        public const string url = @"Data Source=tcp:192.168.1.96,1433;Initial Catalog=inteldev_mobile;User ID=sa;Password=hergo2017";
        private static readonly MobileNinetyNine instance = new MobileNinetyNine();
        private readonly SqlConnection connection = new SqlConnection(url);

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MobileNinetyNine()
        {
        }

        public MobileNinetyNine()
        {
        }

        public static MobileNinetyNine Instancia
        {
            get
            {
                return instance;
            }
        }

        private ConnectionState Estado
        {
            get
            {
                return Instancia.GetDBConnection().State;
            }
        }

        public SqlConnection GetDBConnection()
        {
            return connection;
        }

        private void EjecutarComando(string comando)
        {
            var command = this.CrearComando(comando);
            command.ExecuteNonQuery();
        }

        public SqlCommand CrearComando(string comando)
        {
            var command = Instancia.GetDBConnection().CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = comando;
            return command;
        }

        public void Conectar()
        {

            if (Estado == ConnectionState.Closed)
            {
                Instancia.GetDBConnection().Open();
            }
        }

        public void Desconectar()
        {
            if (Estado == ConnectionState.Open)
                Instancia.GetDBConnection().Close();
        }

    }

}
