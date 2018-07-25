using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.Vistas.ElementosMapa;
using GeoventasPocho.Vistas.ElementosMapa.Pines;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeoventasPocho.Vistas.PosicionesActualesLogistica
{
    /// <summary>
    /// Interaction logic for MapaPosicionesActualesLogistica.xaml
    /// </summary>
    public partial class MapaPosicionesActualesLogistica : Window
    {

        public ObservableCollection<Fletero> Fleteros { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        Timer actualizadorDePosiciones;

        public string FiltroDeLista
        {
            get { return (string)GetValue(FiltroDeListaProperty); }
            set { SetValue(FiltroDeListaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FiltroDeLista.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FiltroDeListaProperty =
            DependencyProperty.Register("FiltroDeLista", typeof(string), typeof(MapaPosicionesActualesLogistica), new PropertyMetadata("Codigo"));
        private ModoVerMarcadores modo;

        public ICommand CmdVerClientesPorRuta { get; set; }
        public ICommand CmdDibujarZona { get; set; }
        public ICommand CmdVerCaminoPreventista { get; set; }
        public ICommand CmdVerificarPreventistaDentroDeZona { get; set; }
        public ICommand CmdVerDomicilioFletero { get; set; }


        public MapaPosicionesActualesLogistica()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Fleteros = new ObservableCollection<Fletero>();

            this.FechaDesde = DateTime.Today.AddHours(5);
            this.FechaHasta = DateTime.Today.AddDays(1);

            var mdp = ControladorMapa.ObtenerCordenadasPorDireccion("Mar del Plata, Buenos Aires");
            if (mdp.HasValue)
                this.mapa.Position = mdp.Value;
            else
                this.mapa.Position = new PointLatLng(-38.0042615, -57.6070055);

            this.actualizadorDePosiciones = new Timer(45000);
            this.actualizadorDePosiciones.Elapsed += actualizadorDePosiciones_Elapsed;
            this.actualizadorDePosiciones.Start();

            this.listaDeElementos.SelectionChanged += listaDeElementos_SelectionChanged;

            this.CmdVerClientesPorRuta = new RelayCommand(e => this.VerClientesDelVendedor(e));
            this.CmdVerCaminoPreventista = new RelayCommand(e => this.VerCaminoDelVendedor(e));
            //this.CmdVerificarPreventistaDentroDeZona = new RelayCommand(e => this.VerificarPreventistaDentroDeZona(e));
            this.CmdVerDomicilioFletero = new RelayCommand(e => this.VerDomicilioFletero(e));

        }


        private void listaDeElementos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.listaDeElementos.SelectedItem as Fletero;
            item.VerClientes = false;
            //item.VerZona = false;
            item.VerTodasLasPosiciones = false;
            //item.VerDomicilioDelVendedor = false;
            SeleccionarMarcador(item);
            this.modo = ModoVerMarcadores.Seleccionado;
        }

        private void SeleccionarMarcador(Fletero item)
        {
            MostrarMarcador(item, true);
            mapa.Position = item.CoordenadaActual;
        }

        private void MostrarTodo()
        {
            this.mapa.Markers.Clear();
            foreach (var item in this.Fleteros.Where(f => f.Visible == true))
            {
                item.VerClientes = false;
                //item.VerZona = false;
                item.VerTodasLasPosiciones = false;
                //item.VerDomicilioDelVendedor = false;
                this.MostrarMarcador(item);
            }
        }

        private void MostrarMarcador(Fletero fleteroMapa, bool clear = false)
        {
            if (clear)
                mapa.Markers.Clear();

            var marcador = new GMapMarker(fleteroMapa.CoordenadaActual);
            Pin pin = new PinAzul();
            pin.Tag = fleteroMapa;
            pin.Etiqueta = fleteroMapa.Codigo;

            var menuItem = new MenuItem();
            menuItem.Header = "Ver Clientes de la Ruta";
            menuItem.Command = this.CmdVerClientesPorRuta;
            menuItem.CommandParameter = fleteroMapa;
            pin.Menu.Items.Add(menuItem);

            var menuMostrarCamino = new MenuItem();
            menuMostrarCamino.Header = "Mostrar todos los reportes";
            menuMostrarCamino.Command = this.CmdVerCaminoPreventista;
            menuMostrarCamino.CommandParameter = fleteroMapa;
            pin.Menu.Items.Add(menuMostrarCamino);

            var menuMostrarDomicilio = new MenuItem();
            menuMostrarDomicilio.Header = "Mostrar domicilio del fletero";
            menuMostrarDomicilio.Command = this.CmdVerDomicilioFletero;
            menuMostrarDomicilio.CommandParameter = fleteroMapa;
            pin.Menu.Items.Add(menuMostrarDomicilio);

            pin.Menu.UpdateLayout();

            //pin.MouseDoubleClick += pin_MouseDoubleClick;

            marcador.Shape = pin;
            marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
            marcador.ZIndex = 4;
            this.mapa.Markers.Add(marcador);

            if (fleteroMapa.VerClientes)
                this.VerClientesDelVendedor(fleteroMapa);
            if (fleteroMapa.VerTodasLasPosiciones)
                this.VerCaminoDelVendedor(fleteroMapa);
            if (fleteroMapa.VerDomicilioDelFletero)
                this.VerDomicilioFletero(fleteroMapa);

            ControladorMapa.RefrescarVista(this.mapa);
        }

        private object VerDomicilioFletero(object fleteroMapa)
        {
            if (fleteroMapa == null)
                return false;
            try
            {
                var f = fleteroMapa as Fletero;
                f.VerDomicilioDelFletero = true;

                if (f.Domicilio != string.Empty)
                {
                    var dirCasa = f.Domicilio + ", Mar del Plata";
                    var coordenadaCasa = ControladorMapa.ObtenerCordenadasPorDireccion(dirCasa);
                    if (coordenadaCasa.HasValue)
                    {
                        f.CoordenadaDomicilio = coordenadaCasa.Value;
                        //ControladorFleteros.ActualizarCoordenadaDomicilio(f);
                    }
                }
                if (f.CoordenadaDomicilio.Lat != 0)
                {
                    var marcador = new GMapMarker(f.CoordenadaDomicilio);
                    var pin = new PinCasa();
                    pin.Tag = f;
                    pin.Etiqueta = string.Empty;
                    marcador.Shape = pin;
                    marcador.Shape.IsHitTestVisible = true;
                    marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                    pin.ToolTip = "Casa de " + f.Nombre;

                    var menuItem = new MenuItem();
                    menuItem.Header = f.Domicilio;
                    pin.Menu.Items.Add(menuItem);

                    pin.Menu.UpdateLayout();

                    marcador.ZIndex = 3;
                    this.mapa.Markers.Add(marcador);
                }
                ControladorMapa.RefrescarVista(this.mapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;

        }

        private object VerClientesDelVendedor(object fleteroMapa)
        {
            if (fleteroMapa == null)
                return false;
            try
            {
                var f = fleteroMapa as Fletero;
                f.VerClientes = true;

                if (f.Clientes.Count == 0)
                    ControladorFleteros.CargarClientesDelFletero(f, DateTime.Today);
                ControladorFleteros.CargarPosiciones(f, FechaDesde, FechaHasta);
                foreach (var cli in f.Clientes)
                {
                    var marcador = new GMapMarker(cli.Coordenada.Value);

                    var pin = ControladorMapa.CrearPinClienteFletero(f.Posiciones, cli);

                    marcador.Shape = pin;
                    marcador.Shape.IsHitTestVisible = true;
                    marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                    marcador.ZIndex = 3;

                    this.mapa.Markers.Add(marcador);
                }
                ControladorMapa.RefrescarVista(this.mapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        private object VerCaminoDelVendedor(object fleteroMapa)
        {
            if (fleteroMapa == null)
                return false;
            try
            {
                var f = fleteroMapa as Fletero;
                f.VerTodasLasPosiciones = true;
                if (f.Posiciones.Count == 0)
                    ControladorFleteros.CargarPosiciones(f, FechaDesde, FechaHasta);
                this.mapa.Markers.Add(ControladorMapa.CrearRuta(f.Posiciones.Where(p => p.Latitud != 0 && p.Longitud != 0).Select(x => new PointLatLng(x.Latitud, x.Longitud)).ToList(), Brushes.Red));
                foreach (var pos in f.Posiciones)
                {
                    if (pos.Latitud != 0 && pos.Longitud != 0)
                        this.mapa.Markers.Add(ControladorMapa.CrearPuntoPosicion(pos));
                }

                ControladorMapa.RefrescarVista(this.mapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        private void actualizadorDePosiciones_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.ObtenerPosiciones();
        }

        void ObtenerPosiciones()
        {
            this.actualizadorDePosiciones.Stop();
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                try
                {
                    var fleteros = ControladorFleteros.ObtenerUltimaPosicionFleteros(FechaDesde);
                    foreach (var item in fleteros)
                    {
                        this.AgregarActualizarFletero(item);
                    }

                    switch (this.modo)
                    {
                        case ModoVerMarcadores.Nada:
                            break;
                        case ModoVerMarcadores.Todo:
                            this.MostrarTodo();
                            break;
                        case ModoVerMarcadores.Seleccionado:
                            this.MostrarMarcador(this.listaDeElementos.SelectedItem as Fletero, true);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            });
            this.actualizadorDePosiciones.Start();
        }

        private void AgregarActualizarFletero(Fletero fleteroMapa)
        {
            try
            {
                var f = Fleteros.FirstOrDefault(e => e.Codigo == fleteroMapa.Codigo);
                if (f == null)
                {
                    var flet = ControladorFleteros.ObtenerFletero(fleteroMapa.Codigo);
                    if (flet != null)
                    {
                        fleteroMapa.Foto = flet.Foto;
                        fleteroMapa.Nombre = flet.Nombre;
                        fleteroMapa.Domicilio = flet.Domicilio;
                        fleteroMapa.CoordenadaDomicilio = new PointLatLng(flet.CoordenadaDomicilio.Lat, flet.CoordenadaDomicilio.Lng);
                    }

                    this.CalculaTiempoReporte(fleteroMapa);

                    if (fleteroMapa.CoordenadaActual.Lat == 0 && fleteroMapa.CoordenadaActual.Lng == 0) //si lat y lng vienen 0, 0 es porque tiene gps apagado
                    {
                        fleteroMapa.CoordenadaActual = new PointLatLng(-38.002452, -57.601936);
                        fleteroMapa.Estado = Estado.GPS_APAGADO;
                    }

                    this.Fleteros.Add(fleteroMapa);
                }
                else
                {
                    if (f.Clientes.Count == 0)
                        ControladorFleteros.CargarClientesDelFletero(f, DateTime.Today);

                    f.Estado = fleteroMapa.Estado;
                    f.Fecha = fleteroMapa.Fecha;

                    this.CalculaTiempoReporte(f);

                    if (fleteroMapa.CoordenadaActual.Lat == 0 && fleteroMapa.CoordenadaActual.Lng == 0)
                        fleteroMapa.Estado = Estado.GPS_APAGADO;
                    else
                        f.CoordenadaActual = fleteroMapa.CoordenadaActual;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void CalculaTiempoReporte(Fletero fletero)
        {
            var fecha = DateTime.Parse(fletero.Fecha);
            var timeSpan = DateTime.Now - fecha;
            if (timeSpan.Minutes > 30 || timeSpan.Hours > 1)
            {
                fletero.Estado = Estado.NO_REPORTA;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }


        #region MENU FILTROS DE VISTA
        private void btnFiltrarPorCodigo_Click(object sender, RoutedEventArgs e)
        {
            this.FiltroDeLista = "Codigo";
        }

        private void btnFiltrarPorNombre_Click(object sender, RoutedEventArgs e)
        {
            this.FiltroDeLista = "Nombre";
        }
        #endregion

        #region MENU VER
        private void btnMostrarTodos_Click(object sender, RoutedEventArgs e)
        {
            this.MostrarTodo();
            this.modo = ModoVerMarcadores.Todo;
        }

        private void btnForzarActualizacion_Click(object sender, RoutedEventArgs e)
        {
            this.ObtenerPosiciones();
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.actualizadorDePosiciones.Dispose();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.F) && (e.KeyboardDevice.Modifiers == ModifierKeys.Control))
                this.ObtenerPosiciones();
        }
    }

    public enum ModoVerMarcadores
    {
        Nada,
        Todo,
        Seleccionado
    }
}
