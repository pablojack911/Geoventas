using GeoventasPocho.Controladores;
using GeoventasPocho.Controladores.Mapas;
using GMap.NET;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace GeoventasPocho.Vistas.Geocoder
{
    /// <summary>
    /// Interaction logic for GeocodificadorDeCallesExcel.xaml
    /// </summary>
    public partial class GeocodificadorDeCallesExcel : Window
    {


        public ObservableCollection<string> clientesActualizados
        {
            get { return (ObservableCollection<string>)GetValue(clientesActualizadosProperty); }
            set { SetValue(clientesActualizadosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for clientesActualizados.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty clientesActualizadosProperty =
            DependencyProperty.Register("clientesActualizados", typeof(ObservableCollection<string>), typeof(GeocodificadorDeCallesExcel));



        public string NombreDeHoja
        {
            get { return (string)GetValue(NombreDeHojaProperty); }
            set { SetValue(NombreDeHojaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NombreDeHoja.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NombreDeHojaProperty =
            DependencyProperty.Register("NombreDeHoja", typeof(string), typeof(GeocodificadorDeCallesExcel));



        public string UbicacionColumnaFechaDesde
        {
            get { return (string)GetValue(UbicacionColumnaFechaDesdeProperty); }
            set { SetValue(UbicacionColumnaFechaDesdeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UbicacionColumnaFechaDesde.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UbicacionColumnaFechaDesdeProperty =
            DependencyProperty.Register("UbicacionColumnaFechaDesde", typeof(string), typeof(GeocodificadorDeCallesExcel));



        public string UbicacionColumnaCodigoCliente
        {
            get { return (string)GetValue(UbicacionColumnaCodigoClienteProperty); }
            set { SetValue(UbicacionColumnaCodigoClienteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UbicacionColumnaCodigoCliente.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UbicacionColumnaCodigoClienteProperty =
            DependencyProperty.Register("UbicacionColumnaCodigoCliente", typeof(string), typeof(GeocodificadorDeCallesExcel));



        public bool BotonHabilitado
        {
            get { return (bool)GetValue(BotonHabilitadoProperty); }
            set { SetValue(BotonHabilitadoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BotonHabilitado.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BotonHabilitadoProperty =
            DependencyProperty.Register("BotonHabilitado", typeof(bool), typeof(GeocodificadorDeCallesExcel), new PropertyMetadata(false));



        public string PathOfFile
        {
            get { return (string)GetValue(PathOfFileProperty); }
            set { SetValue(PathOfFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathOfFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathOfFileProperty =
            DependencyProperty.Register("PathOfFile", typeof(string), typeof(GeocodificadorDeCallesExcel));

        private List<ClienteGeolocalizable> ListaCodigosClientes;
        public List<string> HojasDelLibro { get; set; }
        public IWorkbook HojaDeCalculo { get; set; }

        public string DefaultOpenPath { get; set; }
        public string SavingFolder { get; set; }


        public GeocodificadorDeCallesExcel()
        {
            InitializeComponent();
            this.DataContext = this;
            this.clientesActualizados = new ObservableCollection<string>();

            DefaultOpenPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void btnBuscarArchivo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            if (Directory.Exists(DefaultOpenPath))
            {
                openFileDialog1.InitialDirectory = DefaultOpenPath;
            }
            else
            {
                openFileDialog1.InitialDirectory = @"C:\";
            }
            openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog().HasValue)
            {
                this.PathOfFile = openFileDialog1.FileName;
            }
            try
            {
                var file = new FileStream(PathOfFile, FileMode.Open, FileAccess.Read);
                this.HojaDeCalculo = WorkbookFactory.Create(file);

                //this.ListaCodigosFleteroCliente = new List<Tuple<string, string>>();
                this.ListaCodigosClientes = new List<ClienteGeolocalizable>();
                this.HojasDelLibro = new List<string>();

                var cantidadDeHojas = this.HojaDeCalculo.NumberOfSheets;
                for (int i = 0; i < cantidadDeHojas; i++)
                {
                    this.HojasDelLibro.Add(this.HojaDeCalculo.GetSheetAt(i).SheetName);
                }
                this.lstHojas.ItemsSource = this.HojasDelLibro;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            var listaClientesAGeocodificar = ControladorClientes.ObtenerDatosClientes(ListaCodigosClientes.Select(c => c.Codigo).ToList());

            var zonaPermitida = new List<PointLatLng>() {
                new PointLatLng(-34.96310051817297,-62.93113791346571),
                new PointLatLng(-39.21053969290316,-62.86854934781773),
                new PointLatLng(-39.20053350467479,-60.82838446585139),
                new PointLatLng(-38.56329249427377,-57.80857023916692),
                new PointLatLng(-37.03687315700143,-56.44361058840956),
                new PointLatLng(-35.77347775463433,-56.59223411350986),
                new PointLatLng(-34.97521287216069,-57.12762013249957),
                new PointLatLng(-34.96310051817297,-62.93113791346571) };

            var msje = "Total de clientes obtenidos: " + listaClientesAGeocodificar.Count + "\n";
            MessageBox.Show(msje);
            var sb = new StringBuilder(msje);
            try
            {
                foreach (var cli in listaClientesAGeocodificar)
                {
                    var dirpluslocalidad = cli.Calle + " " + cli.Numero + ", " + cli.Localidad;
                    sb.AppendLine("CLIENTE: " + cli.Codigo);
                    sb.AppendLine("Intentando traducir: " + dirpluslocalidad);
                    //cli.Coordenada = GoogleMapsGeocoder.ObtenerCordenadasPorDireccion(dirpluslocalidad, out status);
                    cli.Coordenada = ControladorMapa.ObtenerCordenadasPorDireccion(dirpluslocalidad);
                    if (cli.Coordenada.HasValue)
                    {
                        sb.AppendLine("COORDENADA OBTENIDA - " + cli.Coordenada.ToString());
                        if (ControladorMapa.DentroDeZona(zonaPermitida, cli.Coordenada.Value.Lat, cli.Coordenada.Value.Lng))
                        {
                            var ok = ControladorClientes.ActualizarLatLngCliente(cli.Codigo, cli.Coordenada.Value, cli.Calle, int.Parse(cli.Numero));
                            sb.AppendLine("Actualizando la dirección traducida: " + ok.ToString());
                            if (ok)
                                clientesActualizados.Add(cli.Codigo + " OK!\n");
                            else
                                clientesActualizados.Add(cli.Codigo + " NOPE!! Error\n");
                        }
                        else
                        {
                            sb.AppendLine("Coordenada fuera de límite");
                            //throw new GeocoderException(new Exception("\nSe detuvo el proceso de geolocación a la altura del cliente " + cli.Codigo), cli.Codigo);
                        }
                        sb.AppendLine();
                    }
                    else
                    {
                        //throw new GeocoderException(new Exception("\nSe detuvo el proceso de geolocación a la altura del cliente " + cli.Codigo), cli.Codigo);
                    }
                }
                System.IO.File.WriteAllText(@"d:\FIN " + DateTime.Now.ToString("yyyy'-'MM'-'dd'- 'HH mm") + ".txt", sb.ToString());
            }
            //catch (GeocoderException geoEx)
            //{
            //    //var msje3 = "Se detuvo el proceso de geolocación a la altura del cliente " + geoEx.codigoCliente;
            //    sb.AppendLine(geoEx.Message);
            //    this.txtCodigo.Text = geoEx.codigoCliente;
            //    MessageBox.Show(geoEx.Message);
            //    System.IO.File.WriteAllText(@"d:\Excepcion " + DateTime.Now.ToString("yyyy'-'MM'-'dd'- 'HH mm") + ".txt", sb.ToString());
            //}
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"d:\Excepcion " + DateTime.Now.ToString("yyyy'-'MM'-'dd'- 'HH mm") + ".txt", sb.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == NombreDeHojaProperty.Name || e.Property.Name == PathOfFileProperty.Name)
            {
                if (this.NombreDeHoja != string.Empty && this.PathOfFile != string.Empty)
                    this.BotonHabilitado = true;
                else
                    this.BotonHabilitado = false;
            }
            base.OnPropertyChanged(e);
        }

        private void lstHojas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstHojas.SelectedItem?.ToString() != string.Empty)
            {
                this.NombreDeHoja = lstHojas.SelectedItem.ToString();
                this.CargarHoja();
            }
        }

        public void CargarHoja()
        {
            var listaCodigosClientes = new List<ClienteGeolocalizable>();

            ISheet sheet = this.HojaDeCalculo.GetSheet(this.NombreDeHoja);
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                var codigo = sheet.GetRow(i)?.GetCell(3)?.ToString().Trim().PadLeft(5, '0');
                if (codigo != null)
                {
                    if (!codigo.Equals("000Nº") && !codigo.Equals("00000") && !codigo.Equals("CLIENTE"))
                    {
                        var cliente = new ClienteGeolocalizable();
                        cliente.Codigo = codigo;
                        cliente.Fecha = sheet.GetRow(i)?.GetCell(0)?.DateCellValue;
                        cliente.Canal = sheet.GetRow(i)?.GetCell(1)?.ToString().Trim();
                        cliente.Zona = sheet.GetRow(i)?.GetCell(2)?.ToString().Trim();
                        cliente.RazonSocial = sheet.GetRow(i)?.GetCell(4)?.StringCellValue;
                        cliente.DomicilioFiscal = sheet.GetRow(i)?.GetCell(5)?.ToString().Trim();
                        cliente.NombreDeFantasia = sheet.GetRow(i)?.GetCell(6)?.ToString().Trim();
                        var alta = sheet.GetRow(i)?.GetCell(7)?.NumericCellValue == 1;
                        var baja = sheet.GetRow(i)?.GetCell(8)?.NumericCellValue == 1;
                        var modi = sheet.GetRow(i)?.GetCell(9)?.NumericCellValue == 1;
                        if (alta)
                            cliente.Estado = ABM.ALTA;
                        else if (baja)
                            cliente.Estado = ABM.BAJA;
                        else if (modi)
                            cliente.Estado = ABM.MODIFICACION;
                        cliente.Observaciones = sheet.GetRow(i)?.GetCell(10)?.StringCellValue;
                        listaCodigosClientes.Add(cliente);
                    }
                }
            }

            foreach (var item in listaCodigosClientes)
            {
                var lista = listaCodigosClientes.Where(c => c.Codigo == item.Codigo);
                var ultimaModificacion = lista.Max(x => x.Fecha);
                var cliente = lista.FirstOrDefault(c => c.Fecha == ultimaModificacion.Value);
                if (cliente != null && cliente.Estado != ABM.BAJA)
                    ListaCodigosClientes.Add(cliente);
            }

            this.dgClientes.ItemsSource = null;
            //this.dgFleterosClientes.ItemsSource = ListaCodigosFleteroCliente;
            this.dgClientes.ItemsSource = ListaCodigosClientes;
        }
    }

    class ClienteGeolocalizable
    {
        public DateTime? Fecha { get; set; }
        public string Canal { get; set; }
        public string Zona { get; set; }
        public string Codigo { get; set; }
        public string RazonSocial { get; set; }
        public string DomicilioFiscal { get; set; }
        public string NombreDeFantasia { get; set; }
        public ABM Estado { get; set; }
        public string Observaciones { get; set; }
    }

    enum ABM
    {
        ALTA,
        BAJA,
        MODIFICACION
    };
}
