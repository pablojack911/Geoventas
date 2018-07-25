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

namespace ZonasComercialesClientes
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


        public List<Tuple<string, string, string, string, string, string, string>> Hoja_1 { get; set; }
        public List<Tuple<string, string, string, string, string>> Hoja_2 { get; set; }
        public List<Tuple<string, string, string, string>> Hoja_3 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Hoja_1 = new List<Tuple<string, string, string, string, string, string, string>>();
            this.Hoja_2 = new List<Tuple<string, string, string, string, string>>();
            this.Hoja_3 = new List<Tuple<string, string, string, string>>();
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

        private void btnAbrirArchivo_Click(object sender, RoutedEventArgs e)
        {
            //abro el archivo de Excel
            IWorkbook wb = WorkbookFactory.Create(new FileStream(PathOfFile, FileMode.Open, FileAccess.Read));

            //HOJA 1
            {
                var zona = string.Empty;
                var nombre = string.Empty;
                var empresa = string.Empty;
                var empresa_rel = string.Empty;
                var operador = string.Empty;
                var region = string.Empty;
                var ciudad = string.Empty;

                var sheet = wb.GetSheetAt(0);//primera hoja
                var tab = new TabItem();
                tab.Header = sheet.SheetName;
                var dgHoja = new DataGrid();

                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    zona = sheet.GetRow(i).GetCell(0).ToString().Trim();
                    nombre = sheet.GetRow(i).GetCell(1).ToString().Trim();
                    empresa = sheet.GetRow(i).GetCell(2).ToString().Trim();
                    empresa_rel = sheet.GetRow(i).GetCell(3).ToString().Trim();
                    operador = sheet.GetRow(i).GetCell(4).ToString().Trim();
                    region = sheet.GetRow(i).GetCell(5).ToString().Trim().PadLeft(2, '0');
                    ciudad = sheet.GetRow(i).GetCell(6).ToString().Trim();

                    Hoja_1.Add(new Tuple<string, string, string, string, string, string, string>(zona, nombre, empresa, empresa_rel, operador, region, ciudad));
                }
                dgHoja.ItemsSource = Hoja_1;
                tab.Content = dgHoja;
                tabControl.Items.Add(tab);
            }

            //HOJA 2
            {
                var zonaComer = string.Empty;
                var empresa = string.Empty;
                var empresa_rel = string.Empty;
                var pedido = string.Empty;
                var entrega = string.Empty;

                var sheet = wb.GetSheetAt(1);//primera hoja
                var tab = new TabItem();
                tab.Header = sheet.SheetName;
                var dgHoja = new DataGrid();

                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    zonaComer = sheet.GetRow(i).GetCell(0).ToString().Trim();
                    empresa = sheet.GetRow(i).GetCell(1).ToString().Trim().PadLeft(2, '0');
                    empresa_rel = sheet.GetRow(i).GetCell(2).ToString().Trim();
                    pedido = sheet.GetRow(i).GetCell(3).ToString().Trim();
                    entrega = sheet.GetRow(i).GetCell(4).ToString().Trim();

                    Hoja_2.Add(new Tuple<string, string, string, string, string>(zonaComer, empresa, empresa_rel, pedido, entrega));
                }
                dgHoja.ItemsSource = Hoja_2;
                tab.Content = dgHoja;
                tabControl.Items.Add(tab);
            }

            //HOJA 3
            {
                var cliente = string.Empty;
                var zonaComer = string.Empty;
                var empresa = string.Empty;
                var empresa_rel = string.Empty;

                var sheet = wb.GetSheetAt(2);//2da hoja
                var tab = new TabItem();
                tab.Header = sheet.SheetName;
                var dgHoja = new DataGrid();

                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    cliente = sheet.GetRow(i).GetCell(0).ToString().Trim().PadLeft(5, '0');
                    zonaComer = sheet.GetRow(i).GetCell(1).ToString().Trim();
                    empresa = sheet.GetRow(i).GetCell(2).ToString().Trim();
                    empresa_rel = sheet.GetRow(i).GetCell(3).ToString().Trim();

                    Hoja_3.Add(new Tuple<string, string, string, string>(cliente, zonaComer, empresa, empresa_rel));
                }
                dgHoja.ItemsSource = Hoja_3;
                tab.Content = dgHoja;
                tabControl.Items.Add(tab);
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
                //        ControladorZonasComerciales.AgregarZonaComercial(zona.Item1, zona.Item2, zona.Item3, zona.Item4, zona.Item5, zona.Item6, zona.Item7);
                //    }
                //}
                //MessageBox.Show("FIN PROCESO HOJA 1");
                ////PROCESO HOJA 2
                //{
                //    foreach (var zonaComer in Hoja_2)
                //    {
                //        this.AgregaActualizaCronPed(zonaComer.Item1, zonaComer.Item2, zonaComer.Item3, zonaComer.Item4, zonaComer.Item5);
                //    }
                //}
                //MessageBox.Show("FIN PROCESO HOJA 2");
                //PROCESO HOJA 3
                {
                    foreach (var cliZona in Hoja_3)
                    {
                        this.AgregaActualizaConfigZona(cliZona.Item1, cliZona.Item2, cliZona.Item3, cliZona.Item4);
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

        private void AgregaActualizaCronPed(string zona, string empresa, string prov, string pedido, string entrega)
        {
            var estaEnCronPed = ControladorCronPed.ConsultaCronPed(zona, empresa, prov);
            if (estaEnCronPed > 0)
                ControladorCronPed.ActualizaCronPed(zona, empresa, prov, pedido, entrega);
            else
                ControladorCronPed.InsertarCronPed(zona, empresa, prov, pedido, entrega);
        }

        private void AgregaActualizaConfigZona(string cliente, string zona, string empresa, string subempresa)
        {
            var estaEnConfigZona = ControladorConfigZona.ConsultaCliente(cliente, empresa, subempresa);
            if (estaEnConfigZona > 0)
                ControladorConfigZona.ActualizarConfigZona(cliente, zona, empresa, subempresa);
            else
                ControladorConfigZona.InsertarConfigZona(cliente, zona, empresa, subempresa);
        }
    }
}
