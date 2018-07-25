using GeoventasPocho.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonasLogisticasClientes
{
    public class ControladorZonasLogis
    {
        /// <summary>
        /// Insertar nueva zona logistica. Caso 1 - Hoja 1
        /// </summary>
        /// <param name="codigo">Codigo</param>
        /// <param name="nombre">Nombre</param>
        public static void AgregarZonaLogistica(string codigo,string nombre)
        {
            try
            {
                var command = FoxDB.Instancia.CrearComando(string.Format("insert into zonas_logis(codigo,nombre) values('{0}','{1}')", codigo, nombre));
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
