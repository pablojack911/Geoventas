using GeoventasPocho.Controladores;
using GeoventasPocho.Vistas.ElementosMapa;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace GeoventasPocho.Vistas.Asistencias
{
    /// <summary>
    /// Interaction logic for AsistenciasWindow.xaml
    /// </summary>
    public partial class AsistenciasWindow : Window
    {
        Random rnd;
        BackgroundWorker bgWorker;

        public ObservableCollection<Vendedor> PresentesHiller
        {
            get { return (ObservableCollection<Vendedor>)GetValue(PresentesHillerProperty); }
            set { SetValue(PresentesHillerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresentesHiller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresentesHillerProperty =
            DependencyProperty.Register("PresentesHiller", typeof(ObservableCollection<Vendedor>), typeof(AsistenciasWindow));

        public ObservableCollection<Vendedor> PresentesAlta
        {
            get { return (ObservableCollection<Vendedor>)GetValue(PresentesAltaProperty); }
            set { SetValue(PresentesAltaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresentesAlta.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresentesAltaProperty =
            DependencyProperty.Register("PresentesAlta", typeof(ObservableCollection<Vendedor>), typeof(AsistenciasWindow));



        public ObservableCollection<Vendedor> AusentesHiller
        {
            get { return (ObservableCollection<Vendedor>)GetValue(AusentesHillerProperty); }
            set { SetValue(AusentesHillerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AusentesHiller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AusentesHillerProperty =
            DependencyProperty.Register("AusentesHiller", typeof(ObservableCollection<Vendedor>), typeof(AsistenciasWindow));

        public ObservableCollection<Vendedor> AusentesAlta
        {
            get { return (ObservableCollection<Vendedor>)GetValue(AusentesAltaProperty); }
            set { SetValue(AusentesAltaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AusentesAlta.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AusentesAltaProperty =
            DependencyProperty.Register("AusentesAlta", typeof(ObservableCollection<Vendedor>), typeof(AsistenciasWindow));

        public AsistenciasWindow()
        {
            InitializeComponent();
            rnd = new Random();
            bgWorker = new BackgroundWorker();
            this.PresentesAlta = new ObservableCollection<Vendedor>();
            this.PresentesHiller = new ObservableCollection<Vendedor>();
            this.AusentesAlta = new ObservableCollection<Vendedor>();
            this.AusentesHiller = new ObservableCollection<Vendedor>();

            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.ProgressChanged +=
                new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.WorkerReportsProgress = true;
            this.DataContext = this;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progress.Visibility = Visibility.Collapsed;
            btnRefrescar.IsEnabled = true;
            btnImprimeAlta.IsEnabled = true;
            btnImprimeHiller.IsEnabled = true;
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var vendedor = (Vendedor)e.UserState;
            if (vendedor.Presente)
            {
                if (vendedor.CodigoEmpresa == "10")
                {
                    this.PresentesAlta.Add(vendedor);
                }
                else
                {
                    this.PresentesHiller.Add(vendedor);
                }
            }
            else
            {
                if (vendedor.CodigoEmpresa == "10")
                {
                    this.AusentesAlta.Add(vendedor);
                }
                else
                {
                    this.AusentesHiller.Add(vendedor);
                }
            }
            progress.Value = e.ProgressPercentage;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.ActualizaAsistenciaVendedores();
        }

        private void ActualizaAsistenciaVendedores()
        {
            try
            {
                var todosLosVendedores = ControladorVendedores.ObtenerCodigosVendedores(null);
                var vendedoresPresentes = ControladorVendedores.ObtenerVendedoresPorFecha(DateTime.Today);
                int cantidadVendedores = vendedoresPresentes.Count;
                int i = 0;
                double progreso = 0;

                foreach (var item in todosLosVendedores)
                {
                    i++;
                    Vendedor vend = null;
                    if (vendedoresPresentes.Any(v => v.Codigo == item))
                    {
                        vend = vendedoresPresentes.Find(v => v.Codigo == item);
                        vend.Presente = true;
                    }
                    else
                    {
                        vend = ControladorVendedores.ObtenerVendedor(item);
                        vend.Presente = false;
                    }
                    Thread.Sleep(rnd.Next(125, 1000));
                    progreso = Math.Ceiling((double)(i * 100) / cantidadVendedores);
                    bgWorker.ReportProgress(Convert.ToInt32(progreso), vend);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void ActualizarHiller()
        //{
        //    try
        //    {
        //        var presentesHiller = new List<Tuple<string, string, string>>(); //codigo, nombre, zona
        //        string empresa = "01";
        //        var todosLosVendedoresHiller = ControladorVendedores.ObtenerCodigosVendedores(empresa);
        //        var vendedoresPresentesHiller = ControladorVendedores.ObtenerVendedoresPorFecha(DateTime.Today);
        //        int cantidadVendedoresHiller = todosLosVendedoresHiller.Count;
        //        int i = 0;
        //        double progreso = 0;
        //        var ausentesHiller = new List<Tuple<string, string>>();
        //        foreach (var item in todosLosVendedoresHiller)
        //        {
        //            i++;
        //            Vendedor vend = null;
        //            if (vendedoresPresentesHiller.Any(v => v.Codigo == item))
        //            {
        //                vend = vendedoresPresentesHiller.Find(v => v.Codigo == item);
        //                vend.Presente = true;
        //                presentesHiller.Add(new Tuple<string, string, string>(vend.Codigo, vend.Nombre, vend.ZonasClienteParaGrilla));
        //            }
        //            else
        //            {
        //                vend = ControladorVendedores.ObtenerVendedor(item);
        //                vend.Presente = false;
        //                if (vend != null)
        //                {
        //                    ausentesHiller.Add(new Tuple<string, string>(vend.Codigo, vend.Nombre));
        //                }
        //            }
        //            Thread.Sleep(rnd.Next(125, 1000));
        //            progreso = Math.Ceiling((double)(i * 100) / cantidadVendedoresHiller);
        //            bgWorker.ReportProgress(Convert.ToInt32(progreso), vend);
        //        }
        //        //this.AusentesHiller = new ObservableCollection<string>(todosLosVendedoresHiller.Except(vendedoresPresentesHiller.Select(c => c.Codigo)));
        //        //this.CrearInformeAsistencias(presentesHiller, ausentesHiller, "HILLER SA");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void btnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            this.Actualizar();
        }

        private void Actualizar()
        {
            btnRefrescar.IsEnabled = false;
            btnImprimeAlta.IsEnabled = false;
            btnImprimeHiller.IsEnabled = false;

            PresentesHiller.Clear();
            PresentesAlta.Clear();
            AusentesHiller.Clear();
            AusentesAlta.Clear();

            progress.Value = 0;
            progress.Visibility = Visibility.Visible;

            bgWorker.RunWorkerAsync();
        }

        private void btnImprimeHiller_Click(object sender, RoutedEventArgs e)
        {
            if (this.PresentesHiller.Count == 0)
                this.Actualizar();
            else
            {
                var presentesHiller = new List<Tuple<string, string, string>>(); //codigo, nombre, zona
                var ausentesHiller = new List<Tuple<string, string>>();

                foreach (var vend in this.PresentesHiller)
                {
                    presentesHiller.Add(new Tuple<string, string, string>(vend.Codigo, vend.Nombre, vend.ZonasClienteParaGrilla));
                }
                foreach (var vend in this.AusentesHiller)
                {
                    ausentesHiller.Add(new Tuple<string, string>(vend.Codigo, vend.Nombre));
                }
                this.CrearInformeAsistencias(presentesHiller, ausentesHiller, "Hiller S.A.");
            }
        }

        private void btnImprimeAlta_Click(object sender, RoutedEventArgs e)
        {
            if (this.PresentesAlta.Count == 0)
                this.Actualizar();
            else
            {
                var presentesAlta = new List<Tuple<string, string, string>>(); //codigo, nombre, zona
                var ausentesAlta = new List<Tuple<string, string>>();

                foreach (var vend in this.PresentesAlta)
                {
                    presentesAlta.Add(new Tuple<string, string, string>(vend.Codigo, vend.Nombre, vend.ZonasClienteParaGrilla));
                }
                foreach (var vend in this.AusentesAlta)
                {
                    ausentesAlta.Add(new Tuple<string, string>(vend.Codigo, vend.Nombre));
                }
                this.CrearInformeAsistencias(presentesAlta, ausentesAlta, "Alta Distribución S.A.");
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
    }
}
