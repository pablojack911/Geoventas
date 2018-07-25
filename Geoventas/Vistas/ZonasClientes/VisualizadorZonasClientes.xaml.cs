using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.Vistas.Converters;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeoventasPocho.Vistas.ZonasClientes
{
    /// <summary>
    /// Interaction logic for VerTodasLasZonas.xaml
    /// </summary>
    public partial class VisualizadorZonasClientes : Window
    {
        public ObservableCollection<Zona> TodasLasZonas { get; set; }

        public ObservableCollection<Zona> Zonas
        {
            get { return (ObservableCollection<Zona>)GetValue(ZonasProperty); }
            set { SetValue(ZonasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Zonas.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZonasProperty =
            DependencyProperty.Register("Zonas", typeof(ObservableCollection<Zona>), typeof(VisualizadorZonasClientes));

        public SelectionMode ModoSeleccion
        {
            get { return (SelectionMode)GetValue(ModoSeleccionProperty); }
            set { SetValue(ModoSeleccionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModoSeleccion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoSeleccionProperty =
            DependencyProperty.Register("ModoSeleccion", typeof(SelectionMode), typeof(VisualizadorZonasClientes), new PropertyMetadata(SelectionMode.Single));

        public GridLength VisibilidadColumnaClientes
        {
            get { return (GridLength)GetValue(VisibilidadColumnaClientesProperty); }
            set { SetValue(VisibilidadColumnaClientesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibilidadColumnaClientes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilidadColumnaClientesProperty =
            DependencyProperty.Register("VisibilidadColumnaClientes", typeof(GridLength), typeof(VisualizadorZonasClientes), new PropertyMetadata(new GridLength(0)));

        public ModoClientesRuteo ModoVerClientesConRuteo
        {
            get { return (ModoClientesRuteo)GetValue(ModoVerClientesConRuteoProperty); }
            set { SetValue(ModoVerClientesConRuteoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModoVerClientesConRuteo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoVerClientesConRuteoProperty =
            DependencyProperty.Register("ModoVerClientesConRuteo", typeof(ModoClientesRuteo), typeof(VisualizadorZonasClientes), new PropertyMetadata(ModoClientesRuteo.ConRecorrido));


        public ICommand CmdMoverUbicacionDeClienteManualmente { get; set; }

        public GMapMarker clienteAMover { get; set; }
        public Zona ZonaSeleccionada { get; set; }

        ColorEmpresaConverter colorEmpresaConverter;

        public VisualizadorZonasClientes()
        {
            InitializeComponent();
            this.DataContext = this;

            colorEmpresaConverter = new ColorEmpresaConverter();

            this.CmdMoverUbicacionDeClienteManualmente = new RelayCommand(e => this.MoverUbicacionDeClienteManualmente(e));

            List<Zona> zonas = new List<Zona>();
            try
            {
                zonas = ControladorZonas.ObtenerZonas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nEsta ventana se cerrará.");
                this.Close();
            }
            this.TodasLasZonas = new ObservableCollection<Zona>(zonas);
            this.Zonas = new ObservableCollection<Zona>(this.TodasLasZonas);
            this.listaDeZonas.SelectionChanged += listaDeZonas_SelectionChanged;

            var mdp = ControladorMapa.ObtenerCordenadasPorDireccion("Mar del Plata, Buenos Aires");
            if (mdp.HasValue)
                this.mapa.Position = mdp.Value;
            else
                this.mapa.Position = new PointLatLng(-38.0042615, -57.6070055);

            this.clientesUC.dgClientes.SelectionChanged += DgClientes_SelectionChanged;

            this.mapa.MouseRightButtonDown += Mapa_MouseRightButtonDown;

            this.KeyDown += VisualizadorZonasClientes_KeyDown;

        }

        private void VisualizadorZonasClientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                if (this.clienteAMover != null)
                    if (this.ZonaSeleccionada != null)
                    {
                        this.clienteAMover = null;
                        this.SeleccionarZona(this.ZonaSeleccionada);
                    }
        }

        private void Mapa_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.clienteAMover != null)
            {
                var mapa = (GMapControl)sender;
                Point p = e.GetPosition(mapa);
                var nuevaposicion = mapa.FromLocalToLatLng((int)p.X, (int)p.Y);
                //GMapMarker m = new GMapMarker(newRoutePoints.Last());
                //var nuevaposicion = mapa.Position;

                var cli = (Cliente)clienteAMover.Tag;

                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    try
                    {
                        var dir = GoogleMapsGeocoder.ObtenerDireccionPorCoordenadas(nuevaposicion);
                        var res = MessageBox.Show("¿Seleccionar esta posición como nueva ubicación para el cliente " + cli.Codigo + " - " + cli.Nombre + "?\n\nUbicación aproximada: " + dir, "Cambiando ubicación del cliente", MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            ControladorClientes.ActualizarLatLngCliente(cli.Codigo, nuevaposicion, null, 0);
                            this.clienteAMover = null;
                            this.SeleccionarZona(this.ZonaSeleccionada);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo obtener una dirección." + ex.Message);
                    }
                }
            }
        }

        private void DgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                var dg = (DataGrid)sender;
                var cliente = (Cliente)dg.SelectedItem;
                if (cliente != null)
                {
                    this.mapa.Position = cliente.Coordenada.Value;
                }
            }
        }

        private void listaDeZonas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.clienteAMover = null;
            try
            {
                this.mapa.Markers.Clear();
                if (this.listaDeZonas.SelectedItem != null)
                {
                    foreach (var zona in listaDeZonas.SelectedItems)
                    {
                        var zonaMapa = zona as Zona;
                        this.SeleccionarZona(zonaMapa, false);
                    }
                }
                ControladorMapa.RefrescarVista(this.mapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SeleccionarZona(Zona zonaMapa, bool clear = true)
        {
            if (clear)
                this.mapa.Markers.Clear();

            this.DibujarZona(zonaMapa);

            zonaMapa.Clientes.Clear();
            ControladorZonas.CargarClientes(zonaMapa, this.ModoVerClientesConRuteo);

            this.VerClientes(zonaMapa.Clientes);

            if (this.ModoSeleccion == SelectionMode.Single)
            {
                this.VisibilidadColumnaClientes = GridLength.Auto;
                this.clientesUC.Clientes = new ObservableCollection<Cliente>(zonaMapa.Clientes);
                this.clientesUC.dgClientes.Items.Filter = null;
                this.clientesUC.cantidadClientes = this.clientesUC.Clientes.Count;
                this.ZonaSeleccionada = zonaMapa;
            }
            else
            {
                this.VisibilidadColumnaClientes = new GridLength(0);
            }
        }

        private void DibujarZona(Zona zonaMapa)
        {
            var tag = zonaMapa.Codigo + "-" + zonaMapa.CodigoDivision + "-" + zonaMapa.CodigoEmpresa;
            var color = (Brush)colorEmpresaConverter.Convert(zonaMapa.CodigoEmpresa, null, null, null);
            if (zonaMapa.Vertices.Count == 0)
                zonaMapa.Vertices = ControladorZonas.ObtenerVerticesZona(zonaMapa.Codigo, zonaMapa.CodigoEmpresa, zonaMapa.CodigoDivision);
            if (!this.mapa.Markers.Any(z => z.ZIndex == 0 && z.Tag.ToString() == tag))
                this.mapa.Markers.Add(ControladorMapa.CrearPoligonoZona(zonaMapa.Vertices, color, tag));
        }

        private void VerClientes(List<Cliente> clientes)
        {
            foreach (var cli in clientes)
            {
                var marcador = new GMapMarker(cli.Coordenada.Value);

                var pin = ControladorMapa.CrearPinCliente(null, cli);

                marcador.Tag = cli;
                marcador.Shape = pin;
                marcador.Shape.IsHitTestVisible = true;
                marcador.Offset = new Point(-pin.Width / 2, -pin.Height);

                if (this.ModoSeleccion == SelectionMode.Single)
                {
                    var menuMover = new MenuItem();
                    menuMover.Header = "Ubicar manualmente en el mapa";
                    menuMover.Command = this.CmdMoverUbicacionDeClienteManualmente;
                    menuMover.CommandParameter = marcador;
                    pin.Menu.Items.Add(menuMover);
                }

                pin.Menu.UpdateLayout();

                marcador.ZIndex = 3;

                this.mapa.Markers.Add(marcador);

            }
            ControladorMapa.RefrescarVista(this.mapa);
        }

        private object MoverUbicacionDeClienteManualmente(object cliente)
        {
            if (cliente == null)
                return false;
            try
            {
                var cli = cliente as GMapMarker;
                this.mapa.Markers.Clear();
                this.DibujarZona(this.ZonaSeleccionada);
                this.mapa.Markers.Add(cli);
                this.clientesUC.dgClientes.Items.Filter = c => c == cli.Tag;
                this.clienteAMover = cli;
                ControladorMapa.RefrescarVista(this.mapa);
                MessageBox.Show("Muévase por el mapa y haga clic derecho sobre la nueva ubicación de este cliente.", "Preparándose para mover el cliente");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ModoVerClientesConRuteoProperty)
            {
                if (this.ModoSeleccion == SelectionMode.Single)
                    if (listaDeZonas.SelectedItem != null)
                    {
                        this.ZonaSeleccionada = (Zona)listaDeZonas.SelectedItem;
                        this.SeleccionarZona(this.ZonaSeleccionada);
                    }
                    else
                    {
                        this.mapa.Markers.Clear();
                        foreach (var zona in listaDeZonas.SelectedItems)
                        {
                            var zonaMapa = zona as Zona;
                            this.SeleccionarZona(zonaMapa, false); //ya hice el clear antes.
                        }
                    }
                ControladorMapa.RefrescarVista(this.mapa);
            }
            base.OnPropertyChanged(e);
        }

        #region Menu Ver (actualmente oculto)
        private void btnchkVerZonasAlta_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnchkVerZonasHiller_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnchkVerZonasAutoservicio_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnchkVerZonasTradicionales_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnchkVerZonasRefrigerados_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMostrarTodos_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region MENU FILTRAR 

        private void btnchkFiltrarZonasAlta_Click(object sender, RoutedEventArgs e)
        {
            this.Zonas = new ObservableCollection<Zona>(this.TodasLasZonas.Where(z => z.CodigoEmpresa == "10"));
        }

        private void btnchkFiltrarZonasHiller_Click(object sender, RoutedEventArgs e)
        {
            this.Zonas = new ObservableCollection<Zona>(this.TodasLasZonas.Where(z => z.CodigoEmpresa == "01"));
        }

        private void btnchkFiltrarZonasAutoservicio_Click(object sender, RoutedEventArgs e)
        {
            this.Zonas = new ObservableCollection<Zona>(this.TodasLasZonas.Where(z => z.Codigo.StartsWith("A")));
        }

        private void btnchkFiltrarZonasTradicionales_Click(object sender, RoutedEventArgs e)
        {
            this.Zonas = new ObservableCollection<Zona>(this.TodasLasZonas.Where(z => !z.Codigo.StartsWith("A") && !z.Codigo.EndsWith("R")));
        }

        private void btnchkFiltrarZonasRefrigerados_Click(object sender, RoutedEventArgs e)
        {
            this.Zonas = new ObservableCollection<Zona>(this.TodasLasZonas.Where(z => z.Codigo.EndsWith("R")));
        }

        private void btnchkFiltrarTODO_Click(object sender, RoutedEventArgs e)
        {
            this.Zonas = new ObservableCollection<Zona>(this.TodasLasZonas);
        }

        #endregion

        #region Modo seleccion
        private void btnIndividual_Click(object sender, RoutedEventArgs e)
        {
            this.ModoSeleccion = SelectionMode.Single;
        }

        private void btnMultiple_Click(object sender, RoutedEventArgs e)
        {
            this.ModoSeleccion = SelectionMode.Multiple;
            this.VisibilidadColumnaClientes = new GridLength(0);
        }
        #endregion

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
    }
}
