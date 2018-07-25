using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.DAO
{
    public sealed class FoxDB
    {
        public const string Real = "Provider=VFPOLEDB.1;Data Source=////192.168.1.95//Work//preventa//datos//truesoft.dbc"; //real
        //const string url = "Provider=VFPOLEDB.1; Data Source=////server//work//AppVfp//Hergo_release//datos//truesoft.dbc"; //prueba
        private static readonly FoxDB instance = new FoxDB();
        private readonly OleDbConnection con = new OleDbConnection(Real);

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static FoxDB()
        {
        }

        public FoxDB()
        {
        }

        public static FoxDB Instancia
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

        public OleDbConnection GetDBConnection()
        {
            return con;
        }

        private void EjecutarComando(string comando)
        {
            var command = this.CrearComando(comando);
            command.ExecuteNonQuery();
        }

        public OleDbCommand CrearComando(string comando)
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
                EjecutarComando("SET NULL OFF");
                EjecutarComando("SET DELETED ON");
                EjecutarComando("SET ENGINEBEHAVIOR 70");
                EjecutarComando("SET multilocks ON");
                EjecutarComando("SET REPROCES TO AUTOMATIC");
            }
        }

        public void Desconectar()
        {
            if (Estado == ConnectionState.Open)
                Instancia.GetDBConnection().Close();
        }

        public OleDbCommand CrearStoredProcedure(string sp)
        {
            var command = Instancia.GetDBConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = sp;
            return command;
        }
    }
}
