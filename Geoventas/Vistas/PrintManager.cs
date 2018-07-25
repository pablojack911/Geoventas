using GeoventasPocho.Vistas.ElementosMapa;
using GeoventasPocho.Vistas.Geocoder;
using GeoventasPocho.Vistas.Historico;
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
using System.Windows.Documents;
using System.Windows.Media;

namespace GeoventasPocho.Vistas
{
    public static class PrintManager
    {
        public static void ReporteDePosiciones(string CodigoVendedor, string NombreVendedor, string FechaDelReporte, List<ItemReporte> Posiciones)
        {
            var table1 = new Table();

            // Set some global formatting properties for the table.
            table1.CellSpacing = 10;
            table1.Background = Brushes.White;

            // Create 6 columns and add them to the table's Columns collection.
            //int numberOfColumns = 6;
            //for (int x = 0; x < numberOfColumns; x++)
            //{
            table1.Columns.Add(new TableColumn() { Width = new GridLength(80) }); //CLIENTE
            table1.Columns.Add(new TableColumn() { Width = new GridLength(80) }); //CHECKIN
            table1.Columns.Add(new TableColumn() { Width = new GridLength(80) }); //CHECKOUT
            table1.Columns.Add(new TableColumn() { Width = new GridLength(80) }); //TIEMPO
            //table1.Columns.Add(new TableColumn() { Width = new GridLength(80) }); //1ER CLIENTE VISITADO
            //table1.Columns.Add(new TableColumn() { Width = new GridLength(80) }); //1ER CLIENTE DE RUTA

            //}
            // Create and add an empty TableRowGroup to hold the table's Rows.
            table1.RowGroups.Add(new TableRowGroup());

            //TITULO
            table1.RowGroups[0].Rows.Add(new TableRow());
            // Alias the current working row for easy reference.
            TableRow currentRow = table1.RowGroups[0].Rows[0];
            // Global formatting for the title row.
            currentRow.Background = Brushes.Silver;
            currentRow.FontSize = 25;
            currentRow.FontFamily = new System.Windows.Media.FontFamily("Sans Serif");
            currentRow.FontWeight = System.Windows.FontWeights.Bold;
            // Add the header row with content, 
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("REPORTE DE POSICIONES"))) { TextAlignment = TextAlignment.Center }); //POSICION 0 en ARRAY
            currentRow.Cells[0].ColumnSpan = 4;

            //SUBTITULO
            // Add the second (header) row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[1];
            // Global formatting for the header row.
            currentRow.FontSize = 18;
            currentRow.FontFamily = new System.Windows.Media.FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(CodigoVendedor + " - " + NombreVendedor)))); //POSICION 0 en ARRAY
            currentRow.Cells[0].ColumnSpan = 4;

            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[2];
            // Global formatting for the header row.
            currentRow.FontSize = 18;
            currentRow.FontFamily = new System.Windows.Media.FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(FechaDelReporte)))); //POSICION 0 en ARRAY
            currentRow.Cells[0].ColumnSpan = 4;

            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[3];
            // Global formatting for the header row.
            currentRow.FontSize = 12;
            currentRow.FontFamily = new System.Windows.Media.FontFamily("Sans Serif");
            currentRow.FontWeight = FontWeights.Bold;
            // Add cells with content to the second row.
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("CLIENTE"))) { TextAlignment = TextAlignment.Center });
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("CHECK IN"))) { TextAlignment = TextAlignment.Center });
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("CHECK OUT"))) { TextAlignment = TextAlignment.Center });
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("TIEMPO"))) { TextAlignment = TextAlignment.Center });

            // Add the third row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[4];

            string cli = "";
            string checkin = "";
            string checkout = "";
            string tiempo = "";

            for (int i = 0; i < Posiciones.Count; i++)
            {
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[i + 4];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                currentRow.FontFamily = new System.Windows.Media.FontFamily("Sans Serif");
                cli = Posiciones[i].Cliente;
                checkin = Posiciones[i].CheckIn.TimeOfDay.ToString(@"hh\:mm");
                checkout = Posiciones[i].CheckOut.TimeOfDay.ToString(@"hh\:mm");
                tiempo = Posiciones[i].Tiempo;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(cli))) { TextAlignment = TextAlignment.Center });
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(checkin))) { TextAlignment = TextAlignment.Center });
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(checkout))) { TextAlignment = TextAlignment.Center });
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(tiempo))) { TextAlignment = TextAlignment.Center });
            }

            var fd = new FlowDocument(table1);

            PrintDialog p = new PrintDialog();
            if (p.ShowDialog() != true)
                return;

            fd.PageHeight = p.PrintableAreaHeight;
            fd.PageWidth = p.PrintableAreaWidth;
            fd.PagePadding = new Thickness(25);

            fd.ColumnGap = 0;

            fd.ColumnWidth = (fd.PageWidth -
                           fd.ColumnGap -
                           fd.PagePadding.Left -
                           fd.PagePadding.Right);

            p.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, "Imprimiendo...");
        }

        public static void ReporteDePosicionesExcel(string CodigoVendedor, string NombreVendedor, string FechaDelReporte, List<ItemReporte> Posiciones)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            object misValue = System.Reflection.Missing.Value;
            if (xlApp == null)
            {
                MessageBox.Show("Excel no está instalado en el equipo.");
                return;
            }
            var xlWorkBook = xlApp.Workbooks.Add(misValue);
            var xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "Cliente";
            xlWorkSheet.Cells[1, 2] = "Check in";
            xlWorkSheet.Cells[1, 3] = "Check out";
            xlWorkSheet.Cells[1, 4] = "Tiempo";
            xlWorkSheet.Cells[1, 5] = "Estado";
            int i = 2;
            foreach (ItemReporte item in Posiciones)
            {
                xlWorkSheet.Cells[i, 1] = item.Cliente;
                xlWorkSheet.Cells[i, 2] = item.CheckIn;
                xlWorkSheet.Cells[i, 3] = item.CheckOut;
                xlWorkSheet.Cells[i, 4] = item.Tiempo;
                xlWorkSheet.Cells[i, 5] = item.TipoVisita.ToString();
                i++;
            }
            
            xlApp.Visible = true;

            //string PathOfFile = "";
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //var DefaultOpenPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //if (Directory.Exists(DefaultOpenPath))
            //{
            //    saveFileDialog.InitialDirectory = DefaultOpenPath;
            //}
            //else
            //{
            //    saveFileDialog.InitialDirectory = @"C:\";
            //}
            //saveFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            //saveFileDialog.FilterIndex = 0;
            //saveFileDialog.RestoreDirectory = true;

            //if (saveFileDialog.ShowDialog().HasValue)
            //{
            //    PathOfFile = saveFileDialog.FileName;
            //}
            //try
            //{
            //    //    var file = new FileStream(PathOfFile, FileMode.CreateNew, FileAccess.ReadWrite);
            //    //    var HojaDeCalculo = WorkbookFactory.Create(file);
            //    //    HojaDeCalculo.CreateSheet(CodigoVendedor + " - " + FechaDelReporte);
            //    //    var Sheet = HojaDeCalculo.GetSheetAt(0);
            //    //    for (int i = 0; i < Posiciones.Capacity; i++)
            //    //    {
            //    //        var row = Sheet.CreateRow(i);
            //    //        row.CreateCell()

            //    //    }
            //    //xlWorkBook.SaveAs(CodigoVendedor + "_" + DateTime.Now.ToString("yyyy-MM-dd"));
            //    xlWorkBook.SaveAs(PathOfFile, ".xlsx");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
            //}
        }
    }
}

