using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GeoventasPocho.Factory;
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

namespace GeoventasPocho.Vistas.MapaConsulta
{
    /// <summary>
    /// Interaction logic for VisualizadorClientesPorConsulta.xaml
    /// </summary>
    public partial class VisualizadorClientesPorConsulta : Window
    {


        //public ObservableCollection<Cliente> listaClientes
        //{
        //    get { return (ObservableCollection<Cliente>)GetValue(listaClientesProperty); }
        //    set { SetValue(listaClientesProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for listaClientes.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty listaClientesProperty =
        //    DependencyProperty.Register("listaClientes", typeof(ObservableCollection<Cliente>), typeof(VisualizadorClientesPorConsulta));



        public ObservableCollection<TrackingFletero> listaTracking
        {
            get { return (ObservableCollection<TrackingFletero>)GetValue(listaTrackingProperty); }
            set { SetValue(listaTrackingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for listaTracking.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty listaTrackingProperty =
            DependencyProperty.Register("listaTracking", typeof(ObservableCollection<TrackingFletero>), typeof(VisualizadorClientesPorConsulta));



        public string query
        {
            get { return (string)GetValue(queryProperty); }
            set { SetValue(queryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for query.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty queryProperty =
            DependencyProperty.Register("query", typeof(string), typeof(VisualizadorClientesPorConsulta), new PropertyMetadata("SELECT cd.a, cd.b, cd.c, cli.Latitud, cli.Longitud, cli.localidad\nFROM \\\\server\\work\\appvfp\\hergo_release\\clientes_domicilios cd\ninner join clientes cli on cd.a=cli.codigo\nWHERE (cd.b <> cli.street OR cd.c<> cli.number)\nORDER BY cd.a"));


        public VisualizadorClientesPorConsulta()
        {
            InitializeComponent();
            this.DataContext = this;

            this.mapa.Position = new PointLatLng(-38.0042615, -57.6070055);

        }

        //private void btnConsultar_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (this.query != string.Empty)
        //        {
        //            this.listaClientes = new ObservableCollection<Cliente>(ControladorClientes.ProcesarConsulta(this.query));
        //            var listaMarcadores = new List<GMapMarker>();
        //            foreach (var cli in this.listaClientes)
        //            {
        //                var marcador = new GMapMarker(cli.Coordenada.Value);

        //                var pin = ControladorMapa.CrearPinCliente(null, cli);

        //                marcador.Shape = pin;
        //                marcador.Shape.IsHitTestVisible = true;
        //                marcador.Offset = new Point(-pin.Width / 2, -pin.Height);
        //                marcador.ZIndex = 3;
        //                listaMarcadores.Add(marcador);
        //            }
        //            this.mapa.ItemsSource = listaMarcadores;
        //            ControladorMapa.RefrescarVista(this.mapa);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void btnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.query != string.Empty)
                {
                    this.listaTracking = new ObservableCollection<TrackingFletero>(ControladorClientes.ProcesarConsultaTracking(this.query));
                    var listaMarcadores = new List<GMapMarker>();
                    foreach (var item in this.listaTracking)
                    {
                        var puntoPosicion = new PointLatLng(item.Latitud, item.Longitud);
                        //var marcador = ControladorMapa.CrearPuntoPosicion(puntoPosicion);
                        var marcador = ControladorMapa.CrearPuntoPosicion(puntoPosicion);
                        var pin = PinFactory.MakePin(Factory.Pines.TipoPin.Azul, item.Usuario);
                        pin.ToolTip = item.Fecha;
                        marcador.Shape = pin;
                        marcador.Shape.IsHitTestVisible = true;
                        marcador.ZIndex = 3;
                        listaMarcadores.Add(marcador);
                    }
                    this.mapa.ItemsSource = listaMarcadores;
                    ControladorMapa.RefrescarVista(this.mapa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lstListaClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.lstListaClientes.SelectedIndex != -1)
                {
                    var item = (Cliente)this.lstListaClientes.SelectedItem;
                    this.mapa.Position = item.Coordenada.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
