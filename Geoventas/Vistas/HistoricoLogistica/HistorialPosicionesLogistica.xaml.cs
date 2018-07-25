using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.Controladores.WorkerFleteros;
using GeoventasPocho.Vistas.ElementosMapa;
using GeoventasPocho.Vistas.ElementosMapa.Pines;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoventasPocho.Vistas.HistoricoLogistica
{
    /// <summary>
    /// Interaction logic for HistorialPosicionesLogistica.xaml
    /// </summary>
    public partial class HistorialPosicionesLogistica : Window
    {
        public HistorialPosicionesLogistica()
        {
            InitializeComponent();
            this.DataContext = this;

            this.Fleteros = new ObservableCollection<Fletero>();
            this.fleterosDelDia = new List<Fletero>();

            var mdp = ControladorMapa.ObtenerCordenadasPorDireccion("Mar del Plata, Buenos Aires");
            if (mdp.HasValue)
                this.mapa.Position = mdp.Value;
            else
                this.mapa.Position = new PointLatLng(-38.0042615, -57.6070055);

            this.listaDeElementos.SelectionChanged += ListaDeElementos_SelectionChanged;

            this.posicionesUC.dgPosiciones.SelectionChanged += dgPosiciones_SelectionChanged;
        }


        public ObservableCollection<Fletero> Fleteros
        {
            get { return (ObservableCollection<Fletero>)GetValue(FleterosProperty); }
            set { SetValue(FleterosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fleteros.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FleterosProperty =
            DependencyProperty.Register("Fleteros", typeof(ObservableCollection<Fletero>), typeof(HistorialPosicionesLogistica));



        public GridLength VisibilidadColumnaClientes
        {
            get { return (GridLength)GetValue(VisibilidadColumnaClientesProperty); }
            set { SetValue(VisibilidadColumnaClientesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibilidadColumnaClientes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilidadColumnaClientesProperty =
            DependencyProperty.Register("VisibilidadColumnaClientes", typeof(GridLength), typeof(HistorialPosicionesLogistica), new PropertyMetadata(new GridLength(0)));



        public ModoClientesRuteo ModoVerClientesConRuteo
        {
            get { return (ModoClientesRuteo)GetValue(ModoVerClientesConRuteoProperty); }
            set { SetValue(ModoVerClientesConRuteoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModoVerClientesConRuteo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoVerClientesConRuteoProperty =
            DependencyProperty.Register("ModoVerClientesConRuteo", typeof(ModoClientesRuteo), typeof(HistorialPosicionesLogistica), new PropertyMetadata(ModoClientesRuteo.Todos));



        public Boolean FleteroSeleccionado
        {
            get { return (Boolean)GetValue(FleteroSeleccionadoProperty); }
            set { SetValue(FleteroSeleccionadoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FleteroSeleccionado.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FleteroSeleccionadoProperty =
            DependencyProperty.Register("FleteroSeleccionado", typeof(Boolean), typeof(HistorialPosicionesLogistica), new PropertyMetadata(false));

        public Fletero fleteroElegido
        {
            get { return (Fletero)GetValue(fleteroElegidoProperty); }
            set { SetValue(fleteroElegidoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for fleteroElegido.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty fleteroElegidoProperty =
            DependencyProperty.Register("fleteroElegido", typeof(Fletero), typeof(HistorialPosicionesLogistica));

        List<Fletero> fleterosDelDia;
        public DateTime? diaSeleccionado { get; set; }

        private void dgPosiciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                var dg = (DataGrid)sender;
                var posicion = (ItemReporte)dg.SelectedItem;
                if (posicion != null)
                {
                    var fletero = this.Fleteros.FirstOrDefault(x => x.Codigo == posicion.CodigoVendedor);
                    var posiciones = fletero.Posiciones.Where(x => x.Fecha >= posicion.CheckIn && x.Fecha <= posicion.CheckOut).Select(p => new PointLatLng(p.Latitud, p.Longitud)).ToList();

                    this.mapa.Markers.Clear();

                    this.mapa.Markers.Add(ControladorMapa.CrearRuta(posiciones, Brushes.Green));
                    foreach (var punto in posiciones)
                    {
                        this.mapa.Markers.Add(ControladorMapa.CrearPuntoPosicion(punto, Brushes.PaleVioletRed));
                    }

                    if (posicion.Cliente != "VIAJE") //muestro el pin del cliente
                    {
                        var cli = fletero.Clientes.FirstOrDefault(c => c.Codigo == posicion.Cliente);
                        if (cli != null)
                        {
                            var marcador = new GMapMarker(cli.Coordenada.Value);

                            var pin = ControladorMapa.CrearPinClienteFletero(fletero.Posiciones.Where(x => x.Fecha >= posicion.CheckIn && x.Fecha <= posicion.CheckOut).ToList(), cli);

                            marcador.Shape = pin;
                            marcador.Shape.IsHitTestVisible = true;
                            marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                            marcador.ZIndex = 3;

                            this.mapa.Markers.Add(marcador);
                        }

                    }
                    ControladorMapa.RefrescarVista(this.mapa);
                }
            }
        }

        private void ListaDeElementos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.mapa.Markers.Clear();
            if (this.listaDeElementos.SelectedItem != null)
            {
                fleteroElegido = this.listaDeElementos.SelectedItem as Fletero;
                try
                {
                    SeleccionarMarcador(fleteroElegido);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                this.VisibilidadColumnaClientes = new GridLength(0);
        }

        private void SeleccionarMarcador(Fletero fletero)
        {
            var worker = new CargarPosicionesFleteroWorker(this.mapa, fletero, diaSeleccionado.Value, diaSeleccionado.Value.AddDays(1));
            worker.RunWorkerCompleted += cargaPosicionesFleteroWorker_Complete;
            worker.RunWorkerAsync();
            this.MostrarMarcador(fletero, true);
        }

        private void cargaPosicionesFleteroWorker_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var fletero = (Fletero)e.Result;
                var cargaClientesWorker = new CargarClientesFleteroWorker(this.mapa, fletero, diaSeleccionado.Value);
                cargaClientesWorker.RunWorkerCompleted += CargaClientesWorker_RunWorkerCompleted;
                cargaClientesWorker.RunWorkerAsync();
                ControladorMapa.ImprimirCamino(this.mapa, fletero.Posiciones);
                var reporte = ReportMaker.CrearReporte(fletero);
                this.posicionesUC.FechaDelReporte = this.dtpFecha.SelectedDate.Value.ToString("dddd, dd MMMM yyyy");
                this.posicionesUC.Posiciones = new ObservableCollection<ItemReporte>(reporte);
                this.posicionesUC.NombreVendedor = fletero.Nombre;
                this.posicionesUC.CodigoVendedor = fletero.Codigo;
                this.VisibilidadColumnaClientes = GridLength.Auto;
            }
        }

        private void CargaClientesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var flet = (Fletero)e.Result;
                ControladorMapa.ImprimirClientesFletero(this.mapa, flet.Posiciones, flet.Clientes);
            }
        }

        private void MostrarMarcador(Fletero fletero, bool clear = false)
        {
            if (clear)
                mapa.Markers.Clear();

            var marcador = new GMapMarker(fletero.CoordenadaActual);
            Pin pin = new PinAzul();
            pin.Tag = fletero;
            pin.Etiqueta = fletero.Codigo;

            var menuItem = new MenuItem();
            menuItem.Header = fletero.Nombre;
            pin.Menu.Items.Add(menuItem);

            pin.Menu.UpdateLayout();

            //pin.MouseDoubleClick += pin_MouseDoubleClick;

            marcador.Shape = pin;
            marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
            marcador.ZIndex = 4;
            this.mapa.Markers.Add(marcador);

            this.VerDomicilioVendedor(fletero);

            ControladorMapa.RefrescarVista(this.mapa);
        }

        private void VerDomicilioVendedor(Fletero fletero)
        {
            if (fletero.CoordenadaDomicilio.Lat != 0)
            {
                var marcador = new GMapMarker(fletero.CoordenadaDomicilio);
                var pin = new PinCasa();
                pin.Tag = fletero;
                pin.Etiqueta = string.Empty;
                marcador.Shape = pin;
                marcador.Shape.IsHitTestVisible = true;
                marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                pin.ToolTip = "Casa de " + fletero.Nombre;

                var menuItem = new MenuItem();
                menuItem.Header = fletero.Domicilio;
                pin.Menu.Items.Add(menuItem);

                pin.Menu.UpdateLayout();

                marcador.ZIndex = 3;
                this.mapa.Markers.Add(marcador);
            }
        }

        private void dtpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.fleterosDelDia = new List<Fletero>();
            fleteroElegido = null;
            diaSeleccionado = null;
            diaSeleccionado = dtpFecha.SelectedDate;
            if (diaSeleccionado != null)
            {
                try
                {
                    var worker = new CargarFleterosWorker(this.fleterosDelDia, diaSeleccionado.Value);
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    worker.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                this.fleterosDelDia = (List<Fletero>)e.Result;
                this.Fleteros = new ObservableCollection<Fletero>(this.fleterosDelDia);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ModoVerClientesConRuteoProperty)
            {
                if (listaDeElementos.SelectedItem != null)
                {
                    this.mapa.Markers.Clear();
                    this.SeleccionarMarcador((Fletero)listaDeElementos.SelectedItem);
                }

                //ControladorMapa.RefrescarVista(this.mapa);
            }
            if (e.Property == fleteroElegidoProperty)
            {
                if (fleteroElegido != null)
                    FleteroSeleccionado = true;
                else
                    FleteroSeleccionado = false;
            }
            base.OnPropertyChanged(e);
        }

        #region Filtro de Clientes
        private void btnTodos_Click(object sender, RoutedEventArgs e)
        {
            this.ModoVerClientesConRuteo = ModoClientesRuteo.Todos;
        }

        private void btnConRecorrido_Click(object sender, RoutedEventArgs e)
        {
            this.ModoVerClientesConRuteo = ModoClientesRuteo.ConRecorrido;
        }

        private void btnSinRecorrido_Click(object sender, RoutedEventArgs e)
        {
            this.ModoVerClientesConRuteo = ModoClientesRuteo.SinRecorrido;
        }

        private void btnOcultarClientes_Click(object sender, RoutedEventArgs e)
        {
            this.ModoVerClientesConRuteo = ModoClientesRuteo.Ninguno;
        }
        #endregion

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarMarcador(fleteroElegido);
        }
    }
}
