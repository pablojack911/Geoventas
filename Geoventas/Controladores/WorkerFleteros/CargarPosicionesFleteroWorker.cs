using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.DAO;
using GeoventasPocho.Vistas.ElementosMapa;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores.WorkerFleteros
{
    public class CargarPosicionesFleteroWorker : BackgroundWorker
    {
        public Fletero fletero { get; set; }
        DateTime fechaDesde;
        DateTime fechaHasta;
        Mapa mapa;
        public CargarPosicionesFleteroWorker()
        {
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
        }

        public CargarPosicionesFleteroWorker(Mapa mapa, Fletero flet, DateTime fechaDesde, DateTime fechaHasta) : this()
        {
            this.mapa = mapa;
            this.fletero = flet;
            //if (this.fletero.CoordenadaActual != null && this.fletero.CoordenadaActual.IsEmpty)
            //{
            //    this.fechaDesde = fechaDesde;
            //}
            //else
            //{
            //    this.fechaDesde = DateTime.Parse(this.fletero.Fecha);
            //}
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
        }

        //protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        //{
        //    ControladorMapa.ImprimirCamino(this.mapa, this.fletero.Posiciones);
        //}

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            fletero.Posiciones.Clear();
            try
            {
                using (var con = new SqlConnection(AccesoDB.SqlLogistica))
                {
                    var query = @"SELECT cliente,estado,latitud,longitud,Max(fecha) as fec FROM trackingfletero WHERE usuario=@pFletero AND fecha between @pFechaDesde and @pFechaHasta group by cliente,estado,latitud,longitud order by fec, cliente, estado";
                    var consulta = con.CreateCommand();
                    consulta.CommandType = CommandType.Text;
                    consulta.CommandText = query;
                    consulta.CommandTimeout = 1000;
                    consulta.Parameters.Add(new SqlParameter("@pFletero", fletero.Codigo));
                    consulta.Parameters.Add(new SqlParameter("@pFechaDesde", fechaDesde));
                    consulta.Parameters.Add(new SqlParameter("@pFechaHasta", fechaHasta));
                    con.Open();
                    using (var dr = consulta.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var pos = new Posicion();
                            pos.Cliente = dr.GetString(0);
                            pos.TipoVisita = (TipoVisita)Convert.ToInt32(dr.GetValue(1));
                            pos.Latitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
                            pos.Longitud = Convert.ToDouble(dr.GetValue(3), CultureInfo.CurrentCulture);
                            pos.Fecha = dr.GetDateTime(4);
                            fletero.Posiciones.Add(pos);
                        }
                        var clientesVisitados = fletero.Posiciones.Where(p => (p.Cliente != "" && p.Cliente != "VIAJE") && p.TipoVisita != TipoVisita.Pendiente).DistinctBy(x => x.Cliente);
                        fletero.Visitados = clientesVisitados.Count();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ReportProgress(100, "Done!");
            }
            e.Result = fletero;
        }

        /*
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            fletero.Posiciones.Clear();
            Func<object, dynamic, object> nonull = (p, def) => p == null ? def : p;
            try
            {
                using (var connection = new OleDbConnection(FoxDB.Real))
                {
                    var query = @"SELECT cliente,estado,latitud,longitud,fecha FROM trackingfletero WHERE usuario=? AND BETWEEN(fecha,?,?) group by latitud,longitud,cliente order by fecha";
                    var consulta = ControladoraDB.CrearComando(connection, query);
                    consulta.Parameters.Add(new OleDbParameter("@fletero", fletero.Codigo));
                    consulta.Parameters.Add(new OleDbParameter("@fdesde", fechaDesde));
                    consulta.Parameters.Add(new OleDbParameter("@fhasta", fechaHasta));
                    ControladoraDB.Conectar(connection);
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
                            fletero.Posiciones.Add(pos);
                        }
                        var clientesVisitados = fletero.Posiciones.Where(p => (p.Cliente != "" && p.Cliente != "VIAJE") && p.MotivoVisita != MotivoVisita.Pendiente).DistinctBy(x => x.Cliente);
                        fletero.Visitados = clientesVisitados.Count();
                    }
                    //ControladoraDB.Desconectar(connection);
                    //dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ReportProgress(100, "Done!");
            }
            e.Result = fletero;
        }*/
    }
}
