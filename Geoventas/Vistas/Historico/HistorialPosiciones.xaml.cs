using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.ServiceMobile;
using GeoventasPocho.Vistas.ElementosMapa;
using GeoventasPocho.Vistas.ElementosMapa.Pines;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace GeoventasPocho.Vistas.Historico
{
    /// <summary>
    /// Interaction logic for HistorialPosiciones.xaml
    /// </summary>
    public partial class HistorialPosiciones : Window
    {
        public ObservableCollection<Vendedor> Vendedores
        {
            get { return (ObservableCollection<Vendedor>)GetValue(VendedoresProperty); }
            set { SetValue(VendedoresProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Vendedores.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VendedoresProperty =
            DependencyProperty.Register("Vendedores", typeof(ObservableCollection<Vendedor>), typeof(HistorialPosiciones));



        public GridLength VisibilidadColumnaClientes
        {
            get { return (GridLength)GetValue(VisibilidadColumnaClientesProperty); }
            set { SetValue(VisibilidadColumnaClientesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibilidadColumnaClientes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilidadColumnaClientesProperty =
            DependencyProperty.Register("VisibilidadColumnaClientes", typeof(GridLength), typeof(HistorialPosiciones), new PropertyMetadata(new GridLength(0)));



        public ModoClientesRuteo ModoVerClientesConRuteo
        {
            get { return (ModoClientesRuteo)GetValue(ModoVerClientesConRuteoProperty); }
            set { SetValue(ModoVerClientesConRuteoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModoVerClientesConRuteo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoVerClientesConRuteoProperty =
            DependencyProperty.Register("ModoVerClientesConRuteo", typeof(ModoClientesRuteo), typeof(HistorialPosiciones), new PropertyMetadata(ModoClientesRuteo.Todos));



        public DateTime? diaSeleccionado { get; set; }

        ServiceSoapClient servicioMobile;

        public HistorialPosiciones()
        {
            InitializeComponent();
            this.DataContext = this;

            this.Vendedores = new ObservableCollection<Vendedor>();

            this.servicioMobile = new ServiceSoapClient();

            var mdp = ControladorMapa.ObtenerCordenadasPorDireccion("Mar del Plata, Buenos Aires");
            if (mdp.HasValue)
                this.mapa.Position = mdp.Value;
            else
                this.mapa.Position = new PointLatLng(-38.0042615, -57.6070055);

            this.listaDeElementos.SelectionChanged += ListaDeElementos_SelectionChanged;

            this.posicionesUC.dgPosiciones.SelectionChanged += dgPosiciones_SelectionChanged;

        }

        private void dgPosiciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                var dg = (DataGrid)sender;
                var posicion = (ItemReporte)dg.SelectedItem;
                if (posicion != null)
                {
                    var vendedor = this.Vendedores.FirstOrDefault(x => x.Codigo == posicion.CodigoVendedor);
                    var posiciones = vendedor.Posiciones.Where(x => x.Fecha >= posicion.CheckIn && x.Fecha <= posicion.CheckOut).Select(p => new PointLatLng(p.Latitud, p.Longitud)).ToList();

                    this.mapa.Markers.Clear();

                    this.mapa.Markers.Add(ControladorMapa.CrearRuta(posiciones, Brushes.Green));
                    foreach (var punto in posiciones)
                    {
                        this.mapa.Markers.Add(ControladorMapa.CrearPuntoPosicion(punto, Brushes.PaleVioletRed));
                    }

                    if (posicion.Cliente != "VIAJE") //muestro el pin del cliente
                    {
                        foreach (var zona in vendedor.Zonas)
                        {
                            var cli = zona.Clientes.FirstOrDefault(c => c.Codigo == posicion.Cliente);
                            if (cli != null)
                            {
                                var marcador = new GMapMarker(cli.Coordenada.Value);

                                var pin = ControladorMapa.CrearPinCliente(null, cli);

                                marcador.Shape = pin;
                                marcador.Shape.IsHitTestVisible = true;
                                marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                                marcador.ZIndex = 3;

                                this.mapa.Markers.Add(marcador);
                                break;
                            }
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
                var vendedor = this.listaDeElementos.SelectedItem as Vendedor;
                try
                {
                    SeleccionarMarcador(vendedor);
                    var reporte = ReportMaker.CrearReporte(vendedor);
                    this.posicionesUC.FechaDelReporte = this.dtpFecha.SelectedDate.Value.ToString("dddd, dd MMMM yyyy");
                    this.posicionesUC.Posiciones = new ObservableCollection<ItemReporte>(reporte);
                    this.posicionesUC.NombreVendedor = vendedor.Nombre;
                    this.posicionesUC.CodigoVendedor = vendedor.Codigo;

                    this.VisibilidadColumnaClientes = GridLength.Auto;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                this.VisibilidadColumnaClientes = new GridLength(0);
        }

        private void SeleccionarMarcador(Vendedor vendedor)
        {
            if (vendedor.Zonas.Count == 0)
                ControladorZonas.CargarZonasDelVendedor(vendedor, diaSeleccionado.Value);
            vendedor.CantidadClientes = 0;
            foreach (var zona in vendedor.Zonas)
            {
                ControladorZonas.CargarClientes(zona, this.ModoVerClientesConRuteo);
                vendedor.CantidadClientes += zona.Clientes.Count;
            }
            ControladorVendedores.CalcularBultosYPesos(vendedor, diaSeleccionado.Value, diaSeleccionado.Value.AddDays(1));
            this.MostrarMarcador(vendedor, true);
        }

        private void MostrarMarcador(Vendedor vendedorMapa, bool clear = false)
        {
            if (clear)
                mapa.Markers.Clear();

            var marcador = new GMapMarker(vendedorMapa.CoordenadaActual);
            Pin pin;
            if (vendedorMapa.CodigoEmpresa == "10")
                pin = new PinNaranja();
            else
                pin = new PinAzul();
            pin.Tag = vendedorMapa;
            pin.Etiqueta = vendedorMapa.Codigo;

            var menuItem = new MenuItem();
            menuItem.Header = vendedorMapa.Nombre;
            pin.Menu.Items.Add(menuItem);

            pin.Menu.UpdateLayout();

            //pin.MouseDoubleClick += pin_MouseDoubleClick;

            marcador.Shape = pin;
            marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
            marcador.ZIndex = 4;
            this.mapa.Markers.Add(marcador);

            try
            {
                this.VerZonasDelVendedor(vendedorMapa);
                this.VerClientesDelVendedor(vendedorMapa);
                this.VerCaminoDelVendedor(vendedorMapa);
                this.VerDomicilioVendedor(vendedorMapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            ControladorMapa.RefrescarVista(this.mapa);
        }

        private void VerDomicilioVendedor(Vendedor vendedorMapa)
        {
            if (vendedorMapa.CoordenadaDomicilio.Lat != 0)
            {
                var marcador = new GMapMarker(vendedorMapa.CoordenadaDomicilio);
                var pin = new PinCasa();
                pin.Tag = vendedorMapa;
                pin.Etiqueta = string.Empty;
                marcador.Shape = pin;
                marcador.Shape.IsHitTestVisible = true;
                marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                pin.ToolTip = "Casa de " + vendedorMapa.Nombre;

                var menuItem = new MenuItem();
                menuItem.Header = vendedorMapa.Calle + " " + vendedorMapa.Numero;
                pin.Menu.Items.Add(menuItem);

                pin.Menu.UpdateLayout();

                marcador.ZIndex = 3;
                this.mapa.Markers.Add(marcador);
            }
        }

        private void VerCaminoDelVendedor(Vendedor v)
        {
            this.mapa.Markers.Add(ControladorMapa.CrearRuta(v.Posiciones.Where(p => p.Latitud != 0 && p.Longitud != 0).Select(x => new PointLatLng(x.Latitud, x.Longitud)).ToList(), Brushes.Red));
            foreach (var pos in v.Posiciones)
            {
                if (pos.Latitud != 0 && pos.Longitud != 0)
                    this.mapa.Markers.Add(ControladorMapa.CrearPuntoPosicion(pos));
            }
        }

        private void VerClientesDelVendedor(Vendedor vendedor)
        {
            if (vendedor.Posiciones.Count == 0)
                ControladorVendedores.CargarPosiciones(vendedor, diaSeleccionado.Value, diaSeleccionado.Value.AddDays(1));
            foreach (var zona in vendedor.Zonas)
            {
                foreach (var cli in zona.Clientes)
                {
                    var marcador = new GMapMarker(cli.Coordenada.Value);

                    var pin = ControladorMapa.CrearPinCliente(vendedor.Posiciones, cli);

                    pin.Menu.UpdateLayout();

                    marcador.Shape = pin;
                    marcador.Shape.IsHitTestVisible = true;
                    marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                    marcador.ZIndex = 3;

                    this.mapa.Markers.Add(marcador);
                }

            }
        }

        private void VerZonasDelVendedor(Vendedor vendedorMapa)
        {
            foreach (var zona in vendedorMapa.Zonas) //dibujo
            {
                if (zona.Vertices.Count == 0) //cargo los vertices si es que no los cargué aun.
                    zona.Vertices = ControladorZonas.ObtenerVerticesZona(zona.Codigo, zona.CodigoEmpresa, zona.CodigoDivision);
                this.mapa.Markers.Add(ControladorMapa.CrearPoligonoZona(zona.Vertices, zona.CodigoEmpresa == "01" ? Brushes.LightBlue : Brushes.Orange, zona.Codigo));
            }
        }

        private void dtpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            diaSeleccionado = null;
            this.Vendedores.Clear();
            diaSeleccionado = dtpFecha.SelectedDate;
            if (diaSeleccionado != null)
            {
                try
                {
                    var dt = servicioMobile.ObtenerVendedoresPorDia(diaSeleccionado.Value, diaSeleccionado.Value.AddDays(1));

                    foreach (DataRow item in dt.AsEnumerable().Where(p => p.Field<string>("usuario") != ""))
                    {
                        Func<object, dynamic, object> nonull = (p, def) => p == null ? def : p;
                        var vendedor = new Vendedor()
                        {
                            Codigo = item.Field<string>("usuario"),
                            CodigoEmpresa = item.Field<string>("empresa"), //pertenece a Vendedores, no a PosicionGps
                            Visitados = (int)nonull(item.Field<object>("visitados"), 0),
                            //Compradores = (int)nonull(item.Field<object>("compradores"), 0),
                            //Bultos = (int)nonull(item.Field<object>("bultos"), 0),
                            Posiciones = new List<Posicion>(),
                            FondoDeCelda = item.Field<string>("empresa") == "10" ? @"S:\GEOVENTAS\alta.jpg" : @"S:\GEOVENTAS\hiller.jpg"
                        };
                        if (vendedor.Codigo != null)
                            this.AgregarVendedor(vendedor);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AgregarVendedor(Vendedor vendedorMapa)
        {
            var v = Vendedores.FirstOrDefault(e => e.Codigo == vendedorMapa.Codigo);
            if (v == null)
            {
                var prev = ControladorVendedores.ObtenerVendedor(vendedorMapa.Codigo);
                if (prev != null)
                {
                    vendedorMapa.Foto = prev.Foto;
                    vendedorMapa.Nombre = prev.Nombre;
                    vendedorMapa.CoordenadaDomicilio = new PointLatLng(prev.CoordenadaDomicilio.Lat, prev.CoordenadaDomicilio.Lng);
                }

                if (vendedorMapa.CoordenadaActual.Lat == 0 && vendedorMapa.CoordenadaActual.Lng == 0) //si lat y lng vienen 0, 0 es porque tiene gps apagado
                {
                    vendedorMapa.CoordenadaActual = new PointLatLng(-38.002452, -57.601936);
                    vendedorMapa.Estado = Estado.GPS_APAGADO;
                }

                this.Vendedores.Add(vendedorMapa);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ModoVerClientesConRuteoProperty)
            {
                if (listaDeElementos.SelectedItem != null)
                {
                    this.mapa.Markers.Clear();
                    this.SeleccionarMarcador((Vendedor)listaDeElementos.SelectedItem);
                }

                //ControladorMapa.RefrescarVista(this.mapa);
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
    }
}
