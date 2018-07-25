using GeoventasPocho.Controladores;
using GeoventasPocho.Vistas.ActividadesClientes;
using GeoventasPocho.Vistas.Asistencias;
//using GeoventasPocho.Vistas.ClientesRojaAzul;
using GeoventasPocho.Vistas.Geocoder;
using GeoventasPocho.Vistas.Historico;
using GeoventasPocho.Vistas.HistoricoLogistica;
using GeoventasPocho.Vistas.MapaConsulta;
using GeoventasPocho.Vistas.PosicionesActuales;
using GeoventasPocho.Vistas.PosicionesActualesLogisticaWorker;
using GeoventasPocho.Vistas.ReportesPorDiaPorVendedor;
using GeoventasPocho.Vistas.ZonasClientes;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace GeoventasPocho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mnuTotalReportesDelDia_Click(object sender, RoutedEventArgs e)
        {
            var ventasPorDia = new ReportesPorDiaPorVendedor();
            ventasPorDia.ShowDialog();
        }

        private void mnuGeoposicionVendedoresActual_Click(object sender, RoutedEventArgs e)
        {
            var vendedores = new MapaPosicionesActuales();
            vendedores.Show();
        }

        private void mnuLeerYGrabar_Click(object sender, RoutedEventArgs e)
        {
            //var geocode = new GeocodificadorDeCalles();
            var geocode = new GeocodificadorDeCallesExcel();
            geocode.Show();
        }

        private void mnuZonas_Click(object sender, RoutedEventArgs e)
        {
            var zonacli = new VisualizadorZonasClientes();
            zonacli.Show();
        }

        private void mnuGeoposicionHistoricaVende_Click(object sender, RoutedEventArgs e)
        {
            var historico = new HistorialPosiciones();
            historico.Show();
        }

        private void mnuActividades_Click(object sender, RoutedEventArgs e)
        {
            var actividades = new VisualizadorActividadesClientes();
            actividades.Show();
        }

        private void mnuAsistenciasAlta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var presentesAlta = new List<Tuple<string, string, string>>(); //codigo, nombre, zona
                var empresa = "10";
                var todosLosVendedoresAlta = ControladorVendedores.ObtenerCodigosVendedores(empresa);
                var vendedoresPresentesAlta = ControladorVendedores.ObtenerVendedoresPorFecha(empresa, DateTime.Today);

                var ausentesAlta = new List<Tuple<string, string>>();
                foreach (var item in todosLosVendedoresAlta)
                {
                    if (vendedoresPresentesAlta.Any(v => v.Codigo == item))
                    {
                        var vend = vendedoresPresentesAlta.Find(v => v.Codigo == item);
                        presentesAlta.Add(new Tuple<string, string, string>(vend.Codigo, vend.Nombre, vend.ZonasClienteParaGrilla));
                    }
                    else
                    {
                        var vend = ControladorVendedores.ObtenerVendedor(item);
                        if (vend != null)
                        {
                            ausentesAlta.Add(new Tuple<string, string>(vend.Codigo, vend.Nombre));
                        }
                    }
                }

                this.CrearInformeAsistencias(presentesAlta, ausentesAlta, "ALTA DISTRIBUCION SA");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuasistenciasHergo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var presentesHiller = new List<Tuple<string, string, string>>(); //codigo, nombre, zona
                string empresa = "01";
                var todosLosVendedoresHiller = ControladorVendedores.ObtenerCodigosVendedores(empresa);
                var vendedoresPresentesHiller = ControladorVendedores.ObtenerVendedoresPorFecha(empresa, DateTime.Today);

                var ausentesHiller = new List<Tuple<string, string>>();
                foreach (var item in todosLosVendedoresHiller)
                {
                    if (vendedoresPresentesHiller.Any(v => v.Codigo == item))
                    {
                        var vend = vendedoresPresentesHiller.Find(v => v.Codigo == item);
                        presentesHiller.Add(new Tuple<string, string, string>(vend.Codigo, vend.Nombre, vend.ZonasClienteParaGrilla));
                    }
                    else
                    {
                        var vend = ControladorVendedores.ObtenerVendedor(item);
                        if (vend != null)
                        {
                            ausentesHiller.Add(new Tuple<string, string>(vend.Codigo, vend.Nombre));
                        }
                    }
                }

                this.CrearInformeAsistencias(presentesHiller, ausentesHiller, "HILLER SA");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void CrearInformeAsistencias(List<Tuple<string, string, string>> presentes, List<Tuple<string, string>> ausentes, string empresa)
        {
            var table1 = new Table();

            table1.CellSpacing = 4;
            table1.Background = Brushes.White;

            table1.Columns.Add(new TableColumn() { Width = new GridLength(80) }); //CODIGO
            table1.Columns.Add(new TableColumn() { Width = new GridLength(140) }); //NOMBRE
            table1.Columns.Add(new TableColumn() { Width = new GridLength(250) }); //ZONAS
            table1.Columns.Add(new TableColumn() { Width = new GridLength(230) }); //ZONAS


            table1.RowGroups.Add(new TableRowGroup());

            //var ultimaFila = table1.RowGroups[0].Rows.Count;

            //TITULO
            table1.RowGroups[0].Rows.Add(new TableRow());
            TableRow currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            currentRow.Background = Brushes.Silver;
            currentRow.FontSize = 25;
            currentRow.FontFamily = new FontFamily("Sans Serif");
            currentRow.FontWeight = System.Windows.FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("INFORME DE ASISTENCIAS DE " + empresa))) { TextAlignment = TextAlignment.Center }); //POSICION 0 en ARRAY
            currentRow.Cells[0].ColumnSpan = table1.Columns.Count;

            //SUBTITULO
            // Add the second (header) row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            // Global formatting for the header row.
            currentRow.FontSize = 18;
            currentRow.FontFamily = new FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\nReporte generado: " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"))))); //POSICION 0 en ARRAY
            currentRow.Cells[0].ColumnSpan = table1.Columns.Count;

            //SUBTITULO 2
            // Add the second (header) row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            // Global formatting for the header row.
            currentRow.FontSize = 18;
            currentRow.FontFamily = new FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\nPRESENTES\n")) { TextAlignment = TextAlignment.Center })); //POSICION 0 en ARRAY
            currentRow.Cells[0].ColumnSpan = table1.Columns.Count;

            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            // Global formatting for the header row.
            currentRow.FontSize = 12;
            currentRow.FontFamily = new FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            // Add cells with content to the second row.
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("CODIGO"))) { TextAlignment = TextAlignment.Center });
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("NOMBRE"))) { TextAlignment = TextAlignment.Center });
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("ZONAS ASIGNADAS"))) { TextAlignment = TextAlignment.Center });

            // Add the third row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            string cod = "";
            string nombre = "";
            string zonasAsignadas = "";

            for (int i = 0; i < presentes.Count; i++)
            {
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                currentRow.FontFamily = new FontFamily("Sans Serif");
                if (i % 2 == 0)
                    currentRow.Background = Brushes.AliceBlue;
                cod = presentes[i].Item1;
                nombre = presentes[i].Item2;
                zonasAsignadas = presentes[i].Item3;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(cod))) { TextAlignment = TextAlignment.Center });
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(nombre))) { TextAlignment = TextAlignment.Center });
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(zonasAsignadas))) { TextAlignment = TextAlignment.Center });
            }

            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            // Global formatting for the header row.
            currentRow.FontSize = 16;
            currentRow.FontFamily = new FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("\nAUN NO REPORTAN\n")) { TextAlignment = TextAlignment.Center })); //POSICION 0 en ARRAY
            currentRow.Cells[0].ColumnSpan = table1.Columns.Count;

            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            // Global formatting for the header row.
            currentRow.FontSize = 12;
            currentRow.FontFamily = new FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            // Add cells with content to the second row.
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("CODIGO"))) { TextAlignment = TextAlignment.Center });
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("NOMBRE"))) { TextAlignment = TextAlignment.Center });

            // Add the third row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
            //string cod = "";
            //string nombre = "";
            //string zonasAsignadas = "";

            for (int i = 0; i < ausentes.Count; i++)
            {
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                currentRow.FontFamily = new FontFamily("Sans Serif");
                if (i % 2 == 0)
                    currentRow.Background = Brushes.AliceBlue;
                cod = ausentes[i].Item1;
                nombre = ausentes[i].Item2;
                //zonasAsignadas = presentes[i].Item3;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(cod))) { TextAlignment = TextAlignment.Center });
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(nombre))) { TextAlignment = TextAlignment.Center });
                //currentRow.Cells.Add(new TableCell(new Paragraph(new Run(zonasAsignadas))) { TextAlignment = TextAlignment.Center });
            }


            PrintDialog p = new PrintDialog();
            if (p.ShowDialog() != true)
                return;

            var fd = new FlowDocument(table1);
            fd.PageHeight = p.PrintableAreaHeight;
            fd.PageWidth = p.PrintableAreaWidth;
            fd.PagePadding = new Thickness(25);

            fd.ColumnGap = 0;

            fd.ColumnWidth = (fd.PageWidth -
                           fd.ColumnGap -
                           fd.PagePadding.Left -
                           fd.PagePadding.Right);


            p.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, "Asistencias " + empresa + " - " + DateTime.Now.ToString("yyyy'-'MM'-'dd'- 'HH mm"));
        }

        private void mnuCreaConsulta_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VisualizadorClientesPorConsulta();
            ventana.Show();
        }

        private void mnuAsistencias_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new AsistenciasWindow();
            ventana.Show();
        }

        private void mnuPosicionActualLogistica_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new PosicionesActualesLogistica();
            ventana.Show();
        }

        private void mnuPosicionHistoricaLogistica_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new HistorialPosicionesLogistica();
            ventana.Show();
        }

        //private void mnuClientesRojoAzul_Click(object sender, RoutedEventArgs e)
        //{
        //    var ventana = new VisualizadorClientesRojaAzul();
        //    ventana.Show();
        //}
    }
}
