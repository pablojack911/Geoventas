using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.DAO;
using GeoventasPocho.Vistas.ElementosMapa;
using GMap.NET;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores.WorkerFleteros
{
    public class CargarClientesFleteroWorker : BackgroundWorker
    {
        public Fletero Fletero { get; set; }
        DateTime fecha;
        Mapa mapa;
        public CargarClientesFleteroWorker()
        {
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
        }
        public CargarClientesFleteroWorker(Mapa mapa, Fletero flet, DateTime fecha) : this()
        {
            this.mapa = mapa;
            this.Fletero = flet;
            this.fecha = fecha;
        }

        //protected override void OnDoWork(DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        if (Fletero.Clientes.Count == 0)
        //        {
        //            var clientes = ControladoraWebApi.ObtenerClientes(Fletero.Codigo).Result;
        //            foreach (var cli in clientes)
        //            {
        //                if (!Fletero.Clientes.Any(c => c.Codigo.Equals(cli.Codigo)))
        //                {
        //                    Fletero.Clientes.Add(cli);
        //                }
        //            }
        //        }
        //        Fletero.CantidadClientes = Fletero.Clientes.DistinctBy(x => x.Codigo).Count();
        //        e.Result = Fletero;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        ReportProgress(100, "Done!");
        //    }
        //}

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                //Fletero.Clientes.Clear();
                if (Fletero.Clientes.Count == 0)
                    using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
                    {
                        var parameters = new OleDbParameter[] { new OleDbParameter("@fletero", Fletero.Codigo), new OleDbParameter("@fecha", fecha.Date) };
                        var sp = AccesoDB.CrearComando(connection, "clientesfletero", CommandType.StoredProcedure, parameters);
                        connection.Open();
                        AccesoDB.ComandosFox(connection);
                        using (var dr = sp.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string codigo = dr.GetString(0).Trim();
                                if (!Fletero.Clientes.Any(c => c.Codigo.Equals(codigo)))
                                {
                                    var cli = new ClienteFletero();
                                    cli.Codigo = codigo;
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
                                    Fletero.Clientes.Add(cli);
                                }
                            }
                        }
                    }
                Fletero.CantidadClientes = Fletero.Clientes.DistinctBy(x => x.Codigo).Count();
                e.Result = Fletero;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ReportProgress(100, "Done!");
            }
        }

        //protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        //{
        //    ControladorMapa.ImprimirClientesFletero(mapa, Fletero.Posiciones, Fletero.Clientes);
        //}
    }
}
