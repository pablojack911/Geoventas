using GeoventasPocho.Vistas.ElementosMapa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas
{
    public class ReportMaker
    {
        public static List<ItemReporte> CrearReporte(Vendedor vendedor)
        {
            var reportes = new List<ItemReporte>();
            foreach (var posicion in vendedor.Posiciones)
            {
                if (posicion.Cliente == string.Empty)
                {
                    CrearItemViaje(reportes, posicion, vendedor.Codigo);
                }
                else
                    if (posicion.Estado == Estado.CHECKIN_CLIENTE)
                    CrearItemCheckin(reportes, posicion, vendedor.Codigo);
                else
                        if (posicion.Estado == Estado.CHECKOUT_CLIENTE)
                    CrearItemCheckout(reportes, posicion, vendedor.Codigo);
            }
            return reportes;
        }

        //public static List<ItemReporte> CrearReporte(Fletero fletero)
        //{
        //    var reportes = new List<ItemReporte>();
        //    foreach (var posicion in fletero.Posiciones)
        //    {
        //        if (posicion.Cliente == string.Empty || posicion.Cliente == "VIAJE")
        //        {
        //            CrearItemViaje(reportes, posicion, fletero.Codigo);
        //        }
        //        else
        //        {
        //            if (posicion.MotivoVisita == MotivoVisita.Pendiente)
        //            {
        //                CrearItemCheckin(reportes, posicion, fletero.Codigo);
        //            }
        //            else
        //            {
        //                //PODRIA PONER QUE TIPO DE MOTIVO ESTOY FILTRANDO...
        //                if (posicion.MotivoVisita != MotivoVisita.Pendiente && posicion.MotivoVisita != MotivoVisita.VolverLuego)
        //                {
        //                    CrearItemCheckout(reportes, posicion, fletero.Codigo); 
        //                }
        //            }
        //        }
        //    }
        //    return reportes;
        //}

        public static List<ItemReporte> CrearReporte(Fletero fletero)
        {
            var reportes = new List<ItemReporte>();
            for (int i = 0; i < fletero.Posiciones.Count; i++)
            {
                var posicion = fletero.Posiciones[i];
                if (posicion.Cliente == string.Empty || posicion.Cliente == "VIAJE")
                {
                    CrearItemViaje(reportes, posicion, fletero.Codigo);
                }
                else
                {
                    //if(posicion.Cliente=="18874")
                    //{
                    //    var x = 1;
                    //    x++;
                    //}
                    if (posicion.TipoVisita == TipoVisita.Pendiente)
                    {
                        var posicionesSiguientes = fletero.Posiciones.GetRange(i + 1, fletero.Posiciones.Count - i - 1);
                        foreach (var ps in posicionesSiguientes)
                        {
                            if (ps.Cliente != string.Empty && ps.Cliente != "VIAJE")
                            {
                                if (ps.Cliente != posicion.Cliente)
                                {
                                    break;
                                }
                                else
                                {
                                    if (ps.TipoVisita == TipoVisita.Pendiente)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        CrearItemCheckin(reportes, posicion, fletero.Codigo);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //PODRIA PONER QUE TIPO DE MOTIVO ESTOY FILTRANDO...
                        if (posicion.TipoVisita != TipoVisita.Pendiente && posicion.TipoVisita != TipoVisita.VolverLuego)
                        {
                            CrearItemCheckout(reportes, posicion, fletero.Codigo);
                        }
                    }
                }
            }
            return reportes;
        }

        private static void CrearItemCheckout(List<ItemReporte> reportes, Posicion posicion, string codigoVendedor)
        {
            if (reportes.Count == 0)
                reportes.Add(new ItemReporte(codigoVendedor)
                {
                    Cliente = posicion.Cliente,
                    CheckOut = posicion.Fecha,
                    Tipo = TipoReporte.CHECK_OUT
                });
            else
                if (reportes.LastOrDefault().Tipo == TipoReporte.CHECK_IN)
            {
                reportes.LastOrDefault().Tipo = TipoReporte.CHECK_OUT;
                reportes.LastOrDefault().CheckOut = posicion.Fecha; //actualizo fecha de checkout
                reportes.LastOrDefault().Tiempo = (reportes.LastOrDefault().CheckOut.TimeOfDay - reportes.LastOrDefault().CheckIn.TimeOfDay).ToString(); //calculo el tiempo que tardó entre un reporte y otro
                reportes.LastOrDefault().TipoVisita = posicion.TipoVisita;
                reportes.LastOrDefault().MotivoNoCompra = posicion.MotivoNoCompra;
            }
        }

        private static void CrearItemCheckin(List<ItemReporte> reportes, Posicion posicion, string codigoVendedor)
        {
            if (reportes.Count == 0)
                reportes.Add(new ItemReporte(codigoVendedor)
                {
                    Cliente = posicion.Cliente,
                    CheckIn = posicion.Fecha,
                    Tipo = TipoReporte.CHECK_IN
                });
            else
                if (reportes.LastOrDefault().Tipo == TipoReporte.CHECK_OUT)
            {
                reportes.Add(new ItemReporte(codigoVendedor)
                {
                    Cliente = posicion.Cliente,
                    CheckIn = posicion.Fecha,
                    Tipo = TipoReporte.CHECK_IN
                });
            }
            else
            {
                reportes.LastOrDefault().CheckOut = posicion.Fecha; //actualizo fecha de checkout
                reportes.LastOrDefault().Tiempo = (reportes.LastOrDefault().CheckOut.TimeOfDay - reportes.LastOrDefault().CheckIn.TimeOfDay).ToString(); //calculo el tiempo que tardó entre un reporte y otro

                reportes.Add(new ItemReporte(codigoVendedor)
                {
                    Cliente = posicion.Cliente,
                    CheckIn = posicion.Fecha,
                    Tipo = TipoReporte.CHECK_IN
                });
            }
        }

        private static void CrearItemViaje(List<ItemReporte> reportes, Posicion posicion, string codigoVendedor)
        {
            if (reportes.Count == 0) //está vacío. Primer reporte.
                reportes.Add(new ItemReporte(codigoVendedor)
                {
                    Cliente = "VIAJE",
                    CheckIn = posicion.Fecha,
                    Tipo = TipoReporte.EN_VIAJE
                });
            else //ya hay elementos en la lista de reportes
                if (reportes.LastOrDefault().Tipo == TipoReporte.CHECK_OUT)
            {
                reportes.Add(new ItemReporte(codigoVendedor)
                {
                    Cliente = "VIAJE",
                    CheckIn = posicion.Fecha,
                    Tipo = TipoReporte.EN_VIAJE
                });
            }
            else
            //if (reportes.LastOrDefault().Tipo == TipoReporte.EN_VIAJE) //el ultimo reporte es tipo EN_VIAJE
            {
                reportes.LastOrDefault().CheckOut = posicion.Fecha; //actualizo fecha de checkout
                reportes.LastOrDefault().Tiempo = (reportes.LastOrDefault().CheckOut.TimeOfDay - reportes.LastOrDefault().CheckIn.TimeOfDay).ToString(); //calculo el tiempo que tardó entre un reporte y otro
            }
            //else //si es tipo CHECK_IN tengo que actualizar el checkout y tiempo por si llegara a ser que no existe elemento checkout
        }
    }
}
