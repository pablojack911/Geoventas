using GeoventasPocho.DAO;
using GeoventasPocho.Vistas.ElementosMapa;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Data;

namespace GeoventasPocho.Controladores
{
    public class ControladorZonas
    {
        public static List<Zona> ObtenerZonas()
        {
            var zonas = new List<Zona>();
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    var query = @"SELECT codigo, empresa_rel, empresa, nombre FROM zonas WHERE activada = 1 AND (operator <> ' ' OR operator2 <> ' ') order by codigo";
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text);
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var zona = new Zona();
                            zona.Codigo = dr.GetString(0).Trim();
                            zona.CodigoEmpresa = dr.GetString(1).ToString().Trim();
                            zona.CodigoDivision = dr.GetString(2).Trim();
                            zona.Nombre = dr.GetString(3).Trim();
                            zonas.Add(zona);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return zonas;
        }

        public static List<Zona> ObtenerZonasDelVendedor(string codigoVendedor, DateTime date)
        {
            var diaVisita = DatetimeToDiaSemana.Convertir(date);
            var zonas = new List<Zona>();
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    var consulta = string.Format("SELECT zonas.codigo as zona, zonas.empresa_rel as empresa, zonas.empresa as division, zonas.nombre as nombre FROM zonas INNER JOIN cron_ped ON zonas.empresa_rel = cron_ped.empresa AND zonas.empresa = cron_ped.prov AND zonas.codigo=cron_ped.zona AND '{0}'$cron_ped.pedido WHERE zonas.activada=1 and (zonas.operator='{1}' or zonas.operator2 = '{1}') group by zonas.codigo", diaVisita, codigoVendedor);
                    var command = AccesoDB.CrearComando(connection, consulta, CommandType.Text);
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var zona = new Zona();
                            zona.Codigo = dr.GetString(0).Trim();
                            zona.CodigoEmpresa = dr.GetString(1).ToString().Trim();
                            zona.CodigoDivision = dr.GetString(2).Trim();
                            zona.Nombre = dr.GetString(3).Trim();
                            zonas.Add(zona);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return zonas;
        }

        public static void CargarZonasDelVendedor(Vendedor vendedor, DateTime date)
        {
            var codigoVendedor = vendedor.Codigo;
            var diaVisita = DatetimeToDiaSemana.Convertir(date);
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    var query = string.Format("SELECT zonas.codigo as zona, zonas.empresa_rel as empresa, zonas.empresa as division, zonas.nombre as nombre FROM zonas INNER JOIN cron_ped ON zonas.empresa_rel = cron_ped.empresa AND zonas.empresa = cron_ped.prov AND zonas.codigo=cron_ped.zona AND '{0}'$cron_ped.pedido WHERE zonas.activada=1 and (zonas.operator='{1}' or zonas.operator2 = '{1}') and zonas.empresa_rel = '{2}' group by zonas.codigo", diaVisita, codigoVendedor, vendedor.CodigoEmpresa);
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var zona = new Zona();
                            zona.Codigo = dr.GetString(0).Trim();
                            zona.CodigoEmpresa = dr.GetString(1).ToString().Trim();
                            zona.CodigoDivision = dr.GetString(2).Trim();
                            zona.Nombre = dr.GetString(3).Trim();
                            vendedor.Zonas.Add(zona);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<PointLatLng> ObtenerVerticesZona(string zona, string empresa, string division)
        {
            var lista = new List<PointLatLng>();
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    var query = string.Format("SELECT latitud, longitud FROM zonas_vertices where zona='{0}' and empresa='{1}' and subempresa='{2}'", zona, empresa, division);
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text);
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var lat = Convert.ToDouble(dr.GetValue(0), CultureInfo.CurrentCulture);
                            var lng = Convert.ToDouble(dr.GetValue(1), CultureInfo.CurrentCulture);
                            var vertice = new PointLatLng(lat, lng);
                            lista.Add(vertice);
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

        /// <summary>
        /// Sirve pero lo comento para no usarlo
        /// </summary>
        /// <param name="zona"></param>
        //public static List<Zona> ObtenerZonasPorEmpresa(string codigoEmpresa)
        //{
        //    return ControladorZonas.ObtenerZonas().Where(x => x.CodigoEmpresa == codigoEmpresa).ToList();
        //}

        public static void CargarClientes(Zona zona, ModoClientesRuteo modoVerClientes = ModoClientesRuteo.Todos)
        {
            try
            {
                zona.Clientes.Clear();
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {

                    var query = string.Empty;

                    switch (modoVerClientes)
                    {
                        case ModoClientesRuteo.ConRecorrido:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre, c.tipo_tarjeta = '03' as roja FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo INNER JOIN config_zona cz ON cz.cliente=c.codigo WHERE cz.zona='{0}' and cz.empresa='{1}' and cz.subempresa='{2}' and cz.baja=0 and c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 and cz.recorrido>0 ", zona.Codigo, zona.CodigoEmpresa, zona.CodigoDivision);
                            break;
                        case ModoClientesRuteo.SinRecorrido:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre, c.tipo_tarjeta = '03' as roja FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo INNER JOIN config_zona cz ON cz.cliente=c.codigo WHERE cz.zona='{0}' and cz.empresa='{1}' and cz.subempresa='{2}' and cz.baja=0 and c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 and cz.recorrido=0 ", zona.Codigo, zona.CodigoEmpresa, zona.CodigoDivision);
                            break;
                        case ModoClientesRuteo.Ninguno:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre, c.tipo_tarjeta = '03' as roja FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo INNER JOIN config_zona cz ON cz.cliente=c.codigo WHERE cz.zona='{0}' and cz.empresa='{1}' and cz.subempresa='{2}' and cz.baja=0 and c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 and cz.recorrido<0 ", zona.Codigo, zona.CodigoEmpresa, zona.CodigoDivision);
                            break;
                        case ModoClientesRuteo.Todos:
                        default:
                            query = string.Format("SELECT c.codigo, c.nombre, c.street, c.number, c.latitud, c.longitud, c.observ, cz.recorrido, r.nombre, c.tipo_tarjeta = '03' as roja FROM clientes c INNER JOIN ramos r ON c.ramo=r.codigo INNER JOIN config_zona cz ON cz.cliente=c.codigo WHERE cz.zona='{0}' and cz.empresa='{1}' and cz.subempresa='{2}' and cz.baja=0 and c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 ", zona.Codigo, zona.CodigoEmpresa, zona.CodigoDivision);
                            break;
                    }

                    query += "order by cz.recorrido";
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text);
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
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
                            cli.Roja = dr.GetBoolean(9);
                            zona.Clientes.Add(cli);
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
