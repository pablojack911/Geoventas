using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoventasPocho.Vistas.ElementosMapa;
using GeoventasPocho.DAO;
using System.Globalization;
using System.Data;
using GMap.NET;
using MoreLinq;

namespace GeoventasPocho.Controladores
{
    public class ControladorFleteros
    {
        public static void ActualizarCoordenadaDomicilio(Fletero f)
        {
            throw new NotImplementedException();
        }

        //public static void CargarClientes(Fletero fletero, DateTime today)
        //{
        //    CargarClientesDelFletero(fletero, today);
        //}

        //public static void CargarPosiciones(Fletero flet, DateTime fechaDesde, DateTime fechaHasta)
        //{
        //    flet.Posiciones.Clear();
        //    Func<object, dynamic, object> nonull = (p, def) => p == null ? def : p;
        //    try
        //    {
        //        FoxDB.Instancia.Conectar();
        //        var consulta = FoxDB.Instancia.CrearComando("SELECT cliente,estado,latitud,longitud,fecha FROM trackingfletero WHERE usuario=? AND BETWEEN(fecha,?,?) group by latitud,longitud,cliente ORDER BY fecha");
        //        consulta.Parameters.Add(new System.Data.OleDb.OleDbParameter("@fletero", flet.Codigo));
        //        consulta.Parameters.Add(new System.Data.OleDb.OleDbParameter("@fdesde", fechaDesde));
        //        consulta.Parameters.Add(new System.Data.OleDb.OleDbParameter("@fhasta", fechaHasta));
        //        var dr = consulta.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            var pos = new Posicion();
        //            pos.Cliente = dr.GetString(0);
        //            pos.MotivoVisita = (MotivoVisita)Convert.ToInt32(dr.GetValue(1));
        //            pos.Latitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
        //            pos.Longitud = Convert.ToDouble(dr.GetValue(3), CultureInfo.CurrentCulture);
        //            pos.Fecha = dr.GetDateTime(4);
        //            flet.Posiciones.Add(pos);
        //        }
        //        //if (dr.Read())
        //        //{
        //        //    var pos = new Posicion();
        //        //    pos.Cliente = dr.GetString(0);
        //        //    pos.MotivoVisita = (MotivoVisita)dr.GetInt32(1);
        //        //    pos.Latitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
        //        //    pos.Longitud= Convert.ToDouble(dr.GetValue(3), CultureInfo.CurrentCulture);
        //        //    pos.Fecha = new DateTime()
        //        //    vendedor.CodigoDivision = dr.GetString(2).Trim();
        //        //    vendedor.Calle = dr.GetString(3).Trim();
        //        //    vendedor.Numero = dr.GetString(4).Trim();
        //        //    var Latitud = Convert.ToDouble(dr.GetValue(5), CultureInfo.CurrentCulture);
        //        //    var Longitud = Convert.ToDouble(dr.GetValue(6), CultureInfo.CurrentCulture);
        //        //    vendedor.CoordenadaDomicilio = new PointLatLng(Latitud, Longitud);
        //        //    vendedor.Foto = dr.GetString(7);
        //        //vendedor.ZonasAsignadas = ControladorZonas.ObtenerZonasDelVendedor(vendedor.Codigo, DatetimeToDiaSemana.Convertir(DateTime.Today));
        //        //}

        //        var clientesVisitados = flet.Posiciones.Where(p => (p.Cliente != "" && p.Cliente != "VIAJE") && p.MotivoVisita != MotivoVisita.Pendiente).DistinctBy(x => x.Cliente);
        //        flet.Visitados = clientesVisitados.Count();

        //        FoxDB.Instancia.Desconectar();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static void CargarPosiciones(Fletero flet, DateTime fechaDesde, DateTime fechaHasta)
        {
            flet.Posiciones.Clear();
            Func<object, dynamic, object> nonull = (p, def) => p == null ? def : p;
            try
            {
                var consulta = MobileNinetySix.Instancia.CrearComando("SELECT cliente,estado,latitud,longitud,fecha FROM trackingfletero WHERE usuario=? AND BETWEEN(fecha,?,?) group by latitud,longitud,cliente ORDER BY fecha");
                consulta.Parameters.Add(new System.Data.SqlClient.SqlParameter("@fletero", flet.Codigo));
                consulta.Parameters.Add(new System.Data.SqlClient.SqlParameter("@fdesde", fechaDesde));
                consulta.Parameters.Add(new System.Data.SqlClient.SqlParameter("@fhasta", fechaHasta));
                MobileNinetySix.Instancia.Conectar();
                using (var dr = consulta.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var pos = new Posicion();
                        pos.Cliente = dr.GetString(0);
                        pos.MotivoVisita = (MotivoVisita)Convert.ToInt32(dr.GetValue(1));
                        pos.Latitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
                        pos.Longitud = Convert.ToDouble(dr.GetValue(3), CultureInfo.CurrentCulture);
                        pos.Fecha = dr.GetDateTime(4);
                        flet.Posiciones.Add(pos);
                    }
                }
                var clientesVisitados = flet.Posiciones.Where(p => (p.Cliente != "" && p.Cliente != "VIAJE") && p.MotivoVisita != MotivoVisita.Pendiente).DistinctBy(x => x.Cliente);
                flet.Visitados = clientesVisitados.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MobileNinetySix.Instancia.Desconectar();
            }
        }

        public static Fletero ObtenerFletero(string codigo)
        {
            var flet = new Fletero();
            try
            {
                FoxDB.Instancia.Conectar();
                var consulta = FoxDB.Instancia.CrearComando(string.Format("SELECT codigo,nombre,domicilio FROM proveedo WHERE fletero=1 AND habilitado=1 and codigo='{0}'", codigo));
                var dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    flet.Codigo = codigo;
                    flet.Nombre = dr.GetString(0).Trim();
                    flet.Domicilio = dr.GetString(1).Trim();
                }
                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flet;
        }

        //public static List<Fletero> ObtenerUltimaPosicionFleteros(DateTime fecha)
        //{
        //    List<Fletero> fleteros = new List<Fletero>();
        //    try
        //    {
        //        FoxDB.Instancia.Conectar();
        //        var consulta = FoxDB.Instancia.CrearComando("SELECT usuario, latitud, longitud, MAX(fecha)FROM trackingfletero WHERE fecha >= ? GROUP BY usuario ORDER BY fecha desc");
        //        consulta.Parameters.Add(new System.Data.OleDb.OleDbParameter("@fecha", fecha));
        //        var dr = consulta.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            var flet = new Fletero();
        //            flet.Codigo = dr.GetString(0).ToString();
        //            var latitud = Convert.ToDouble(dr.GetValue(1), CultureInfo.CurrentCulture);
        //            var longitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
        //            flet.CoordenadaActual = new GMap.NET.PointLatLng(latitud, longitud);
        //            flet.Fecha = dr.GetDateTime(3).ToString();
        //            fleteros.Add(flet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return fleteros;
        //}

        public static List<Fletero> ObtenerUltimaPosicionFleteros(DateTime fecha)
        {
            List<Fletero> fleteros = new List<Fletero>();
            try
            {
                var consulta = MobileNinetySix.Instancia.CrearComando("SELECT usuario, latitud, longitud, MAX(fecha) FROM trackingfletero WHERE fecha >= ? GROUP BY usuario ORDER BY fecha desc");
                consulta.Parameters.Add(new System.Data.SqlClient.SqlParameter("@fecha", fecha));
                MobileNinetySix.Instancia.Conectar();
                using (var dr = consulta.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var flet = new Fletero();
                        flet.Codigo = dr.GetString(0).ToString();
                        var latitud = Convert.ToDouble(dr.GetValue(1), CultureInfo.CurrentCulture);
                        var longitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
                        flet.CoordenadaActual = new GMap.NET.PointLatLng(latitud, longitud);
                        flet.Fecha = dr.GetDateTime(3).ToString();
                        fleteros.Add(flet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MobileNinetySix.Instancia.Desconectar();
            }
            return fleteros;
        }

        //SELECT distinc comision.cliente,clientes.nombre,clientes.domicilio, SUM(IIF(comision.condvta='01',final,cast(0 as n(10,2)))) as contado,SUM(IIF(comision.condvta='01',cast(0 as n(10,2)),final)) as ctacte  FROM comision INNER JOIN (SELECT reparto FROM reparmad WHERE fletero='{0}') as crsMov ON crsmov.reparto=comision.reparto INNER JOIN clientes ON clientes.codigo=comision.cliente GROUP BY cliente

        public static void CargarClientesDelFletero(Fletero flet, DateTime today)
        {
            try
            {
                flet.Clientes.Clear();
                FoxDB.Instancia.Conectar();
                var consulta = FoxDB.Instancia.CrearStoredProcedure("clientesfletero");
                consulta.Parameters.Add(new System.Data.OleDb.OleDbParameter("@fletero", flet.Codigo));
                consulta.Parameters.Add(new System.Data.OleDb.OleDbParameter("@fecha", today));

                var dr = consulta.ExecuteReader();

                while (dr.Read())
                {
                    var cli = new ClienteFletero();
                    cli.Codigo = dr.GetString(0);
                    cli.Nombre = dr.GetString(1).Trim();
                    cli.Calle = dr.GetString(2).Trim();
                    cli.Numero = dr.GetValue(3).ToString();
                    var Latitud = Convert.ToDouble(dr.GetValue(4), CultureInfo.CurrentCulture);
                    var Longitud = Convert.ToDouble(dr.GetValue(5), CultureInfo.CurrentCulture);
                    cli.Coordenada = new PointLatLng(Latitud, Longitud);
                    cli.Observacion = dr.GetString(6).Trim();
                    cli.OrdenRecorrido = 0;
                    cli.Actividad = dr.GetString(7).Trim();
                    cli.Roja = dr.GetBoolean(8);
                    cli.Contado = Convert.ToDecimal(dr.GetValue(9));
                    cli.CtaCte = Convert.ToDecimal(dr.GetValue(10));
                    flet.Clientes.Add(cli);
                }
                FoxDB.Instancia.Desconectar();
                flet.CantidadClientes = flet.Clientes.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FoxDB.Instancia.Desconectar();
            }
        }
    }
}
