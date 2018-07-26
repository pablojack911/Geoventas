using GeoventasPocho.Controladores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoventasPocho.Factory.Pines
{
    public class Pin : UserControl
    {
        public Image Icono { get; set; }
        public TextBlock Etiqueta { get; set; }

        public Pin()
        {
            Grid grid = new Grid();
            Etiqueta = new TextBlock
            {
                Margin = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                Text = string.Empty
            };
            Icono = new Image();
            grid.Children.Add(Icono);
            grid.Children.Add(Etiqueta);
            Content = grid;
        }

        public void setTipo(TipoPin tipo)
        {
            switch (tipo)
            {
                case TipoPin.Amarillo:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_amarillo");
                    break;
                case TipoPin.Azul:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_azul");
                    break;
                case TipoPin.Casa:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_casa");
                    break;
                case TipoPin.Celeste:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_celeste");
                    break;
                case TipoPin.Cerrado:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("cerrado");
                    break;
                case TipoPin.EntregaParcial:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("entrega_parcial");
                    break;
                case TipoPin.EntregaTotal:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("entrega_total");
                    break;
                case TipoPin.Fucsia:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_fucsia");
                    break;
                case TipoPin.Gris:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_gris");
                    break;
                case TipoPin.Naranja:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_naranja");
                    break;
                case TipoPin.Rechazado:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("rechazado");
                    break;
                case TipoPin.Rojo:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_rojo");
                    break;
                case TipoPin.SinVisitar:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("sin_visitar");
                    break;
                case TipoPin.Verde:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("pin_verde");
                    break;
                case TipoPin.VuelveLuego:
                    Icono.Source = ResourceHelper.LoadBitmapFromResource("volver");
                    break;
                default:
                    break;
            }
        }

        internal void setEtiqueta(string label)
        {
            Etiqueta.Text = label;
        }
    }
}
