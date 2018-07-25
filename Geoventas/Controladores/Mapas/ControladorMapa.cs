using GeoventasPocho.Vistas.ElementosMapa;
using GeoventasPocho.Vistas.ElementosMapa.Pines;
using GeoventasPocho.Vistas.ElementosMapa.Pines.Logistica;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeoventasPocho.Controladores.Mapas
{
    class ControladorMapa
    {
        public static GMapMarker CrearPuntoPosicion(PointLatLng posicion, Brush colorPunto = null, string hora = null)
        {
            var marcador = new GMapMarker(posicion);
            var forma = new Ellipse() { Height = 10, Width = 10 };
            if (hora != null)
                forma.ToolTip = hora;
            forma.Fill = colorPunto == null ? Brushes.OrangeRed : colorPunto;
            marcador.Shape = forma;
            marcador.Shape.IsHitTestVisible = true;
            marcador.Offset = new Point(-forma.Width / 2, -forma.Height);
            marcador.ZIndex = 2;
            return marcador;
        }

        public static void ImprimirCamino(Mapa mapa, List<Posicion> posiciones)
        {
            var ruta = CrearRuta(posiciones.Where(p => p.Latitud != 0 && p.Longitud != 0).Select(x => new PointLatLng(x.Latitud, x.Longitud)).ToList(), Brushes.Red);
            mapa.Markers.Add(ruta);
            int i = 0;
            foreach (var pos in posiciones)
            {
                i++;
                if (pos.Latitud != 0 && pos.Longitud != 0 && i == 10)
                {
                    i = 0;
                    mapa.Markers.Add(CrearPuntoPosicion(pos));
                }
            }
        }

        public static GMapMarker CrearPuntoPosicion(Posicion posicion, Brush colorPunto = null)
        {
            //var marcador = new GMapMarker(new PointLatLng(posicion.Latitud, posicion.Longitud));
            //var forma = new Ellipse() { Height = 10, Width = 10 };
            //forma.Fill = colorPunto == null ? Brushes.OrangeRed : colorPunto;
            //marcador.Shape = forma;
            //marcador.Shape.IsHitTestVisible = false;
            //marcador.Offset = new Point(-forma.Width / 2, -forma.Height);
            //marcador.ZIndex = 2;
            //return marcador;
            return CrearPuntoPosicion(new PointLatLng(posicion.Latitud, posicion.Longitud), colorPunto, posicion.Fecha.TimeOfDay.ToString());
        }

        public static GMapRoute CrearRuta(List<PointLatLng> puntosDeReporte, Brush color)
        {
            var path = new GMapRoute(puntosDeReporte);
            path.Shape = new Path() { Stroke = color, Opacity = 1, StrokeThickness = 2 };
            path.Shape.Visibility = Visibility.Visible;
            path.Shape.IsHitTestVisible = false;
            path.Tag = "CAMINO";
            path.ZIndex = 1;
            return path;
        }

        public static void ImprimirClientesFletero(Mapa mapa, List<Posicion> posiciones, List<ClienteFletero> clientes)
        {
            foreach (var cli in clientes)
            {
                var marcador = new GMapMarker(cli.Coordenada.Value);

                var pin = ControladorMapa.CrearPinClienteFletero(posiciones, cli);

                pin.Menu.UpdateLayout();

                marcador.Shape = pin;
                marcador.Shape.IsHitTestVisible = true;
                marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                marcador.ZIndex = 3;

                mapa.Markers.Add(marcador);
            }
        }

        public static GMapPolygon CrearPoligonoZona(List<PointLatLng> vertices, Brush color, string codigo)
        {
            var poly = new GMapPolygon(vertices);
            poly.Shape = new Path() { Fill = color, ToolTip = codigo, Stroke = Brushes.Black, Opacity = 0.35, StrokeThickness = 2 };
            poly.Shape.Visibility = Visibility.Visible;
            poly.Tag = "POLIGONO";
            poly.Shape.IsHitTestVisible = true;
            poly.ZIndex = 0;
            return poly;
        }

        public static void RefrescarVista(Mapa mapa)
        {
            var zoomActual = mapa.Zoom;
            mapa.Zoom = 20;
            mapa.Zoom = zoomActual;
        }

        public static PointLatLng? ObtenerCordenadasPorDireccion(string dir)
        {
            GeoCoderStatusCode status;
            var r = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetPoint(dir, out status);
            return r;
        }

        public static Pin CrearPinCliente(List<Posicion> posiciones, Cliente cli, bool imprimeRecorrido = true)
        {
            if (posiciones == null)
                posiciones = new List<Posicion>();

            var motivo = MotivoNoCompra.Compra;

            Pin pin;

            if (cli.OrdenRecorrido == 0)
            {
                pin = new PinGris();
            }
            else
            {
                if (posiciones.Any(p => p.Cliente == cli.Codigo))
                {
                    if (posiciones.Any(p => p.Cliente == cli.Codigo && p.MotivoNoCompra == MotivoNoCompra.Compra && p.PesosCompra > 0))
                        pin = new PinVerde();
                    else
                    {
                        pin = new PinRojo();
                        motivo = posiciones.LastOrDefault(p => p.Cliente == cli.Codigo).MotivoNoCompra;
                    }
                }
                else
                    pin = new PinAmarillo();
            }
            if (imprimeRecorrido)
                pin.Etiqueta = cli.OrdenRecorrido.ToString();
            else
                pin.Etiqueta = cli.Codigo;

            if (motivo != MotivoNoCompra.Compra)
                pin.ToolTip = cli.ToString() + "\n" + motivo.ToString().SplitCC();
            else
                pin.ToolTip = cli.ToString();

            var menuItem = new MenuItem();
            menuItem.Header = cli.Observacion == string.Empty ? "Sin observaciones" : cli.Observacion;
            pin.Menu.Items.Add(menuItem);
            pin.Menu.UpdateLayout();

            //if (ModoSeleccion == SelectionMode.Single)
            //{
            //    var menuMover = new MenuItem();
            //    menuMover.Header = "Ubicar manualmente en el mapa";
            //    menuMover.Command = this.CmdMoverUbicacionDeClienteManualmente;
            //    menuMover.CommandParameter = marcador;
            //    pin.Menu.Items.Add(menuMover);
            //}
            //var marcador = new GMapMarker(cli.Coordenada.Value);
            //marcador.Tag = cli;
            //marcador.Shape = pin;
            //marcador.Shape.IsHitTestVisible = true;
            //marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
            //marcador.ZIndex = 3;

            return pin;
        }

        public static Pin CrearPinClienteFletero(List<Posicion> posiciones, Cliente cli, bool imprimeRecorrido = true)
        {
            if (posiciones == null)
                posiciones = new List<Posicion>();
            Pin pin;
            var motivo = TipoVisita.Pendiente;
            if (posiciones.Any(p => p.Cliente == cli.Codigo))
            {
                if (posiciones.Any(p => p.Cliente == cli.Codigo && p.TipoVisita == TipoVisita.EntregaTotal))
                {
                    pin = new PinEntregaTotal();
                    motivo = TipoVisita.EntregaTotal;
                }
                else
                {
                    motivo = posiciones.LastOrDefault(p => p.Cliente == cli.Codigo).TipoVisita;
                    switch (motivo)
                    {
                        case TipoVisita.EntregaParcial:
                            pin = new PinEntregaParcial();
                            break;
                        case TipoVisita.Rechazado:
                            pin = new PinPedidoRechazado();
                            break;
                        case TipoVisita.Cerrado:
                            pin = new PinVisitaCerrado();
                            break;
                        case TipoVisita.VolverLuego:
                            pin = new PinVolverLuego();
                            break;
                        case TipoVisita.SinVisitar:
                            pin = new PinSinVisitar();
                            break;
                        default:
                            pin = new PinVisitaPendiente();
                            break;
                    }
                }
                //if (posiciones.Any(p => p.Cliente == cli.Codigo && p.MotivoNoCompra == MotivoNoCompra.Compra && p.PesosCompra > 0))
                //    pin = new PinVerde();
                //else
                //{
                //    pin = new PinRojo();
                //    motivo = posiciones.LastOrDefault(p => p.Cliente == cli.Codigo).MotivoVisita;
                //}
            }
            else
                pin = new PinVisitaPendiente();

            pin.Etiqueta = cli.Codigo;

            //if (motivo != MotivoVisita.EntregaTotal)
            pin.ToolTip = cli.ToString() + "\n" + motivo.ToString().SplitCC();
            //else
            //    pin.ToolTip = cli.ToString();

            var menuItem = new MenuItem();
            menuItem.Header = cli.Observacion == string.Empty ? "Sin observaciones" : cli.Observacion;
            pin.Menu.Items.Add(menuItem);
            pin.Menu.UpdateLayout();
            return pin;
        }

        /// <summary>
        /// Devuelve true si el punto se encuentra dentro del poligono
        /// </summary>
        /// <param name="poligonoZona"></param>
        /// <param name="ubicacionActualVendedor"></param>
        /// <returns></returns>
        public static bool DentroDeZona(List<PointLatLng> poligonoZona, double latitud, double longitud)
        {
            //  Globals which should be set before calling this function:
            //
            //  int    polyCorners  =  how many corners the polygon has (no repeats)
            //  float  polyX[]      =  horizontal coordinates of corners
            //  float  polyY[]      =  vertical coordinates of corners
            //  float  x, y         =  point to be tested
            //
            //  (Globals are used in this example for purposes of speed.  Change as
            //  desired.)
            //
            //  The function will return YES if the point x,y is inside the polygon, or
            //  NO if it is not.  If the point is exactly on the edge of the polygon,
            //  then the function may return YES or NO.
            //
            //  Note that division by zero is avoided because the division is protected
            //  by the "if" clause which surrounds it.

            var polyCorners = poligonoZona.Count;
            var polyY = poligonoZona.Select(v => v.Lng).ToArray();
            var polyX = poligonoZona.Select(v => v.Lat).ToArray();
            var y = longitud;
            var x = latitud;
            int i, j = polyCorners - 1;
            bool oddNodes = false;

            for (i = 0; i < polyCorners; i++)
            {
                if ((polyY[i] < y && polyY[j] >= y || polyY[j] < y && polyY[i] >= y) && (polyX[i] <= x || polyX[j] <= x))
                {
                    if (polyX[i] + (y - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]) < x)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }
    }
}
