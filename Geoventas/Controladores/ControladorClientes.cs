using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.DAO;
using GeoventasPocho.Vistas.ElementosMapa;
using GeoventasPocho.Vistas.Geocoder;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores
{
    public class ControladorClientes
    {
        public static List<Cliente> ObtenerClientes(string codigo)
        {
            //var lista = new List<Cliente>();
            string consulta = string.Empty;

            //try
            //{
            //FoxDB.Instancia.Conectar();
            //consulta = string.Format(@"SELECT cd.a,cd.b,cd.c,cli.localidad FROM s:\appvfp\hergo_release\clientes_domicilios cd inner join clientes cli on cd.a=cli.codigo"); //TEMPORAL!!
            //consulta = string.Format(@"SELECT cd.a,cd.b,cd.c,cli.localidad FROM s:\appvfp\hergo_release\clientes_domicilios cd inner join clientes cli on cd.a=cli.codigo WHERE (cd.b <> cli.street AND cd.c<> transform(cli.number) OR cli.Latitud=0)");
            //consulta = string.Format(@"SELECT cd.a,cd.b,cd.c,cli.Latitud, cli.Longitud,cli.localidad FROM \\server\work\appvfp\hergo_release\clientes_domicilios cd inner join clientes cli on cd.a=cli.codigo WHERE cli.Latitud=0");
            consulta = string.Format(@"SELECT cd.a,cd.b,cd.c,cli.Latitud, cli.Longitud,cli.localidad FROM \\server\work\appvfp\hergo_release\clientes_domicilios cd inner join clientes cli on cd.a=cli.codigo ");
            //consulta = string.Format(@"SELECT cd.a, cd.b, cd.c, cli.Latitud, cli.Longitud, cli.localidad FROM \\server\work\appvfp\hergo_release\clientes_domicilios cd inner join clientes cli on cd.a=cli.codigo WHERE (cd.b <> cli.street OR cd.c<> cli.number)");
            //consulta = string.Format(@"SELECT cd.a, cd.b, cd.c, cli.Latitud, cli.Longitud, cli.Localidad FROM \\server\work\appvfp\hergo_release\clientes_domicilios cd inner join clientes cli on cd.a = cli.codigo WHERE cli.latitud = -38.010178 OR cd.b <> cli.street OR cd.c <> cli.number");
            //SELECT cd.a,cd.b as calleCorregida,cli.street as calleActual,cd.c as numeroCorregido,cli.number as numeroActual,cli.localidad FROM s:\appvfp\hergo_release\clientes20160526 cd inner join clientes cli on cd.a=cli.codigo WHERE cd.b <> cli.street OR cd.c <> cli.number ORDER BY cd.b
            if (codigo != string.Empty)
                consulta += " AND cd.a > '" + codigo + "'";
            //consulta += " WHERE cd.a > '" + codigo + "'";

            consulta += " order by cd.a";
            return ProcesarConsulta(consulta);
        }

        public static List<Cliente> ObtenerDatosClientes(List<string> clientesGeo)
        {
            var clientes = new List<Cliente>();

            foreach (var cli in clientesGeo)
            {
                try
                {
                    using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                    {
                        var query = "select codigo, street, number, latitud, longitud, localidad from clientes where codigo='" + cli + "'";
                        var command = AccesoDB.CrearComando(connection, query, CommandType.Text);
                        connection.Open();
                        using (var dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var cliente = new Cliente();
                                cliente.Codigo = dr.GetString(0).Trim();
                                cliente.Calle = dr.GetString(1).Trim();
                                cliente.Numero = Convert.ToString(dr.GetValue(2));
                                var lat = Convert.ToDouble(dr.GetValue(3), CultureInfo.CurrentCulture);
                                var lng = Convert.ToDouble(dr.GetValue(4), CultureInfo.CurrentCulture);
                                cliente.Coordenada = new PointLatLng(lat, lng);
                                cliente.Localidad = dr.GetString(5).Trim();
                                clientes.Add(cliente);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return clientes;
        }

        public static List<TrackingFletero> ProcesarConsultaTracking(string consulta)
        {
            //var lista = new List<Cliente>();
            var lista = new List<TrackingFletero>();
            var listaEnCuadrante = new List<TrackingFletero>();
            var zona = new List<PointLatLng>()
            {
                new PointLatLng(-38.095119, -57.605652),
                new PointLatLng(-38.095119,-57.600030),
                new PointLatLng(-38.097078,-57.605652),
                new PointLatLng(-38.097078,-57.600030)
            };
            try
            {
                using (var connection = new SqlConnection(AccesoDB.SqlLogistica))
                {
                    var command = AccesoDB.CrearComando(connection, consulta, CommandType.Text);
                    connection.Open();
                    //AccesoDB.ComandosFox(connection);
                    using (var dr = command.ExecuteReader())
                    {
                        //while (dr.Read())
                        //{
                        //    var cliente = new Cliente();

                        //    cliente.Codigo = dr.GetString(0).Trim();
                        //    cliente.Calle = dr.GetString(1).Trim();
                        //    cliente.Numero = Convert.ToString(dr.GetValue(2));
                        //    var lat = Convert.ToDouble(dr.GetValue(3), CultureInfo.CurrentCulture);
                        //    var lng = Convert.ToDouble(dr.GetValue(4), CultureInfo.CurrentCulture);
                        //    cliente.Coordenada = new PointLatLng(lat, lng);
                        //    cliente.Localidad = dr.GetString(5).Trim();
                        //    lista.Add(cliente);
                        //}
                        while (dr.Read())
                        {
                            var Pos = new TrackingFletero()
                            {
                                Usuario = dr.GetString(1).Trim(),
                                Latitud = Convert.ToDouble(dr.GetValue(5), CultureInfo.CurrentCulture),
                                Longitud = Convert.ToDouble(dr.GetValue(6), CultureInfo.CurrentCulture),
                                Fecha = dr.GetDateTime(4)
                            };
                            lista.Add(Pos);
                        }
                    }
                    foreach (var item in lista)
                    {
                        if (ControladorMapa.DentroDeZona(zona, item.Latitud, item.Longitud))
                            listaEnCuadrante.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listaEnCuadrante;
        }

        public static List<Cliente> ProcesarConsulta(string consulta)
        {
            var lista = new List<Cliente>();
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    var command = AccesoDB.CrearComando(connection, consulta, CommandType.Text);
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var cliente = new Cliente();

                            cliente.Codigo = dr.GetString(0).Trim();
                            cliente.Calle = dr.GetString(1).Trim();
                            cliente.Numero = Convert.ToString(dr.GetValue(2));
                            var lat = Convert.ToDouble(dr.GetValue(3), CultureInfo.CurrentCulture);
                            var lng = Convert.ToDouble(dr.GetValue(4), CultureInfo.CurrentCulture);
                            cliente.Coordenada = new PointLatLng(lat, lng);
                            cliente.Localidad = dr.GetString(5).Trim();
                            lista.Add(cliente);
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

        public static bool ActualizarLatLngCliente(string codigo, PointLatLng coordenada, string calle, int nro)
        {
            var ok = false;
            var query = string.Empty;
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    query = string.Format(@"UPDATE clientes SET latitud =?, longitud =? where codigo = ?");
                    var lat = Math.Round(coordenada.Lat, 12, MidpointRounding.AwayFromZero);
                    var lng = Math.Round(coordenada.Lng, 12, MidpointRounding.AwayFromZero);
                    var parameters = new OleDbParameter[] { new OleDbParameter("@lat", lat), new OleDbParameter("@lng", lng), new OleDbParameter("@codigo", codigo) };
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text, parameters);
                    var res = command.ExecuteNonQuery();
                    if (res > 0)
                        ok = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ok;
        }

    }
}
