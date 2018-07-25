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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeoventasPocho.Vistas.ElementosMapa.Pines
{
    /// <summary>
    /// Interaction logic for Pin.xaml
    /// </summary>
    public partial class Pin : UserControl
    {
        public string Etiqueta
        {
            get { return (string)GetValue(EtiquetaProperty); }
            set { SetValue(EtiquetaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Etiqueta.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EtiquetaProperty =
            DependencyProperty.Register("Etiqueta", typeof(string), typeof(Pin), new PropertyMetadata("Pin"));

        //public ContextMenu Menu
        //{
        //    get { return (ContextMenu)GetValue(MenuProperty); }
        //    set { SetValue(MenuProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty MenuProperty =
        //    DependencyProperty.Register("Menu", typeof(ContextMenu), typeof(Pin), null);

        public Pin()
        {
            InitializeComponent();
        }

        //private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (this.Menu != null)
        //        this.Menu.Focus();
        //}
    }
}
