using GeoventasPocho.DAO;
using GeoventasPocho.Vistas.ElementosMapa;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores
{
    public class ControladorActividades
    {
        public static List<Actividad> ObtenerActividades()
        {
            var lista = new List<Actividad>();

            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    var query = "SELECT codigo, nombre FROM ramos order by nombre";
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text);
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var actividad = new Actividad();
                            actividad.Codigo = dr.GetString(0).Trim();
                            actividad.Nombre = dr.GetString(1).Trim();
                            actividad.Clientes = new List<Cliente>();
                            lista.Add(actividad);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public static void CargarClientes(Actividad actividad, ModoClientesRuteo modoVerClientes)
        {
            try
            {
                actividad.Clientes.Clear();
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    var query = string.Empty;
                    switch (modoVerClientes)
                    {
                        case ModoClientesRuteo.ConRecorrido:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo INNER JOIN config_zona cz ON cz.cliente = c.codigo WHERE c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 AND cz.baja=0 AND cz.zona<>' ' and cz.recorrido>0 and r.Codigo = '{0}'", actividad.Codigo);
                            break;
                        case ModoClientesRuteo.SinRecorrido:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo INNER JOIN config_zona cz ON cz.cliente = c.codigo WHERE c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 AND cz.baja=0 AND cz.zona<>' ' AND cz.recorrido=0 and r.Codigo = '{0}'", actividad.Codigo);
                            break;
                        case ModoClientesRuteo.Ninguno:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo INNER JOIN config_zona cz ON cz.cliente = c.codigo WHERE c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 AND cz.baja=0 AND cz.zona<>' ' AND cz.recorrido<0 and r.Codigo = '{0}'", actividad.Codigo);
                            break;
                        case ModoClientesRuteo.Todos:
                        default:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo  INNER JOIN config_zona cz ON cz.cliente = c.codigo WHERE c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 AND cz.baja=0 AND cz.zona<>' ' and r.Codigo = '{0}'", actividad.Codigo);
                            break;
                    }

                    query += "group by c.codigo order by cz.recorrido";
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var cli = new Cliente();
                            cli.Codigo = dr.GetString(0);
                            cli.Nombre = dr.GetString(1).Trim();
                            cli.Calle = dr.GetString(2).Trim();
                            cli.Numero = dr.GetValue(3).ToString();
                            var Latitud = Convert.ToDouble(dr.GetValue(4), CultureInfo.CurrentCulture);
                            var Longitud = Convert.ToDouble(dr.GetValue(5), CultureInfo.CurrentCulture);
                            cli.Coordenada = new PointLatLng(Latitud, Longitud);
                            cli.Observacion = dr.GetString(6).Trim();
                            cli.OrdenRecorrido = Convert.ToInt32(dr.GetValue(7));
                            cli.Actividad = dr.GetString(8).Trim();
                            actividad.Clientes.Add(cli);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
