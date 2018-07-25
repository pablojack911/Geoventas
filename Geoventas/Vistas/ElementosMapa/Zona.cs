using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public class Zona : DependencyObject
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string CodigoEmpresa { get; set; }
        public string CodigoDivision { get; set; }
        public List<PointLatLng> Vertices { get; set; }
        public List<Cliente> Clientes { get; set; }
        //public SolidColorBrush ColorFondo { get; set; }

        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register("Visible", typeof(bool), typeof(Zona), new PropertyMetadata(true));

        public Zona()
        {
            this.Vertices = new List<PointLatLng>();
            this.Clientes = new List<Cliente>();
        }
    }
}
