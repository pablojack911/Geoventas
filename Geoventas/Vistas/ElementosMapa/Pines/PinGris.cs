﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GeoventasPocho.Vistas.ElementosMapa.Pines
{
    public class PinGris : Pin
    {
        public PinGris()
            : base()
        {
            this.Height = 40;
            this.Width = 42;
            this.Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_gris");
            this.Texto.Foreground = Brushes.Black;
            this.Texto.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.Texto.FontWeight = FontWeights.Bold;
        }
    }
}
