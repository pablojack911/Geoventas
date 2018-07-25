using GeoventasPocho.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonasComercialesClientes
{
    public class ControladorCronPed
    {
        /// <summary>
        /// Actualiza el cronograma de visita de una zona en particular
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="empresa"></param>
        /// <param name="prov"></param>
        /// <param name="pedido"></param>
        /// <param name="entrega"></param>
        public static void ActualizaCronPed(string zona, string empresa, string prov, string pedido, string entrega)
        {
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("update cron_ped set pedido='{0}', entrega='{1}' where zona='{2}' and empresa='{3}' and prov='{4}'", pedido, entrega, zona, empresa, prov));
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
        /// Agrega un registro de cronograma de pedido de una zona en particular
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="empresa"></param>
        /// <param name="prov"></param>
        /// <param name="pedido"></param>
        /// <param name="entrega"></param>
        public static void InsertarCronPed(string zona, string empresa, string prov, string pedido, string entrega)
        {
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("insert into cron_ped(zona,empresa,prov,pedido,entrega) values ('{0}','{1}','{2}','{3}','{4}')", zona, empresa, prov, pedido, entrega));
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
        /// Consulta si existe una zona en cron_ped
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="empresa"></param>
        /// <param name="prov"></param>
        /// <returns></returns>
        public static int ConsultaCronPed(string zona, string empresa, string prov)
        {
            int res = 0;
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("select * from cron_ped where zona='{0}' and empresa='{1}' and prov='{2}'", zona, empresa, prov));
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
