using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GeoventasPocho.Vistas.ClientesRojaAzul
{
    /// <summary>
    /// Interaction logic for VisualizadorClientesRojaAzul.xaml
    /// </summary>
    public partial class VisualizadorClientesRojaAzul : Window
    {
        BackgroundWorker bgWorker;
        public VisualizadorClientesRojaAzul()
        {
            InitializeComponent();
            this.DataContext = this;
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.ProgressChanged +=
                new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.WorkerReportsProgress = true;

        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progress.Visibility = Visibility.Collapsed;
            this.menues.IsEnabled = true;
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnTodos_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnConRecorrido_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOcultarClientes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSinRecorrido_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
