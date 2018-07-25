using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GeoventasPocho.Vistas.ElementosMapa.Pines
{
    public class PinRojo : Pin
    {
        public PinRojo()
            : base()
        {
            this.Height = 35;
            this.Width = 37;
            //var imgs = new ImageSourceConverter().ConvertFromString(@"S:\GeoventasPocho\pin_gris.png");
            //this.Icono.Source = (ImageSource)imgs;
            this.Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_rojo");
            this.Texto.Foreground = Brushes.Black;
            this.Texto.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.Texto.FontWeight = FontWeights.Bold;
            //this.Menu = new System.Windows.Controls.ContextMenu();
        }
    }
}
