using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.Factory;
using GeoventasPocho.Factory.Pines;
using GeoventasPocho.ServiceMobile;
using GeoventasPocho.Vistas.ElementosMapa;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace GeoventasPocho.Vistas.PosicionesActuales
{
    /// <summary>
    /// Interaction logic for MapaPosicionesActuales.xaml
    /// </summary>
    public partial class MapaPosicionesActuales : Window
    {
        public ObservableCollection<Vendedor> Vendedores { get; set; }

        ServiceSoapClient servicioMobile;

        ModoVerMarcadores modo;

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
            DependencyProperty.Register("FiltroDeLista", typeof(string), typeof(MapaPosicionesActuales), new PropertyMetadata("Codigo"));



        public ModoClientesRuteo ModoVerClientesConRuteo
        {
            get { return (ModoClientesRuteo)GetValue(ModoVerClientesConRuteoProperty); }
            set { SetValue(ModoVerClientesConRuteoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModoVerClientesConRuteo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoVerClientesConRuteoProperty =
            DependencyProperty.Register("ModoVerClientesConRuteo", typeof(ModoClientesRuteo), typeof(MapaPosicionesActuales), new PropertyMetadata(ModoClientesRuteo.ConRecorrido));



        public ICommand CmdVerClientesPorRuta { get; set; }
        public ICommand CmdDibujarZona { get; set; }
        public ICommand CmdVerCaminoPreventista { get; set; }
        public ICommand CmdVerificarPreventistaDentroDeZona { get; set; }
        public ICommand CmdVerDomicilioVendedor { get; set; }


        public MapaPosicionesActuales()
        {
            InitializeComponent();
            this.DataContext = this;
            this.servicioMobile = new ServiceSoapClient();
            this.Vendedores = new ObservableCollection<Vendedor>();

            this.FechaDesde = DateTime.Today.AddHours(8);
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
            this.CmdDibujarZona = new RelayCommand(e => this.VerZonasDelVendedor(e));
            this.CmdVerCaminoPreventista = new RelayCommand(e => this.VerCaminoDelVendedor(e));
            //this.CmdVerificarPreventistaDentroDeZona = new RelayCommand(e => this.VerificarPreventistaDentroDeZona(e));
            this.CmdVerDomicilioVendedor = new RelayCommand(e => this.VerDomicilioVendedor(e));

        }


        private void listaDeElementos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.listaDeElementos.SelectedItem as Vendedor;
            item.VerClientes = false;
            item.VerZona = false;
            item.VerTodasLasPosiciones = false;
            item.VerDomicilioDelVendedor = false;
            SeleccionarMarcador(item);
            this.modo = ModoVerMarcadores.Seleccionado;
        }

        private void SeleccionarMarcador(Vendedor item)
        {
            MostrarMarcador(item, true);
            mapa.Position = item.CoordenadaActual;
        }

        private void MostrarTodo()
        {
            this.mapa.Markers.Clear();
            foreach (var item in this.Vendedores.Where(v => v.Visible == true))
            {
                item.VerClientes = false;
                item.VerZona = false;
                item.VerTodasLasPosiciones = false;
                item.VerDomicilioDelVendedor = false;
                this.MostrarMarcador(item);
            }
        }

        private void MostrarMarcador(Vendedor vendedorMapa, bool clear = false)
        {
            if (clear)
                mapa.Markers.Clear();

            var marcador = new GMapMarker(vendedorMapa.CoordenadaActual);
            Pin pin=PinFactory.MakePin();
            if (vendedorMapa.CodigoEmpresa == "10")
                pin.setTipo(TipoPin.Naranja);
            else
                pin.setTipo(TipoPin.Azul);
            pin.Tag = vendedorMapa;
            pin.setEtiqueta(vendedorMapa.Codigo);

            var menuItem = new MenuItem
            {
                Header = "Ver Clientes de la Ruta",
                Command = this.CmdVerClientesPorRuta,
                CommandParameter = vendedorMapa
            };
            pin.ContextMenu.Items.Add(menuItem);

            var menuMostrarZona = new MenuItem
            {
                Header = "Dibujar Zona",
                Command = this.CmdDibujarZona,
                CommandParameter = vendedorMapa
            };
            pin.ContextMenu.Items.Add(menuMostrarZona);

            var menuMostrarCamino = new MenuItem
            {
                Header = "Mostrar todos los reportes",
                Command = this.CmdVerCaminoPreventista,
                CommandParameter = vendedorMapa
            };
            pin.ContextMenu.Items.Add(menuMostrarCamino);

            var menuMostrarDomicilio = new MenuItem
            {
                Header = "Mostrar domicilio del vendedor",
                Command = this.CmdVerDomicilioVendedor,
                CommandParameter = vendedorMapa
            };
            pin.ContextMenu.Items.Add(menuMostrarDomicilio);

            pin.ContextMenu.UpdateLayout();

            //pin.MouseDoubleClick += pin_MouseDoubleClick;

            marcador.Shape = pin;
            marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
            marcador.ZIndex = 4;
            this.mapa.Markers.Add(marcador);

            if (vendedorMapa.VerZona)
                this.VerZonasDelVendedor(vendedorMapa);
            if (vendedorMapa.VerClientes)
                this.VerClientesDelVendedor(vendedorMapa);
            if (vendedorMapa.VerTodasLasPosiciones)
                this.VerCaminoDelVendedor(vendedorMapa);
            if (vendedorMapa.VerDomicilioDelVendedor)
                this.VerDomicilioVendedor(vendedorMapa);

            ControladorMapa.RefrescarVista(this.mapa);
        }

        private object VerDomicilioVendedor(object vendedorMapa)
        {
            if (vendedorMapa == null)
                return false;
            try
            {
                var v = vendedorMapa as Vendedor;
                v.VerDomicilioDelVendedor = true;

                if (v.Calle != string.Empty)
                {
                    var dirCasa = v.Calle + " " + v.Numero + ", Mar del Plata";
                    var coordenadaCasa = ControladorMapa.ObtenerCordenadasPorDireccion(dirCasa);
                    if (coordenadaCasa.HasValue)
                    {
                        v.CoordenadaDomicilio = coordenadaCasa.Value;
                        ControladorVendedores.ActualizarCoordenadaDomicilio(v);
                    }
                }
                if (v.CoordenadaDomicilio.Lat != 0)
                {
                    var marcador = new GMapMarker(v.CoordenadaDomicilio);
                    var pin = PinFactory.MakePin(TipoPin.Casa);
                    pin.Tag = v;
                    marcador.Shape = pin;
                    marcador.Shape.IsHitTestVisible = true;
                    marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                    pin.ToolTip = "Casa de " + v.Nombre;

                    var menuItem = new MenuItem();
                    menuItem.Header = v.Calle + " " + v.Numero;
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

        private object VerClientesDelVendedor(object vendedorMapa)
        {
            if (vendedorMapa == null)
                return false;
            try
            {
                var v = vendedorMapa as Vendedor;
                v.VerClientes = true;

                //if (v.CantidadClientes == 0)

                ControladorVendedores.CargarClientes(v, DateTime.Today, this.ModoVerClientesConRuteo);
                if (v.Posiciones.Count == 0)
                    ControladorVendedores.CargarPosiciones(v, FechaDesde, FechaHasta);
                foreach (var zona in v.Zonas)
                {
                    foreach (var cli in zona.Clientes)
                    {
                        var marcador = new GMapMarker(cli.Coordenada.Value);

                        var pin = ControladorMapa.CrearPinCliente(v.Posiciones, cli);

                        marcador.Shape = pin;
                        marcador.Shape.IsHitTestVisible = true;
                        marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
                        marcador.ZIndex = 3;

                        this.mapa.Markers.Add(marcador);
                    }

                }
                ControladorMapa.RefrescarVista(this.mapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        private object VerCaminoDelVendedor(object vendedorMapa)
        {
            if (vendedorMapa == null)
                return false;
            try
            {
                var v = vendedorMapa as Vendedor;
                v.VerTodasLasPosiciones = true;

                this.mapa.Markers.Add(ControladorMapa.CrearRuta(v.Posiciones.Where(p => p.Latitud != 0 && p.Longitud != 0).Select(x => new PointLatLng(x.Latitud, x.Longitud)).ToList(), Brushes.Red));
                foreach (var pos in v.Posiciones)
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

        private object VerZonasDelVendedor(object vendedorMapa)
        {
            //Random r = new Random();
            var v = (Vendedor)vendedorMapa;
            try
            {
                if (v.Zonas.Count == 0) //cargo las listas de zonas si no están cargadas
                    ControladorZonas.CargarZonasDelVendedor(v, DateTime.Today);
                foreach (var zonamapa in v.Zonas) //dibujo
                {
                    //var alpha = r.Next(0, byte.MaxValue + 1);
                    //var red = r.Next(0, byte.MaxValue + 1);
                    //var green = r.Next(0, byte.MaxValue + 1);
                    //var blue = r.Next(0, byte.MaxValue + 1);
                    //var brush = new SolidColorBrush(Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue));
                    if (zonamapa.Vertices.Count == 0) //cargo los vertices si es que no los cargué aun.
                        zonamapa.Vertices = ControladorZonas.ObtenerVerticesZona(zonamapa.Codigo, zonamapa.CodigoEmpresa, zonamapa.CodigoDivision);
                    this.mapa.Markers.Add(ControladorMapa.CrearPoligonoZona(zonamapa.Vertices, zonamapa.CodigoEmpresa == "01" ? Brushes.LightBlue : Brushes.Orange, zonamapa.Codigo));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            v.VerZona = true;
            ControladorMapa.RefrescarVista(this.mapa);
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
                    //var dt = servicioMobile.ObtenerPosicionActualPreventistas();
                    var vendedores = ControladorVendedores.ObtenerUltimaPosicionVendedores();
                    //foreach (DataRow item in dt.AsEnumerable().Where(p => p.Field<string>("usuario") != ""))
                    //{
                    //    Func<object, dynamic, object> nonull = (p, def) => p == null ? def : p;
                    //    var elemento = new Vendedor()
                    //    {
                    //        Codigo = item.Field<string>("usuario"),
                    //        Estado = (Estado)nonull(item.Field<object>("estado"), Estado.OK),
                    //        CodigoEmpresa = item.Field<string>("empresa"),
                    //        Fecha = item.Field<object>("fecha").ToString(),
                    //        CoordenadaActual = new PointLatLng(
                    //                               double.Parse(item.Field<object>("latitud").ToString()),
                    //                               double.Parse(item.Field<object>("longitud").ToString())),
                    //        //Visitados = (int)nonull(item.Field<object>("visitados"), 0),
                    //        //Compradores = (int)nonull(item.Field<object>("compradores"), 0),
                    //        FondoDeCelda = item.Field<string>("empresa") == "10" ? @"S:\GEOVENTAS\alta.jpg" : @"S:\GEOVENTAS\hiller.jpg"
                    //    };

                    //    if (elemento.Codigo != null)
                    foreach (var elemento in vendedores)
                    {
                        this.AgregarActualizarVendedor(elemento);
                    }


                    switch (this.modo)
                    {
                        case ModoVerMarcadores.Nada:
                            break;
                        case ModoVerMarcadores.Todo:
                            this.MostrarTodo();
                            break;
                        case ModoVerMarcadores.Seleccionado:
                            this.MostrarMarcador(this.listaDeElementos.SelectedItem as Vendedor, true);
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

        private void AgregarActualizarVendedor(Vendedor vendedorMapa)
        {
            try
            {
                var v = Vendedores.FirstOrDefault(e => e.Codigo == vendedorMapa.Codigo);
                if (v == null)
                {
                    var prev = ControladorVendedores.ObtenerVendedor(vendedorMapa.Codigo);
                    if (prev != null)
                    {
                        vendedorMapa.Foto = prev.Foto;
                        vendedorMapa.Nombre = prev.Nombre;
                        vendedorMapa.Calle = prev.Calle;
                        vendedorMapa.Numero = prev.Numero;
                        vendedorMapa.CoordenadaDomicilio = new PointLatLng(prev.CoordenadaDomicilio.Lat, prev.CoordenadaDomicilio.Lng);
                    }

                    this.CalculaTiempoReporte(vendedorMapa);

                    if (vendedorMapa.CoordenadaActual.Lat == 0 && vendedorMapa.CoordenadaActual.Lng == 0) //si lat y lng vienen 0, 0 es porque tiene gps apagado
                    {
                        vendedorMapa.CoordenadaActual = new PointLatLng(-38.002452, -57.601936);
                        vendedorMapa.Estado = Estado.GPS_APAGADO;
                    }

                    this.Vendedores.Add(vendedorMapa);
                }
                else
                {
                    if (v.Zonas.Count == 0)
                        ControladorZonas.CargarZonasDelVendedor(v, DateTime.Today);
                    ControladorVendedores.CalcularBultosYPesos(v, FechaDesde, FechaHasta);

                    v.Estado = vendedorMapa.Estado;
                    v.Fecha = vendedorMapa.Fecha;

                    this.CalculaTiempoReporte(v);

                    if (vendedorMapa.CoordenadaActual.Lat == 0 && vendedorMapa.CoordenadaActual.Lng == 0)
                        vendedorMapa.Estado = Estado.GPS_APAGADO;
                    else
                        v.CoordenadaActual = vendedorMapa.CoordenadaActual;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void CalculaTiempoReporte(Vendedor vendedor)
        {
            var fecha = DateTime.Parse(vendedor.Fecha);
            var timeSpan = DateTime.Now - fecha;
            if (timeSpan.Minutes > 30 || timeSpan.Hours > 1)
            {
                vendedor.Estado = Estado.NO_REPORTA;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ModoVerClientesConRuteoProperty)
            {
                if (listaDeElementos.SelectedItem != null)
                {
                    this.mapa.Markers.Clear();
                    this.MostrarMarcador((Vendedor)listaDeElementos.SelectedItem, true);
                }

                //ControladorMapa.RefrescarVista(this.mapa);
            }
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

        private void btnchkVerAlta_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Vendedores)
            {
                if (item.CodigoEmpresa == "10")
                    item.Visible = true;
                else
                    item.Visible = false;
            }
        }

        private void btnchkVerHiller_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Vendedores)
            {
                if (item.CodigoEmpresa == "01")
                    item.Visible = true;
                else
                    item.Visible = false;
            }
        }

        private void btnchkVerTODO_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Vendedores)
            {
                item.Visible = true;
            }
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

        #region FILTRO DE CLIENTES
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

    public enum ModoVerMarcadores
    {
        Nada,
        Todo,
        Seleccionado
    }
}
