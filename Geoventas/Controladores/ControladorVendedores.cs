using GeoventasPocho.DAO;
using GeoventasPocho.Vistas.ElementosMapa;
using GMap.NET;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace GeoventasPocho.Controladores
{
    public class ControladorVendedores
    {
        private static PointLatLng ultimaCoordenadaConocida = new PointLatLng() { Lat = -38.002601, Lng = -57.601849 };
        private static Func<object, dynamic, object> nonull = (p, def) => p == null ? def : p;

        public static Vendedor ObtenerVendedor(string codigo)
        {
            var vendedor = new Vendedor();
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    var consulta = "select nombre,empresa,prov,calle,numero,latitud,longitud,foto from operator where codigo='" + codigo + "' and cargo=1 and mobile=1";
                    var command = AccesoDB.CrearComando(connection, consulta, CommandType.Text);
                    connection.Open();
                    AccesoDB.ComandosFox(connection);
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            vendedor.Codigo = codigo;
                            vendedor.Nombre = dr.GetString(0).Trim();
                            vendedor.CodigoEmpresa = dr.GetString(1).Trim();
                            vendedor.CodigoDivision = dr.GetString(2).Trim();
                            vendedor.Calle = dr.GetString(3).Trim();
                            vendedor.Numero = dr.GetString(4).Trim();
                            var Latitud = Convert.ToDouble(dr.GetValue(5), CultureInfo.CurrentCulture);
                            var Longitud = Convert.ToDouble(dr.GetValue(6), CultureInfo.CurrentCulture);
                            vendedor.CoordenadaDomicilio = new PointLatLng(Latitud, Longitud);
                            vendedor.Foto = dr.GetString(7);
                            //vendedor.ZonasAsignadas = ControladorZonas.ObtenerZonasDelVendedor(vendedor.Codigo, DatetimeToDiaSemana.Convertir(DateTime.Today));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vendedor;
        }

        public static List<string> ObtenerCodigosVendedores(string empresa = null)
        {
            var listaCodigosVendedores = new List<string>();
            try
            {
                using (var connection = new SqlConnection(AccesoDB.SqlPreventa))
                {
                    SqlParameter[] parameter = new SqlParameter[1];
                    var q = "select usuario from vendedores where borrado=0";
                    if (empresa != null)
                    {
                        q += @" and empresa=@pEmpresa";
                        parameter[0] = new SqlParameter("@pEmpresa", empresa);
                    }
                    else
                        parameter = null;
                    var command = AccesoDB.CrearComando(connection, q, CommandType.Text, parameter);
                    connection.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaCodigosVendedores.Add(dr.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener vendedores. " + ex.Message + "\n\nCtrlVend_ObtenerCodigosVendedores");
            }
            return listaCodigosVendedores;

        }
        public static List<Vendedor> ObtenerVendedoresPorFecha(DateTime fecha)
        {
            if (fecha == null)
                fecha = DateTime.Today;

            var vendedoresPresentes = new List<string>();

            try
            {
                using (var connection = new SqlConnection(AccesoDB.SqlPreventa))
                {
                    SqlParameter[] parameters = { new SqlParameter("@pFechaDesde", fecha), new SqlParameter("@pFechaHasta", fecha.AddDays(1)) };
                    var query = @"select p.usuario
                       from posiciongps p
                       inner join vendedores v on p.usuario = v.usuario
                       where fecha >= @pFechaDesde and fecha < @pFechaHasta and v.borrado = 0
                       group by p.usuario";
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text, parameters);

                    connection.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            vendedoresPresentes.Add(dr.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener vendedores. " + ex.Message + "\n\nCtrlVend_ObtenerVendedoresPorFecha");
            }

            var vendedores = new List<Vendedor>();
            foreach (var codigo in vendedoresPresentes)
            {
                var vendedor = ControladorVendedores.ObtenerVendedor(codigo);
                ControladorZonas.CargarZonasDelVendedor(vendedor, fecha);
                vendedores.Add(vendedor);
            }

            return vendedores;
        }


        public static List<Vendedor> ObtenerVendedoresPorFecha(string empresa, DateTime fecha)
        {
            if (fecha == null)
                fecha = DateTime.Today;

            var vendedoresPresentes = new List<string>();

            try
            {
                using (var connection = new SqlConnection(AccesoDB.SqlPreventa))
                {
                    SqlParameter[] parameters = { new SqlParameter("@pFechaDesde", fecha), new SqlParameter("@pFechaHasta", fecha.AddDays(1)), new SqlParameter("@pEmpresa", empresa) };
                    var query = @"select p.usuario
                       from posiciongps p
                       inner join vendedores v on p.usuario = v.usuario
                       where fecha >= @pFechaDesde and fecha < @pFechaHasta and v.empresa = @pEmpresa and v.borrado = 0
                       group by p.usuario, v.empresa";
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text, parameters);

                    connection.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            vendedoresPresentes.Add(dr.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener vendedores. " + ex.Message + "\n\nCtrlVend_ObtenerVendedoresPorFecha_Empresa");
            }

            var vendedores = new List<Vendedor>();
            foreach (var codigo in vendedoresPresentes)
            {
                var vendedor = ControladorVendedores.ObtenerVendedor(codigo);
                ControladorZonas.CargarZonasDelVendedor(vendedor, fecha);
                vendedores.Add(vendedor);
            }

            return vendedores;
        }


        public static void CargarPosiciones(Vendedor vendedor, DateTime fechaDesde, DateTime fechaHasta)
        {

            try
            {
                using (var connection = new SqlConnection(AccesoDB.SqlPreventa))
                {
                    SqlParameter[] parameters = { new SqlParameter("@pUsuario", vendedor.Codigo), new SqlParameter("@pFechaDesde", fechaDesde), new SqlParameter("@pFechaHasta", fechaHasta) };
                    var query = "select * from posiciongps where usuario = @pUsuario and fecha >= @pFechaDesde and fecha < @pFechaHasta";
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text, parameters);

                    connection.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr["usuario"].ToString() != null)
                            {
                                var pos = new Posicion();
                                pos.Cliente = dr["cliente"].ToString();
                                var Lat = double.Parse(dr["latitud"].ToString());
                                var Lng = double.Parse(dr["longitud"].ToString());

                                if (Lat != 0 && Lng != 0)
                                {
                                    ultimaCoordenadaConocida.Lat = Lat;
                                    ultimaCoordenadaConocida.Lng = Lng;
                                }
                                else
                                    if (Lat == 0)
                                    Console.WriteLine("aha!");

                                //pos.Coordenada = new PointLatLng()
                                //{
                                pos.Latitud = ultimaCoordenadaConocida.Lat;
                                pos.Longitud = ultimaCoordenadaConocida.Lng;
                                //};

                                if (dr["estado"] != null)
                                    pos.Estado = (Estado)dr["estado"];
                                else
                                    pos.Estado = Estado.OK;
                                pos.Fecha = Convert.ToDateTime(dr["fecha"]);
                                if (dr["motivonocompra"] != null)
                                    pos.MotivoNoCompra = (MotivoNoCompra)dr["motivonocompra"];
                                else
                                    pos.MotivoNoCompra = MotivoNoCompra.Compra;

                                pos.BultosCompra = int.Parse((nonull(dr["bultos"], 0).ToString()));
                                pos.PesosCompra = Decimal.Parse(nonull(dr["pesos"], 0).ToString());

                                vendedor.Posiciones.Add(pos);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar posiciones. " + ex.Message + "\n\nCtrlVend_CargarPosiciones");
            }

            //ServiceSoapClient serviceMobile = new ServiceSoapClient();
            //var dt = serviceMobile.ObtenerPosicionesDelPreventista(vendedor.Codigo, fechaDesde, fechaHasta);
            ////var ultimaCoordenadaConocida = new PointLatLng() { Lat = -38.002601, Lng = -57.601849 };
            //foreach (DataRow item in dt.AsEnumerable().Where(p => p.Field<string>("usuario") != ""))
            //{
            //    var pos = new Posicion();

            //    pos.Cliente = item.Field<object>("cliente").ToString();

            //    var Lat = double.Parse(item.Field<object>("latitud").ToString());
            //    var Lng = double.Parse(item.Field<object>("longitud").ToString());

            //    if (Lat != 0 && Lng != 0)
            //    {
            //        ultimaCoordenadaConocida.Lat = Lat;
            //        ultimaCoordenadaConocida.Lng = Lng;
            //    }
            //    else
            //        if (Lat == 0)
            //        Console.WriteLine("aha!");

            //    //pos.Coordenada = new PointLatLng()
            //    //{
            //    pos.Latitud = ultimaCoordenadaConocida.Lat;
            //    pos.Longitud = ultimaCoordenadaConocida.Lng;
            //    //};

            //    if (item.Field<object>("estado") != null)
            //        pos.Estado = (Estado)item.Field<object>("estado");
            //    else
            //        pos.Estado = Estado.OK;
            //    pos.Fecha = item.Field<DateTime>("fecha");
            //    if (item.Field<object>("motivonocompra") != null)
            //        pos.MotivoNoCompra = (MotivoNoCompra)item.Field<object>("motivonocompra");
            //    else
            //        pos.MotivoNoCompra = MotivoNoCompra.Compra;

            //    pos.BultosCompra = int.Parse((nonull(item.Field<object>("bultos"), 0).ToString()));
            //    pos.PesosCompra = Decimal.Parse(nonull(item.Field<object>("pesos"), 0).ToString());

            //    vendedor.Posiciones.Add(pos);
            //}

            var clientesVisitados = vendedor.Posiciones.DistinctBy(p => p.Cliente);
            vendedor.Visitados = clientesVisitados.Count(p => p.Cliente != "");
            var clientesCompradores = vendedor.Posiciones.Where(p => p.PesosCompra > 0 && p.Cliente != "").GroupBy(p => p.Cliente);
            vendedor.Compradores = clientesCompradores.Count();
            //vendedor.Compradores = 
        }

        public static void CalcularBultosYPesos(Vendedor vendedor, DateTime fechaDesde, DateTime fechaHasta)
        {
            vendedor.Posiciones.Clear();
            ControladorVendedores.CargarPosiciones(vendedor, fechaDesde, fechaHasta);

            var listaDeClientesVisitados = vendedor.Posiciones.Where(p => p.Cliente != "" && p.Estado == Estado.CHECKOUT_CLIENTE).ToList(); //tomo todos los checkouts donde tengo almacenados los pesos y los bultos
            var listaCodigosRevisados = new List<string>(); //almaceno aqui los codigos de los clientes que voy revisando. ocurre que hay veces que se registran mas de una vez el mismo cliente, con cantidades distintas de pesos y bultos. debo tomar siempre el último registro de estos.
            var listaFinalCheckouts = new List<Posicion>();
            foreach (var item in listaDeClientesVisitados)
            {
                var cliente = item.Cliente;
                if (!listaCodigosRevisados.Contains(cliente)) //verifico que no haya analizado a este cliente aun
                {
                    listaCodigosRevisados.Add(cliente); //procedo a agregarlo para saltear la proxima ocurrencia en la lista 

                    var todosLosRegistrosDeEsteCliente = listaDeClientesVisitados.Where(p => p.Cliente == cliente).ToList();
                    listaFinalCheckouts.Add(todosLosRegistrosDeEsteCliente.OrderByDescending(t => t.Fecha).FirstOrDefault());
                }
                //listaDeClientesVisitados.RemoveAll(p => p.Cliente == cliente);
            }
            vendedor.Pesos = listaFinalCheckouts.Sum(p => p.PesosCompra);
            vendedor.Bultos = listaFinalCheckouts.Sum(p => p.BultosCompra);
        }

        public static void CargarClientes(Vendedor vendedor, DateTime dia, ModoClientesRuteo modo)
        {
            try
            {
                if (vendedor.Zonas.Count == 0)
                    ControladorZonas.CargarZonasDelVendedor(vendedor, dia);
                vendedor.CantidadClientes = 0;
                foreach (var zona in vendedor.Zonas)
                {
                    ControladorZonas.CargarClientes(zona, modo);
                    vendedor.CantidadClientes += zona.Clientes.Count;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ActualizarCoordenadaDomicilio(Vendedor vendedor)
        {
            try
            {
                using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                {
                    var query = string.Format(@"UPDATE operator SET latitud = ?, longitud=? WHERE codigo=?");
                    var parameters = new OleDbParameter[] { new OleDbParameter("@lat", vendedor.CoordenadaDomicilio.Lat), new OleDbParameter("@lng", vendedor.CoordenadaDomicilio.Lng), new OleDbParameter("@cod", vendedor.Codigo) };
                    var command = AccesoDB.CrearComando(connection, query, CommandType.Text, parameters);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Vendedor> ObtenerUltimaPosicionVendedores()
        {
            List<Vendedor> res = new List<Vendedor>();
            try
            {
                using (var connection = new SqlConnection(AccesoDB.SqlPreventa))
                {
                    var command = AccesoDB.CrearComando(connection, "spObtenerUltimaPosicionVendedor", CommandType.StoredProcedure, new SqlParameter[] { new SqlParameter("@fecha", DateTime.Now.ToString("dd-MM-yyyy")) });
                    connection.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Func<object, dynamic, object> nonull = (p, def) => p == null ? def : p;
                            var elemento = new Vendedor()
                            {
                                Codigo = dr["usuario"].ToString(),
                                Estado = (Estado)nonull(dr["estado"], Estado.OK),
                                CodigoEmpresa = dr["empresa"].ToString(),
                                Fecha = dr["fecha"].ToString(),
                                CoordenadaActual = new PointLatLng(
                                                       double.Parse(dr["latitud"].ToString()),
                                                       double.Parse(dr["longitud"].ToString())),
                                //Visitados = (int)nonull(item.Field<object>("visitados"), 0),
                                //Compradores = (int)nonull(item.Field<object>("compradores"), 0),
                                FondoDeCelda = dr["empresa"].ToString() == "10" ? @"S:\GEOVENTAS\alta.jpg" : @"S:\GEOVENTAS\hiller.jpg"
                            };
                            res.Add(elemento);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
    }
}