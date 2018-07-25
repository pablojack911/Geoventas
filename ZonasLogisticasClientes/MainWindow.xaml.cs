using Microsoft.Win32;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZonasLogisticasClientes
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

        public List<Tuple<string, string>> Hoja_1 { get; set; }
        public List<Tuple<string, string>> Hoja_2 { get; set; }
        public List<Tuple<string, string>> Hoja_3 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Hoja_1 = new List<Tuple<string, string>>();
            this.Hoja_2 = new List<Tuple<string, string>>();
            this.Hoja_3 = new List<Tuple<string, string>>();
        }


        private void btnAbrirArchivo_Click(object sender, RoutedEventArgs e)
        {
            //abro el archivo de Excel
            IWorkbook wb = WorkbookFactory.Create(new FileStream(PathOfFile, FileMode.Open, FileAccess.Read));

            //HOJA 1
            {
                var zonaLogis = string.Empty;
                var nombre = string.Empty;

                var sheet = wb.GetSheetAt(0);//primera hoja
                var tab = new TabItem();
                tab.Header = sheet.SheetName;
                var dgHoja = new DataGrid();

                for (int i = 0; i < sheet.LastRowNum; i++)
                {
                    zonaLogis = sheet.GetRow(i).GetCell(0).ToString().Trim();
                    nombre = sheet.GetRow(i).GetCell(1).ToString().Trim();
                    Hoja_1.Add(new Tuple<string, string>(zonaLogis, nombre));
                }
                dgHoja.ItemsSource = Hoja_1;
                tab.Content = dgHoja;
                tabControl.Items.Add(tab);
            }

            //HOJA 2
            {
                var zonaComer = string.Empty;
                var zonaLogis = string.Empty;

                var sheet = wb.GetSheetAt(1);//primera hoja
                var tab = new TabItem();
                tab.Header = sheet.SheetName;
                var dgHoja = new DataGrid();

                for (int i = 0; i < sheet.LastRowNum; i++)
                {
                    zonaComer = sheet.GetRow(i).GetCell(0).ToString().Trim();
                    zonaLogis = sheet.GetRow(i).GetCell(1).ToString().Trim();

                    Hoja_2.Add(new Tuple<string, string>(zonaComer, zonaLogis));
                }
                dgHoja.ItemsSource = Hoja_2;
                tab.Content = dgHoja;
                tabControl.Items.Add(tab);
            }

            //HOJA 3
            {
                var cliente = string.Empty;
                var zonaLogis = string.Empty;

                var sheet = wb.GetSheetAt(2);//primera hoja
                var tab = new TabItem();
                tab.Header = sheet.SheetName;
                var dgHoja = new DataGrid();

                for (int i = 0; i < sheet.LastRowNum; i++)
                {
                    cliente = sheet.GetRow(i).GetCell(0).ToString().Trim();
                    zonaLogis = sheet.GetRow(i).GetCell(1).ToString().Trim();

                    Hoja_3.Add(new Tuple<string, string>(cliente, zonaLogis));
                }
                dgHoja.ItemsSource = Hoja_3;
                tab.Content = dgHoja;
                tabControl.Items.Add(tab);
            }
        }

        private void btnBuscarArchivo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog().HasValue)
            {
                this.PathOfFile = openFileDialog1.FileName;
            }
        }

        private void btnProcesar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////PROCESO HOJA 1
                //{
                //    foreach (var zona in Hoja_1)
                //    {
                //        ControladorZonasLogis.AgregarZonaLogistica(zona.Item1, zona.Item2);
                //    }
                //}
                //MessageBox.Show("FIN PROCESO HOJA 1");
                //PROCESO HOJA 2
                {
                    foreach (var zonaComerLogis in Hoja_2)
                    {
                        var clientesZonaComer = ControladorClientesZona.ObtenerClientesPorZonaComercial(zonaComerLogis.Item1);
                        foreach (var cliente in clientesZonaComer)
                        {
                            this.AgregaActualizaConfigLogis(cliente, zonaComerLogis.Item2);
                        }
                    }
                }
                MessageBox.Show("FIN PROCESO HOJA 2");
                //PROCESO HOJA 3
                {
                    foreach (var cliLogis in Hoja_3)
                    {
                        this.AgregaActualizaConfigLogis(cliLogis.Item1, cliLogis.Item2);
                    }
                }
                MessageBox.Show("FIN PROCESO HOJA 3");
                MessageBox.Show("¡PROCESO FINALIZADO!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void AgregaActualizaConfigLogis(string cliente, string zona)
        {
            var estaEnConfigLogis = ControladorConfigLogis.ConsultaCliente(cliente);
            if (estaEnConfigLogis > 0)
                ControladorConfigLogis.ActualizarConfigLogis(zona, cliente);
            else
                ControladorConfigLogis.InsertarConfigLogis(zona, cliente);
        }
    }
}
