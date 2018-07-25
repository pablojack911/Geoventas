using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.Vistas.ElementosMapa;
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

namespace GeoventasPocho.Vistas.ActividadesClientes
{
    /// <summary>
    /// Interaction logic for VisualizadorActividadesClientes.xaml
    /// </summary>
    public partial class VisualizadorActividadesClientes : Window
    {
        public ObservableCollection<Actividad> Actividades { get; set; }

        public List<Cliente> clientesActividadSeleccionMultiple;

        public SelectionMode ModoSeleccion
        {
            get { return (SelectionMode)GetValue(ModoSeleccionProperty); }
            set { SetValue(ModoSeleccionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModoSeleccion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoSeleccionProperty =
            DependencyProperty.Register("ModoSeleccion", typeof(SelectionMode), typeof(VisualizadorActividadesClientes), new PropertyMetadata(SelectionMode.Single));

        public GridLength VisibilidadColumnaClientes
        {
            get { return (GridLength)GetValue(VisibilidadColumnaClientesProperty); }
            set { SetValue(VisibilidadColumnaClientesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibilidadColumnaClientes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilidadColumnaClientesProperty =
            DependencyProperty.Register("VisibilidadColumnaClientes", typeof(GridLength), typeof(VisualizadorActividadesClientes), new PropertyMetadata(new GridLength(0)));

        public ModoClientesRuteo ModoVerClientesConRuteo
        {
            get { return (ModoClientesRuteo)GetValue(ModoVerClientesConRuteoProperty); }
            set { SetValue(ModoVerClientesConRuteoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModoVerClientesConRuteo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoVerClientesConRuteoProperty =
            DependencyProperty.Register("ModoVerClientesConRuteo", typeof(ModoClientesRuteo), typeof(VisualizadorActividadesClientes), new PropertyMetadata(ModoClientesRuteo.Todos));

        public Actividad ActividadSeleccionada
        {
            get { return (Actividad)GetValue(ActividadSeleccionadaProperty); }
            set { SetValue(ActividadSeleccionadaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActividadSeleccionada.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActividadSeleccionadaProperty =
            DependencyProperty.Register("ActividadSeleccionada", typeof(Actividad), typeof(VisualizadorActividadesClientes));

        public VisualizadorActividadesClientes()
        {
            InitializeComponent();
            this.DataContext = this;
            List<Actividad> actividades = new List<Actividad>();
            this.clientesActividadSeleccionMultiple = new List<Cliente>();
            try
            {
                actividades = ControladorActividades.ObtenerActividades();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nEsta ventana se cerrará.");
                this.Close();
            }

            this.Actividades = new ObservableCollection<Actividad>(actividades);

            var mdp = ControladorMapa.ObtenerCordenadasPorDireccion("Mar del Plata, Buenos Aires");
            if (mdp.HasValue)
                this.mapa.Position = mdp.Value;
            else
                this.mapa.Position = new PointLatLng(-38.0042615, -57.6070055);

            this.listaDeActividades.SelectionChanged += ListaDeActividades_SelectionChanged;
        }

        private void ListaDeActividades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.mapa.Markers.Clear();
                this.clientesActividadSeleccionMultiple.Clear();
                if (this.listaDeActividades.SelectedItem != null)
                {
                    foreach (var act in listaDeActividades.SelectedItems)
                    {
                        var actividad = act as Actividad;
                        this.SeleccionarActividad(actividad, false);
                        this.clientesActividadSeleccionMultiple.AddRange(actividad.Clientes);
                    }
                    if (ModoSeleccion == SelectionMode.Multiple)
                    {
                        this.clientesUC.Clientes = new ObservableCollection<Cliente>(clientesActividadSeleccionMultiple);
                        this.clientesUC.cantidadClientes = this.clientesUC.Clientes.Count;
                        this.clientesUC.dgClientes.Items.Filter = null;
                    }
                }
                ControladorMapa.RefrescarVista(this.mapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SeleccionarActividad(Actividad actividad, bool clear = true)
        {
            if (clear)
                this.mapa.Markers.Clear();

            actividad.Clientes.Clear();
            ControladorActividades.CargarClientes(actividad, this.ModoVerClientesConRuteo);

            this.VerClientes(actividad.Clientes);

            if (this.ModoSeleccion == SelectionMode.Single)
            {
                this.VisibilidadColumnaClientes = GridLength.Auto;
                this.clientesUC.Clientes = new ObservableCollection<Cliente>(actividad.Clientes);
                this.clientesUC.cantidadClientes = this.clientesUC.Clientes.Count;
                this.clientesUC.dgClientes.Items.Filter = null;
                this.ActividadSeleccionada = actividad;
            }
            else
            {
                //this.VisibilidadColumnaClientes = new GridLength(0);
            }
        }

        private void VerClientes(List<Cliente> clientes)
        {
            foreach (var cli in clientes)
            {
                var marcador = new GMapMarker(cli.Coordenada.Value);

                var pin = ControladorMapa.CrearPinCliente(null, cli, false);

                marcador.Tag = cli;
                marcador.Shape = pin;
                marcador.Shape.IsHitTestVisible = true;
                marcador.Offset = new Point(-pin.Width / 2, -pin.Height);

                //if (this.ModoSeleccion == SelectionMode.Single)
                //{
                //    var menuMover = new MenuItem();
                //    menuMover.Header = "Ubicar manualmente en el mapa";
                //    menuMover.Command = this.CmdMoverUbicacionDeClienteManualmente;
                //    menuMover.CommandParameter = marcador;
                //    pin.Menu.Items.Add(menuMover);
                //}

                pin.Menu.UpdateLayout();

                marcador.ZIndex = 3;

                this.mapa.Markers.Add(marcador);

            }
            ControladorMapa.RefrescarVista(this.mapa);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ModoVerClientesConRuteoProperty)
            {
                if (this.ModoSeleccion == SelectionMode.Single)
                {
                    if (listaDeActividades.SelectedItem != null)
                    {
                        this.ActividadSeleccionada = (Actividad)listaDeActividades.SelectedItem;
                        this.SeleccionarActividad(this.ActividadSeleccionada);
                    }
                }
                else
                {
                    this.mapa.Markers.Clear();
                    this.clientesActividadSeleccionMultiple.Clear();
                    foreach (var act in listaDeActividades.SelectedItems)
                    {
                        var actividad = act as Actividad;
                        this.SeleccionarActividad(actividad, false); //ya hice el clear antes.
                        this.clientesActividadSeleccionMultiple.AddRange(actividad.Clientes);
                    }
                    this.clientesUC.Clientes = new ObservableCollection<Cliente>(clientesActividadSeleccionMultiple);
                    this.clientesUC.cantidadClientes = this.clientesUC.Clientes.Count;
                    this.clientesUC.dgClientes.Items.Filter = null;
                }
                ControladorMapa.RefrescarVista(this.mapa);
            }
            base.OnPropertyChanged(e);
        }


        #region Modo seleccion
        private void btnIndividual_Click(object sender, RoutedEventArgs e)
        {
            this.ModoSeleccion = SelectionMode.Single;
        }

        private void btnMultiple_Click(object sender, RoutedEventArgs e)
        {
            this.ModoSeleccion = SelectionMode.Multiple;
            //this.VisibilidadColumnaClientes = new GridLength(0);
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
