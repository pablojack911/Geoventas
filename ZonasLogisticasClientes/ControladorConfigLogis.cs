using GeoventasPocho.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonasLogisticasClientes
{
    public class ControladorConfigLogis
    {
        /// <summary>
        /// Actualizar el codigo de un registro de cliente en config_logis
        /// </summary>
        /// <param name="codigoZona">zona a actualizar</param>
        /// <param name="codigoCliente">para el cliente especifico</param>
        public static void ActualizarConfigLogis(string codigoZona, string codigoCliente)
        {
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("update config_logis set zona='{0}' where cliente='{1}'", codigoZona, codigoCliente));
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
        /// Inserta un cliente en la tabla config_logis, asignandolo a una zona logistica
        /// </summary>
        /// <param name="codigoZona">zona logistica</param>
        /// <param name="codigoCliente">cliente</param>
        public static void InsertarConfigLogis(string codigoZona, string codigoCliente)
        {
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("insert into config_logis(zona,cliente) values ('{0}','{1}')", codigoZona, codigoCliente));
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
        /// Consulta si existe o no el cliente en config_logis
        /// </summary>
        /// <param name="codigoCliente">codigo del cliente</param>
        /// <returns>cantidad de veces que aparece el cliente en config_logis</returns>
        public static int ConsultaCliente(string codigoCliente)
        {
            int res = 0;
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("select * from config_logis where cliente='{0}'", codigoCliente));
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
