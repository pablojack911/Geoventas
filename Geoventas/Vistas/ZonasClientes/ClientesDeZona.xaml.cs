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

namespace GeoventasPocho.Vistas.ZonasClientes
{
    /// <summary>
    /// Interaction logic for ClientesDeZona.xaml
    /// </summary>
    public partial class ClientesDeZona : UserControl
    {


        public int cantidadClientes
        {
            get { return (int)GetValue(cantidadClientesProperty); }
            set { SetValue(cantidadClientesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for cantidadClientes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty cantidadClientesProperty =
            DependencyProperty.Register("cantidadClientes", typeof(int), typeof(ClientesDeZona));


        public ObservableCollection<Cliente> Clientes
        {
            get { return (ObservableCollection<Cliente>)GetValue(ClientesProperty); }
            set { SetValue(ClientesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Clientes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClientesProperty =
            DependencyProperty.Register("Clientes", typeof(ObservableCollection<Cliente>), typeof(ClientesDeZona));


        public ClientesDeZona()
        {
            InitializeComponent();
            //this.Clientes = new ObservableCollection<Cliente>();
            this.DataContext = this;
        }
    }
}
