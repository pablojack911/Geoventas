using GeoventasPocho.Vistas.ElementosMapa;
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

namespace GeoventasPocho.Vistas.HistoricoLogistica
{
    /// <summary>
    /// Interaction logic for PosicionesClientes.xaml
    /// </summary>
    public partial class PosicionesClientesLogistica : UserControl
    {

        public string CodigoVendedor
        {
            get { return (string)GetValue(CodigoVendedorProperty); }
            set { SetValue(CodigoVendedorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CodigoVendedor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodigoVendedorProperty =
            DependencyProperty.Register("CodigoVendedor", typeof(string), typeof(PosicionesClientesLogistica));


        public string NombreVendedor
        {
            get { return (string)GetValue(NombreVendedorProperty); }
            set { SetValue(NombreVendedorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NombreVendedor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NombreVendedorProperty =
            DependencyProperty.Register("NombreVendedor", typeof(string), typeof(PosicionesClientesLogistica));


        public string FechaDelReporte
        {
            get { return (string)GetValue(FechaDelReporteProperty); }
            set { SetValue(FechaDelReporteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FechaDelReporte.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FechaDelReporteProperty =
            DependencyProperty.Register("FechaDelReporte", typeof(string), typeof(PosicionesClientesLogistica));

        public ObservableCollection<ItemReporte> Posiciones
        {
            get { return (ObservableCollection<ItemReporte>)GetValue(PosicionesProperty); }
            set { SetValue(PosicionesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Posiciones.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PosicionesProperty =
            DependencyProperty.Register("Posiciones", typeof(ObservableCollection<ItemReporte>), typeof(PosicionesClientesLogistica));




        public PosicionesClientesLogistica()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintManager.ReporteDePosiciones(this.CodigoVendedor, this.NombreVendedor, this.FechaDelReporte, this.Posiciones.ToList());
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            PrintManager.ReporteDePosicionesExcel(this.CodigoVendedor, this.NombreVendedor, this.FechaDelReporte, this.Posiciones.ToList());
        }
    }
}
