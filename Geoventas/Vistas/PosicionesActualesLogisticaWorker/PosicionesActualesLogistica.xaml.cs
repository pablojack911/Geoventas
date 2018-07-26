using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.Controladores.WorkerFleteros;
using GeoventasPocho.Vistas.ElementosMapa;
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
using System.ComponentModel;
using GeoventasPocho.Factory;
using GeoventasPocho.Factory.Pines;

namespace GeoventasPocho.Vistas.PosicionesActualesLogisticaWorker
{
    public partial class PosicionesActualesLogistica : Window
    {
        public ObservableCollection<Fletero> Fleteros { get; set; }
        List<Fletero> fleterosDelDia;
        Fletero fleteroSeleccionado;

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
            DependencyProperty.Register("FiltroDeLista", typeof(string), typeof(PosicionesActualesLogistica), new PropertyMetadata("Codigo"));
        private ModoVerMarcadores modo;
        //private CargarClientesFleteroWorker cargarClientesWorker;
        //private CargarPosicionesFleteroWorker cargaPosicionesWorker;

        //public ICommand CmdVerClientesPorRuta { get; set; }
        public ICommand CmdDibujarZona { get; set; }
        //public ICommand CmdVerCaminoPreventista { get; set; }
        public ICommand CmdVerificarPreventistaDentroDeZona { get; set; }
        public ICommand CmdVerDomicilioFletero { get; set; }

        public ICommand CmdOcultarClientesDeLaRuta { get; set; }
        public ICommand CmdOcultarCamino { get; set; }


        public PosicionesActualesLogistica()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Fleteros = new ObservableCollection<Fletero>();
            this.fleterosDelDia = new List<Fletero>();

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

            //this.CmdVerClientesPorRuta = new RelayCommand(e => this.VerClientesDelVendedor(e));
            //this.CmdVerCaminoPreventista = new RelayCommand(e => this.VerCaminoDelVendedor(e));
            //this.CmdVerificarPreventistaDentroDeZona = new RelayCommand(e => this.VerificarPreventistaDentroDeZona(e));
            this.CmdVerDomicilioFletero = new RelayCommand(e => this.VerDomicilioFletero(e));

            this.CmdOcultarClientesDeLaRuta = new RelayCommand(e => this.OcultarClientesDeLaRuta(e));
            this.CmdOcultarCamino = new RelayCommand(e => this.OcultarCamino(e));

            this.ObtenerPosiciones();
        }


        private void listaDeElementos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listaDeElementos.SelectedItem != null)
            {
                var flet = this.listaDeElementos.SelectedItem as Fletero;
                if (fleteroSeleccionado == null || (!flet.Codigo.Equals(fleteroSeleccionado.Codigo)))
                {
                    //this.cargarClientesWorker?.CancelAsync();
                    //this.cargaPosicionesWorker?.CancelAsync();
                    fleteroSeleccionado = flet;
                    //fleteroSeleccionado.VerClientes = false;
                    //item.VerZona = false;
                    //fleteroSeleccionado.VerTodasLasPosiciones = false;
                    //item.VerDomicilioDelVendedor = false;
                    SeleccionarMarcador(fleteroSeleccionado);
                    this.modo = ModoVerMarcadores.Seleccionado;
                }
            }
        }

        private void SeleccionarMarcador(Fletero item)
        {
            item.VerClientes = true;
            item.VerTodasLasPosiciones = true;
            MostrarMarcador(item, true);
            mapa.Position = item.CoordenadaActual;
        }

        private void MostrarTodo()
        {
            this.mapa.Markers.Clear();
            foreach (var item in this.Fleteros)
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
            //if (listaDeElementos.Items.Contains(fleteroMapa) && this.modo == ModoVerMarcadores.Seleccionado)
            //    listaDeElementos.SelectedItem = fleteroMapa;
            var marcador = new GMapMarker(fleteroMapa.CoordenadaActual);
            //Pin pin = new PinAzul();
            //pin.Etiqueta = fleteroMapa.Codigo;
            var pin = PinFactory.MakePin(Factory.Pines.TipoPin.Azul, fleteroMapa.Codigo);
            pin.Tag = fleteroMapa;
            var menuMostrarDomicilio = new MenuItem
            {
                Header = "Mostrar domicilio del fletero",
                Command = this.CmdVerDomicilioFletero,
                CommandParameter = fleteroMapa
            };
            pin.ContextMenu.Items.Add(menuMostrarDomicilio);

            pin.ContextMenu.UpdateLayout();

            //pin.MouseDoubleClick += pin_MouseDoubleClick;

            marcador.Shape = pin;
            marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
            marcador.ZIndex = 4;
            this.mapa.Markers.Add(marcador);

            if (fleteroMapa.VerClientes)
                if (fleteroMapa.Clientes.Count > 0)
                    ControladorMapa.ImprimirClientesFletero(this.mapa, fleteroMapa.Posiciones, fleteroMapa.Clientes);
            //else
            //    this.VerClientesDelVendedor(fleteroMapa);
            if (fleteroMapa.VerTodasLasPosiciones)
                if (fleteroMapa.Posiciones.Count > 0)
                    ControladorMapa.ImprimirCamino(this.mapa, fleteroMapa.Posiciones);
            //else
            //    this.VerCaminoDelVendedor(fleteroMapa);
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
                    var pin = PinFactory.MakePin(TipoPin.Casa);
                    pin.Tag = f;
                    marcador.Shape = pin;
                    marcador.Shape.IsHitTestVisible = true;
                    marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                    pin.ToolTip = "Casa de " + f.Nombre;

                    var menuItem = new MenuItem
                    {
                        Header = f.Domicilio
                    };
                    pin.ContextMenu.Items.Add(menuItem);
                    pin.ContextMenu.UpdateLayout();

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

        //private object VerClientesDelVendedor(object fleteroMapa)
        //{
        //    if (fleteroMapa == null)
        //        return false;
        //    try
        //    {
        //        fleteroSeleccionado = fleteroMapa as Fletero;
        //        fleteroSeleccionado.VerClientes = true;
        //        if (fleteroSeleccionado.Clientes.Count == 0)
        //        {
        //            this.cargarClientesWorker?.CancelAsync();
        //            this.cargarClientesWorker = new CargarClientesFleteroWorker(this.mapa, fleteroSeleccionado, FechaDesde);
        //            cargarClientesWorker.RunWorkerCompleted += CargarClientesWorker_RunWorkerCompleted;
        //            cargarClientesWorker.RunWorkerAsync();
        //        }
        //        else
        //        {
        //            MostrarMarcador(fleteroSeleccionado, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    return true;
        //}

        private void CargarClientesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                fleteroSeleccionado = (Fletero)e.Result;
                ControladorMapa.ImprimirClientesFletero(this.mapa, fleteroSeleccionado.Posiciones, fleteroSeleccionado.Clientes);
            }
        }

        private object OcultarClientesDeLaRuta(object fleteroMapa)
        {
            if (fleteroMapa == null)
                return false;
            fleteroSeleccionado = fleteroMapa as Fletero;
            fleteroSeleccionado.VerClientes = false;
            this.MostrarMarcador(fleteroSeleccionado, true);
            return true;
        }

        //private object VerCaminoDelVendedor(object fleteroMapa)
        //{
        //    if (fleteroMapa == null)
        //        return false;
        //    try
        //    {
        //        fleteroSeleccionado = fleteroMapa as Fletero;
        //        fleteroSeleccionado.VerTodasLasPosiciones = true;
        //        if (fleteroSeleccionado.Posiciones.Count == 0)
        //        //    ControladorFleteros.CargarPosiciones(fleteroSeleccionado, FechaDesde, FechaHasta);
        //        //this.mapa.Markers.Add(ControladorMapa.CrearRuta(fleteroSeleccionado.Posiciones.Where(p => p.Latitud != 0 && p.Longitud != 0).Select(x => new PointLatLng(x.Latitud, x.Longitud)).ToList(), Brushes.Red));
        //        //foreach (var pos in fleteroSeleccionado.Posiciones)
        //        //{
        //        //    if (pos.Latitud != 0 && pos.Longitud != 0)
        //        //        this.mapa.Markers.Add(ControladorMapa.CrearPuntoPosicion(pos));
        //        //}
        //        //ControladorMapa.RefrescarVista(this.mapa);
        //        {
        //            this.cargaPosicionesWorker?.CancelAsync();
        //            this.cargaPosicionesWorker = new CargarPosicionesFleteroWorker(this.mapa, fleteroSeleccionado, FechaDesde, FechaHasta);
        //            cargaPosicionesWorker.RunWorkerCompleted += CargaPosicionesWorker_RunWorkerCompleted;
        //            cargaPosicionesWorker.RunWorkerAsync();
        //        }
        //        else
        //        {
        //            MostrarMarcador(fleteroSeleccionado, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    return true;
        //}

        private object OcultarCamino(object fleteroMapa)
        {
            if (fleteroMapa == null)
                return false;
            fleteroSeleccionado = fleteroMapa as Fletero;
            fleteroSeleccionado.VerTodasLasPosiciones = false;
            this.MostrarMarcador(fleteroSeleccionado, true);
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
                    var worker = new CargarFleterosWorker(this.fleterosDelDia, FechaDesde);
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    worker.RunWorkerAsync();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            });
            this.actualizadorDePosiciones.Start();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                this.fleterosDelDia = (List<Fletero>)e.Result;
                this.Fleteros.Clear();
                foreach (var item in this.fleterosDelDia)
                {
                    this.Fleteros.Add(item);
                }

                switch (this.modo)
                {
                    case ModoVerMarcadores.Nada:
                        break;
                    case ModoVerMarcadores.Todo:
                        this.MostrarTodo();
                        break;
                    case ModoVerMarcadores.Seleccionado:
                        this.MostrarMarcador(fleteroSeleccionado, true);
                        break;
                    default:
                        break;
                }
                foreach (var item in this.Fleteros)
                {
                    try
                    {
                        //this.cargaPosicionesWorker?.CancelAsync();
                        //this.cargaPosicionesWorker = new CargarPosicionesFleteroWorker(this.mapa, item, FechaDesde, FechaHasta);
                        //cargaPosicionesWorker.RunWorkerCompleted += CargaPosicionesWorker_RunWorkerCompleted;
                        //cargaPosicionesWorker.RunWorkerAsync();
                        var cargaPosicionesWorker = new CargarPosicionesFleteroWorker(this.mapa, item, FechaDesde, FechaHasta);
                        cargaPosicionesWorker.RunWorkerCompleted += CargaPosicionesWorker_RunWorkerCompleted;
                        cargaPosicionesWorker.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void CargaPosicionesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                try
                {
                    var flet = (Fletero)e.Result;
                    //var cargarClientesWorker?.CancelAsync();
                    var cargarClientesWorker = new CargarClientesFleteroWorker(this.mapa, flet, FechaDesde);
                    cargarClientesWorker.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
            this.modo = ModoVerMarcadores.Todo;
            this.listaDeElementos.SelectedItem = null;
            this.listaDeElementos.SelectedIndex = -1;
            fleteroSeleccionado = null;
            this.MostrarTodo();
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