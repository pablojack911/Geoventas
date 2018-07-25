using Inteldev.Fixius.Mapas;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZonasPorKml
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

        public List<Zona> Zonas { get; set; }
        public List<String> ZonasBorradas { get; set; }


        public List<string> Empresas
        {
            get { return (List<string>)GetValue(EmpresasProperty); }
            set { SetValue(EmpresasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Empresas.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmpresasProperty =
            DependencyProperty.Register("Empresas", typeof(List<string>), typeof(MainWindow), new PropertyMetadata(new List<string>()));


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.ZonasBorradas = new List<string>();
            //this.Zonas = new ObservableCollection<Zona>();
        }

        private void btnAbrirArchivo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "D:\\Desktop";
            openFileDialog1.Filter = "kml files (*.kml)|*.kml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog().HasValue)
            {
                this.PathOfFile = openFileDialog1.FileName;
            }
        }

        private void btnAbrir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Zonas = ImportKml.ImportarZonasKml(this.PathOfFile);
                this.dgZonasVertices.ItemsSource = this.Zonas;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var zona in this.Zonas)
            {
                this.BorrarZona(zona.Codigo);
                this.AgregarZona(zona);
                sb.AppendLine(zona.Codigo);
            }
            System.IO.File.WriteAllText(@"d:\IMPORTACION ZONAS.txt", sb.ToString());
            MessageBox.Show("Importación completa.");

            //var sb = new StringBuilder();
            //foreach (var zona in zonas)
            //{
            //    foreach (var coord in zona.Value)
            //    {
            //        sb.AppendLine(string.Format("insert into zonas_vertices(zona,empresa,subempresa,latitud,longitud) values('H{0}','{1}','{2}',{3},{4})", zona.Key, "01", "00BEB", coord.Lat, coord.Lng));
            //        sb.AppendLine(string.Format("insert into zonas_vertices(zona,empresa,subempresa,latitud,longitud) values('A{0}','{1}','{2}',{3},{4})", zona.Key, "10", "00ALI", coord.Lat, coord.Lng));
            //    }
            //}
            //System.IO.File.WriteAllText(@"d:\IMPORTACION ZONAS.txt", sb.ToString());
            //MessageBox.Show("Importación completa.");
        }

        private void AgregarZona(Zona zona)
        {
            ControladorZonasVertices.InsertarZona(zona, this.Empresas);
        }

        private void BorrarZona(string codigo)
        {
            if (!this.ZonasBorradas.Any(x => x.Equals(codigo)))
            {
                ControladorZonasVertices.BorrarZona(codigo, this.Empresas);
                this.ZonasBorradas.Add(codigo);
            }
        }

        private void chkAlta_Checked(object sender, RoutedEventArgs e)
        {
            var chk = (CheckBox)sender;
            if (chk.IsChecked.Value)
                this.Empresas.Add("10");
            else
                this.Empresas.Remove("10");
        }

        private void chkHiller_Checked(object sender, RoutedEventArgs e)
        {
            var chk = (CheckBox)sender;
            if (chk.IsChecked.Value)
                this.Empresas.Add("01");
            else
                this.Empresas.Remove("01");
        }
    }
}
