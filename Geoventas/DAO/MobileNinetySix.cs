using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.DAO
{
    public sealed class MobileNinetySix
    {
        public const string Path = @"Data Source=tcp:192.168.1.96,1433;Initial Catalog=Logistica;User ID=sa;Password=hergo2017";
        private static readonly MobileNinetySix instance = new MobileNinetySix();
        private readonly SqlConnection connection = new SqlConnection(Path);

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MobileNinetySix()
        {
        }

        public MobileNinetySix()
        {
        }

        public static MobileNinetySix Instancia
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
