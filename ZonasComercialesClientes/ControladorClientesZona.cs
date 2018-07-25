using GeoventasPocho.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonasComercialesClientes
{
    public class ControladorClientesZona
    {
        /// <summary>
        /// Obtiene todos los clientes asignados a una zona comercial especifica
        /// </summary>
        /// <param name="codigoZonaComer">zona comercial</param>
        /// <returns>lista de clientes</returns>
        public static List<string> ObtenerClientesPorZonaComercial(string codigoZonaComer)
        {
            var lista = new List<string>();

            try
            {
                FoxDB.Instancia.Conectar();

                var consulta = string.Format("SELECT distinct(c.codigo) FROM clientes c INNER JOIN config_zona cz ON cz.cliente=c.codigo WHERE cz.zona='{0}' and cz.baja=0 and c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 and cz.recorrido>0 order by cz.recorrido", codigoZonaComer);

                var dr = FoxDB.Instancia.CrearComando(consulta).ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(dr.GetString(0).Trim());
                }

                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }
    }
}
