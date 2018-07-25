using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GeoventasPocho.Vistas.ElementosMapa.Pines.Logistica
{
    public class PinVisitaPendiente : Pin
    {
        public PinVisitaPendiente() : base()
        {
            this.Height = 40;
            this.Width = 42;
            this.Icono.Source = ResourceHelper.LoadBitmapFromResource("Logistica/pin_amarillo");
            this.Texto.Foreground = Brushes.Black;
            this.Texto.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.Texto.FontWeight = FontWeights.Bold;
        }
    }
}
