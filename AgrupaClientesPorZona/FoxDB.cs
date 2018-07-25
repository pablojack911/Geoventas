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
        const string url = "Provider=VFPOLEDB.1;Data Source=////server//work//preventa//datos//truesoft.dbc"; //real
        //const string url = "Provider=VFPOLEDB.1; Data Source=////server//work//AppVfp//Hergo_release//datos//truesoft.dbc"; //prueba
        private static readonly FoxDB instance = new FoxDB();
        private readonly OleDbConnection con = new OleDbConnection(url);

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
            }
        }

        public void Desconectar()
        {
            if (Estado == ConnectionState.Open)
                Instancia.GetDBConnection().Close();
        }


    }
}
