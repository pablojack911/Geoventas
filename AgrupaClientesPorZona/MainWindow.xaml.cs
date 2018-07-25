using GeoventasPocho.DAO;
using MatrizDeAhorroWPF;
using Microsoft.Win32;
using NPOI.SS.UserModel;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace AgrupaClientesPorZona
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string PathOfFile
        {
            get { return (string)GetValue(PathOfFileProperty); }
            set { SetValue(PathOfFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathOfFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathOfFileProperty =
            DependencyProperty.Register("PathOfFile", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

        public string NombreHoja
        {
            get { return (string)GetValue(NombreHojaProperty); }
            set { SetValue(NombreHojaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NombreHoja.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NombreHojaProperty =
            DependencyProperty.Register("NombreHoja", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

        public bool BotonHabilitado
        {
            get { return (bool)GetValue(BotonHabilitadoProperty); }
            set { SetValue(BotonHabilitadoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BotonHabilitado.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BotonHabilitadoProperty =
            DependencyProperty.Register("BotonHabilitado", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        //public List<Tuple<string, string>> ListaCodigosFleteroCliente { get; set; }
        public List<string> ListaCodigoZonas { get; set; }

        public List<string> HojasDelLibro { get; set; }
        public IWorkbook HojaDeCalculo { get; set; }

        public string DefaultOpenPath { get; set; }
        public string SavingFolder { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            DefaultOpenPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == NombreHojaProperty.Name || e.Property.Name == PathOfFileProperty.Name)
            {
                if (this.NombreHoja != string.Empty && this.PathOfFile != string.Empty)
                    this.BotonHabilitado = true;
                else
                    this.BotonHabilitado = false;
            }
            base.OnPropertyChanged(e);

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
                this.ListaCodigoZonas = new List<string>();
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

        //private void btnAbrirArchivo_Click(object sender, RoutedEventArgs e)
        //{
        //    //abro el archivo de Excel
        //    try
        //    {
        //        var file = new FileStream(PathOfFile, FileMode.Open, FileAccess.Read);
        //        this.HojaDeCalculo = WorkbookFactory.Create(file);

        //        this.ListaCodigosFleteroCliente = new List<Tuple<string, string>>();
        //        this.HojasDelLibro = new List<string>();

        //        var cantidadDeHojas = this.HojaDeCalculo.NumberOfSheets;
        //        for (int i = 0; i < cantidadDeHojas; i++)
        //        {
        //            this.HojasDelLibro.Add(this.HojaDeCalculo.GetSheetAt(i).SheetName);
        //        }
        //        this.lstHojas.ItemsSource = this.HojasDelLibro;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
        //    }
        //}

        public void CargarHoja()
        {
            ListaCodigoZonas = new List<string>();
            //ListaCodigosFleteroCliente = new List<Tuple<string, string>>();
            var zona = string.Empty;
            //var cliente = string.Empty;
            ISheet sheet = this.HojaDeCalculo.GetSheet(this.NombreHoja);
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                zona = sheet.GetRow(i).GetCell(0)?.ToString().Trim();
                //cliente = sheet.GetRow(i).GetCell(1)?.ToString().Trim().PadLeft(5, '0');
                //ListaCodigosFleteroCliente.Add(new Tuple<string, string>(flet, cliente));
                ListaCodigoZonas.Add(zona);
            }

            this.dgFleterosClientes.ItemsSource = null;
            //this.dgFleterosClientes.ItemsSource = ListaCodigosFleteroCliente;
            this.dgFleterosClientes.ItemsSource = ListaCodigoZonas;
        }
        //private void btnAbrirArchivo_Click(object sender, RoutedEventArgs e)
        //{
        //    //abro el archivo de Excel
        //    try
        //    {
        //        var file = new FileStream(PathOfFile, FileMode.Open, FileAccess.Read);
        //        IWorkbook wb = WorkbookFactory.Create(file);
        //        //Selecciono la hoja que voy a leer
        //        ISheet sheet = wb.GetSheet(this.NombreHoja);

        //        this.ListaCodigosFleteroCliente = new List<Tuple<string, string>>();
        //        var flet = string.Empty;
        //        var cliente = string.Empty;
        //        for (int i = 0; i <= sheet.LastRowNum; i++)
        //        {
        //            flet = sheet.GetRow(i).GetCell(0).ToString().Trim();
        //            cliente = sheet.GetRow(i).GetCell(1).ToString().Trim().PadLeft(5, '0');
        //            ListaCodigosFleteroCliente.Add(new Tuple<string, string>(flet, cliente));
        //        }

        //        this.dgFleterosClientes.ItemsSource = ListaCodigosFleteroCliente;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
        //    }
        //}

        private void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            this.SavingFolder = this.ElegirDondeGuardar();
            List<Cliente> clientes = new List<Cliente>();
            foreach (var z in ListaCodigoZonas)
            {
                clientes = ObtenerClientes(z);
                //GrabarEnTxt(this.NombreHoja + "_" + fletero.Key, clientes);
                GrabarEnKML(this.NombreHoja + "_" + z, clientes);
                clientes.Clear();
            }
            MessageBox.Show("PROCESO CONCLUIDO");
        }

        private List<Cliente> ObtenerClientes(string z)
        {
            var clientes = new List<Cliente>();
            try
            {
                var consulta = FoxDB.Instancia.CrearComando(string.Format("select codigo,latitud,longitud from clientes c inner join config_zona cz on cz.cliente=c.codigo where cz.zona='{0}' and cz.baja=0 and c.inactivo=0 and c.potencial=0 and c.legales=0 and c.suspendido=0 group by c.codigo", z));
                FoxDB.Instancia.Conectar();
                var dr = consulta.ExecuteReader();
                while (dr.Read())
                {
                    clientes.Add(new Cliente()
                    {
                        Codigo = dr.GetString(0).Trim(),
                        Lat = Convert.ToDouble(dr.GetValue(1), CultureInfo.CurrentCulture),
                        Lng = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture)
                    });
                }
                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return clientes;
        }

        public void GrabarEnTxt(string Titulo, List<Cliente> listaClientes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("codigo,latitud,longitud");
            foreach (var item in listaClientes)
            {
                sb.AppendLine(item.Codigo + "," + item.Lat + "," + item.Lng);
            }
            System.IO.File.WriteAllText(this.PathOfFile + @"\" + Titulo + ".txt", sb.ToString());
        }

        public string ElegirDondeGuardar()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                return dialog.SelectedPath;
            return this.DefaultOpenPath;
        }

        public void GrabarEnKML(string Titulo, List<Cliente> listaClientes)
        {
            var dir = System.IO.Directory.CreateDirectory(this.SavingFolder + "\\" + this.NombreHoja);
            using (XmlWriter writer = XmlWriter.Create(dir.FullName + @"\" + Titulo + ".kml", new XmlWriterSettings() { Indent = true }))
            {
                //writer.WriteStartAttribute("kml", "xmlns", "http://www.opengis.net/kml/2.2");
                writer.WriteStartDocument();
                writer.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
                writer.WriteStartElement("Document");

                foreach (var cli in listaClientes)
                {
                    writer.WriteStartElement("Placemark");
                    writer.WriteElementString("name", cli.Codigo);
                    writer.WriteStartElement("Point");
                    writer.WriteElementString("coordinates", cli.Lng + "," + cli.Lat + ",0");
                    //end point
                    writer.WriteEndElement();
                    //end placemark
                    writer.WriteEndElement();
                }
                //end document
                writer.WriteEndElement();
                //end kml
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public Cliente ObtenerCliente(string codigo)
        {
            Cliente cliente = new Cliente();
            try
            {
                var consulta = FoxDB.Instancia.CrearComando(string.Format("select codigo,latitud,longitud from clientes where codigo='{0}'", codigo));
                FoxDB.Instancia.Conectar();
                var dr = consulta.ExecuteReader();
                while (dr.Read())
                {
                    cliente.Codigo = dr.GetString(0).Trim();
                    cliente.Lat = Convert.ToDouble(dr.GetValue(1), CultureInfo.CurrentCulture);
                    cliente.Lng = Convert.ToDouble(dr.GetValue(2), CultureInfo.CurrentCulture);
                }
                FoxDB.Instancia.Desconectar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return cliente;
        }

        private void lstHojas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstHojas.SelectedItem?.ToString() != string.Empty)
            {
                this.NombreHoja = lstHojas.SelectedItem.ToString();
                this.CargarHoja();
            }

        }
    }
}
