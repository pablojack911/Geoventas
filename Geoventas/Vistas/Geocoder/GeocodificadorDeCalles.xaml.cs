using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeoventasPocho.Vistas.Geocoder
{
    /// <summary>
    /// Interaction logic for GeocodificadorDeCalles.xaml
    /// </summary>
    public partial class GeocodificadorDeCalles : Window
    {
        //public ObservableCollection<string> clientesactualizados { get; set; }

        public ObservableCollection<string> clientesactualizados
        {
            get { return (ObservableCollection<string>)GetValue(clientesactualizadosProperty); }
            set { SetValue(clientesactualizadosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for clientesactualizados.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty clientesactualizadosProperty =
            DependencyProperty.Register("clientesactualizados", typeof(ObservableCollection<string>), typeof(GeocodificadorDeCalles));

        public GeocodificadorDeCalles()
        {
            InitializeComponent();
            this.DataContext = this;
            this.clientesactualizados = new ObservableCollection<string>();
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            var codigoInicial = this.txtCodigo.Text;
            var listaClientesAGeocodificar = ControladorClientes.ObtenerClientes(codigoInicial);
            var msje = "Total de clientes obtenidos: " + listaClientesAGeocodificar.Count + "\n";
            MessageBox.Show(msje);
            var sb = new StringBuilder(msje);
            try
            {
                foreach (var cli in listaClientesAGeocodificar)
                {
                    GeoCoderStatusCode status;

                    var dirpluslocalidad = cli.Calle + " " + cli.Numero + ", " + cli.Localidad;
                    sb.AppendLine("CLIENTE: " + cli.Codigo + "\n");
                    sb.AppendLine("Intentando traducir: " + dirpluslocalidad + "\n");
                    //cli.Coordenada = GoogleMapsGeocoder.ObtenerCordenadasPorDireccion(dirpluslocalidad, out status);
                    cli.Coordenada = ControladorMapa.ObtenerCordenadasPorDireccion(dirpluslocalidad);
                    if (cli.Coordenada.HasValue)
                    {
                        sb.AppendLine("COORDENADA OBTENIDA - " + cli.Coordenada.ToString() + "\n");
                        if (cli.Coordenada.Value.Lat < 0 && cli.Coordenada.Value.Lng < 0)
                        {
                            var ok = ControladorClientes.ActualizarLatLngCliente(cli.Codigo, cli.Coordenada.Value, cli.Calle, int.Parse(cli.Numero));
                            sb.AppendLine("Actualizando la dirección traducida: " + ok.ToString() + "\n");
                            if (ok)
                                clientesactualizados.Add(cli.Codigo + " OK!\n\n");
                            else
                                clientesactualizados.Add(cli.Codigo + " NOPE!! Error\n\n");
                        }
                        else
                        {
                            //throw new GeocoderException(new Exception("\nSe detuvo el proceso de geolocación a la altura del cliente " + cli.Codigo), cli.Codigo);
                        }
                    }
                    else
                    {
                        //throw new GeocoderException(new Exception("\nSe detuvo el proceso de geolocación a la altura del cliente " + cli.Codigo), cli.Codigo);
                    }
                }
                System.IO.File.WriteAllText(@"d:\FIN " + DateTime.Now.ToString("yyyy'-'MM'-'dd'- 'HH mm") + ".txt", sb.ToString());
            }
            catch (GeocoderException geoEx)
            {
                //var msje3 = "Se detuvo el proceso de geolocación a la altura del cliente " + geoEx.codigoCliente;
                sb.AppendLine(geoEx.Message);
                this.txtCodigo.Text = geoEx.codigoCliente;
                MessageBox.Show(geoEx.Message);
                System.IO.File.WriteAllText(@"d:\Excepcion " + DateTime.Now.ToString("yyyy'-'MM'-'dd'- 'HH mm") + ".txt", sb.ToString());
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"d:\Excepcion " + DateTime.Now.ToString("yyyy'-'MM'-'dd'- 'HH mm") + ".txt", sb.ToString());
                MessageBox.Show(ex.Message);
            }
        }
    }
}
