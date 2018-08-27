using GeoventasPocho.DAO;
using GeoventasPocho.Vistas.ElementosMapa;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class CargarFleterosWorker : BackgroundWorker
    {
        DateTime fecha;
        List<Fletero> FleterosDelDia;

        public CargarFleterosWorker()
        {
            this.WorkerReportsProgress = true;
        }

        public CargarFleterosWorker(List<Fletero> fleterosDelDia, DateTime fecha) : this()
        {
            this.fecha = fecha;
            this.FleterosDelDia = fleterosDelDia;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            var fleteros = obtenerFleteros();
            foreach (var item in fleteros)
            {
                this.AgregarFletero(item);
            }
            e.Result = this.FleterosDelDia;
        }

        private List<Fletero> obtenerFleteros()
        {
            var fleteros = new List<Fletero>();
            try
            {
                using (var con = new SqlConnection(AccesoDB.SqlLogistica))
                {
                    var query = "SELECT distinct a.usuario, a.latitud, a.longitud, a.fecha FROM trackingfletero a inner join(select usuario, max(fecha) as fecha from trackingfletero where fecha>=@pFecha group by usuario) b on a.usuario = b.usuario where a.fecha = b.fecha";
                    var consulta = con.CreateCommand();
                    consulta.CommandType = CommandType.Text;
                    consulta.CommandText = query;
                    consulta.CommandTimeout = 3600;
                    consulta.Parameters.Add(new SqlParameter("@pFecha", fecha));
                    con.Open();
                    using (var dr = consulta.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var flet = new Fletero();
                            flet.Codigo = dr.GetString(0).ToString();
                            var latitud = Convert.ToDouble(dr.GetValue(1), CultureInfo.CurrentCulture);
                            var longitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
                            flet.CoordenadaActual = new PointLatLng(latitud, longitud);
                            flet.Fecha = dr.GetDateTime(3).ToString();
                            fleteros.Add(flet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fleteros;
        }

        //private void AgregarFletero(Fletero fletero)
        //{
        //    var f = this.FleterosDelDia.FirstOrDefault(e => e.Codigo == fletero.Codigo);
        //    if (f == null)
        //    {
        //        var flet = CargarDatosFletero(fletero.Codigo);
        //        if (flet != null)
        //        {
        //            fletero.Foto = flet.Foto;
        //            fletero.Nombre = flet.Nombre;
        //            fletero.CoordenadaDomicilio = new PointLatLng(flet.CoordenadaDomicilio.Lat, flet.CoordenadaDomicilio.Lng);
        //        }

        //        if (fletero.CoordenadaActual.Lat == 0 && fletero.CoordenadaActual.Lng == 0) //si lat y lng vienen 0, 0 es porque tiene gps apagado
        //        {
        //            fletero.CoordenadaActual = new PointLatLng(-38.002452, -57.601936);
        //            fletero.Estado = Estado.GPS_APAGADO;
        //        }
        //        CalculaTiempoReporte(fletero);

        //        this.FleterosDelDia.Add(fletero);
        //    }
        //}

        private void AgregarFletero(Fletero fletero)
        {
            if (this.FleterosDelDia.Any(e => e.Codigo == fletero.Codigo))
            {
                var flet = this.FleterosDelDia.FirstOrDefault(x => x.Codigo == fletero.Codigo);
                //.Estado = fletero.Estado;
                //this.FleterosDelDia.FirstOrDefault(x => x.Codigo == fletero.Codigo).Fecha = fletero.Fecha;
                //this.FleterosDelDia.FirstOrDefault(x => x.Codigo == fletero.Codigo));
                flet.Estado = fletero.Estado;
                flet.Fecha = fletero.Fecha;
                CalculaTiempoReporte(ref flet);
                if (fletero.CoordenadaActual.Lat == 0)
                    flet.Estado = Estado.GPS_APAGADO;
                else
                    flet.CoordenadaActual = fletero.CoordenadaActual;
                //            f.Estado = fleteroMapa.Estado;
                //            f.Fecha = fleteroMapa.Fecha;

                //            this.CalculaTiempoReporte(f);

                //            if (fleteroMapa.CoordenadaActual.Lat == 0 && fleteroMapa.CoordenadaActual.Lng == 0)
                //                fleteroMapa.Estado = Estado.GPS_APAGADO;
                //            else
                //                f.CoordenadaActual = fleteroMapa.CoordenadaActual;
            }
            else
            {
                CargarDatosFletero(ref fletero);
                CalculaTiempoReporte(ref fletero);
                if (fletero.CoordenadaActual.Lat == 0 && fletero.CoordenadaActual.Lng == 0) //si lat y lng vienen 0, 0 es porque tiene gps apagado
                {
                    fletero.CoordenadaActual = new PointLatLng(-38.002452, -57.601936);
                    fletero.Estado = Estado.GPS_APAGADO;
                }
                this.FleterosDelDia.Add(fletero);
            }
        }

        private void CalculaTiempoReporte(ref Fletero fletero)
        {
            var fecha = DateTime.Parse(fletero.Fecha);
            var timeSpan = DateTime.Now - fecha;
            if (timeSpan.Minutes > 30 || timeSpan.Hours > 1)
            {
                fletero.Estado = Estado.NO_REPORTA;
            }
        }

        private void CargarDatosFletero(ref Fletero flet)
        {
            //var flet = new Fletero();

            using (var connection = new OleDbConnection(AccesoDB.FoxPreventaReal))
            {
                var query = string.Format("SELECT nombre,domicilio FROM proveedo WHERE fletero=1 AND habilitado=1 and codigo='{0}'", flet.Codigo);
                var consulta = AccesoDB.CrearComando(connection, query, CommandType.Text);
                connection.Open();
                AccesoDB.ComandosFox(connection);
                var dr = consulta.ExecuteReader();
                if (dr.Read())
                {
                    flet.Nombre = dr.GetString(0).Trim();
                    flet.Domicilio = dr.GetString(1).Trim();
                }
            }
            //return flet;
        }
        //private List<Fletero> obtenerFleteros()
        //{
        //    var fleteros = new List<Fletero>();
        //    try
        //    {
        //        using (var connection = new OleDbConnection(FoxDB.Real))
        //        {
        //            var query = "SELECT usuario, latitud, longitud, MAX(fecha) FROM trackingfletero WHERE fecha >= ? GROUP BY usuario ORDER BY fecha desc";
        //            var consulta = ControladoraDB.CrearComando(connection, query);
        //            consulta.Parameters.Add(new OleDbParameter("@fecha", fecha));
        //            ControladoraDB.Conectar(connection);
        //            using (var dr = consulta.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    var flet = new Fletero();
        //                    flet.Codigo = dr.GetString(0).ToString();
        //                    var latitud = Convert.ToDouble(dr.GetValue(1), CultureInfo.CurrentCulture);
        //                    var longitud = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
        //                    flet.CoordenadaActual = new GMap.NET.PointLatLng(latitud, longitud);
        //                    flet.Fecha = dr.GetDateTime(3).ToString();
        //                    fleteros.Add(flet);
        //                }
        //            }
        //            //ControladoraDB.Desconectar(connection);
        //            //dr.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return fleteros;
        //}


    }
}
