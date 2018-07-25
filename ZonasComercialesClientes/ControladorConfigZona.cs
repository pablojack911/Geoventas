using GeoventasPocho.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonasComercialesClientes
{
    public class ControladorConfigZona
    {
        /// <summary>
        /// Actualizar el codigo de un registro de cliente en config_zona
        /// </summary>
        /// <param name="codigoCliente">para el cliente especifico</param>
        /// <param name="codigoZona">zona a actualizar</param>
        public static void ActualizarConfigZona(string codigoCliente, string codigoZona, string empresa, string subempresa)
        {
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("update config_zona set zona='{0}' where cliente='{3}' and empresa='{1}' and subempresa='{2}' ", codigoZona, empresa, subempresa, codigoCliente));
                FoxDB.Instancia.Conectar();
                command.ExecuteNonQuery();
                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserta un cliente en la tabla config_zona, asignandolo a una zona comercial
        /// </summary>
        /// <param name="codigoCliente">cliente</param>
        /// <param name="codigoZona">zona comercial</param>
        /// <param name="empresa">empresa</param>
        /// <param name="subempresa">subempresa</param>
        public static void InsertarConfigZona(string codigoCliente, string codigoZona, string empresa, string subempresa)
        {
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("insert into config_zona(cliente,zona,empresa,subempresa,recorrido) values ('{0}','{1}','{2}','{3}',{4})", codigoCliente, codigoZona, empresa, subempresa, 1));
                FoxDB.Instancia.Conectar();
                command.ExecuteNonQuery();
                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Consulta si existe o no el cliente en config_zona
        /// </summary>
        /// <param name="codigoCliente">codigo del cliente</param>
        /// <returns>cantidad de veces que aparece el cliente en config_logis</returns>
        public static int ConsultaCliente(string codigoCliente, string empresa, string subempresa)
        {
            int res = 0;
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("select * from config_zona where cliente='{0}' and empresa='{1}' and subempresa='{2}'", codigoCliente, empresa, subempresa));
                FoxDB.Instancia.Conectar();
                res = command.ExecuteNonQuery();
                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
    }
}
