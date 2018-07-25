using GeoventasPocho.ServiceMobile;
using System;
using System.Collections.Generic;
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

namespace GeoventasPocho.Vistas.ReportesPorDiaPorVendedor
{
    /// <summary>
    /// Interaction logic for TotalReportesDelDiaPorCodigoVendedor.xaml
    /// </summary>
    public partial class ReportesPorDiaPorVendedor : Window
    {
        ServiceSoapClient servicioMobile;

        public Visibility Visible
        {
            get { return (Visibility)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register("Visible", typeof(Visibility), typeof(ReportesPorDiaPorVendedor), new PropertyMetadata(Visibility.Collapsed));

        public string codigoVendedor
        {
            get { return (string)GetValue(codigoVendedorProperty); }
            set { SetValue(codigoVendedorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for codigoVendedor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty codigoVendedorProperty =
            DependencyProperty.Register("codigoVendedor", typeof(string), typeof(ReportesPorDiaPorVendedor), new PropertyMetadata(string.Empty));

        public ReportesPorDiaPorVendedor()
        {
            InitializeComponent();
            this.servicioMobile = new ServiceMobile.ServiceSoapClient();
            this.DataContext = this;
            this.dtpFechaDesde.SelectedDate = DateTime.Today;
            this.dtpFechaHasta.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void btnObtenerPosicionesPorCodigo_Click(object sender, RoutedEventArgs e)
        {
            var fechaDesde = this.dtpFechaDesde.SelectedDate.Value;
            var fechaHasta = this.dtpFechaHasta.SelectedDate.Value;
            if (this.cbiTodos.IsSelected)
                //res = this.servicioMobile.Obtener(fechaDesde, fechaHasta);
                MessageBox.Show("No hay nada para mostrar.\n\nSeleccione 'Filtrar por Código'");
            else
            {
                var res = this.servicioMobile.ObtenerPosicionesDelPreventista(this.codigoVendedor, fechaDesde, fechaHasta);
                MessageBox.Show("Cantidad de reportes de " + this.codigoVendedor + ":\n\n\t" + res.Rows.Count.ToString());
            }
        }

        private void cbiTodos_Selected(object sender, RoutedEventArgs e)
        {
            this.Visible = Visibility.Collapsed;
        }

        private void cbiUno_Selected(object sender, RoutedEventArgs e)
        {
            this.Visible = Visibility.Visible;
        }

        private void txtCodigoVendedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.codigoVendedor == string.Empty)
                this.btnObtenerPosicionesPorCodigo.IsEnabled = false;
            else
                this.btnObtenerPosicionesPorCodigo.IsEnabled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
