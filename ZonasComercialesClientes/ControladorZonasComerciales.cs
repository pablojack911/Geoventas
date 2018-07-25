using GeoventasPocho.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonasComercialesClientes
{
    public class ControladorZonasComerciales
    {
        /// <summary>
        /// Insertar nueva zona comercial. 
        /// </summary>
        /// <param name="codigo">Codigo</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="empresa">Codigo de la division</param>
        /// <param name="empresa_rel">codigo de la empresa</param>
        /// <param name="operador">Codigo del preventista</param>
        /// <param name="region">Codigo de la region</param>
        /// <param name="ciudad">CP de la ciudad</param>
        public static void AgregarZonaComercial(string codigo, string nombre, string empresa, string empresa_rel, string operador, string region, string ciudad)
        {
            var activada = 1;
            var aceptaped = 1;
            var recargo = 1;
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("insert into zonas(codigo,nombre,empresa,empresa_rel,operator,region,ciudad,activada,aceptaped,recargo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9})", codigo, nombre, empresa, empresa_rel, operador, region, ciudad, activada, aceptaped, recargo));
                FoxDB.Instancia.Conectar();
                command.ExecuteNonQuery();
                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
